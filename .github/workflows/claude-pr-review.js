const fs = require('fs');
const https = require('https');
const Anthropic = require('@anthropic-ai/sdk');

function fetchIssue(token, repo, number) {
  return new Promise((resolve, reject) => {
    const options = {
      hostname: 'api.github.com',
      path: `/repos/${repo}/issues/${number}`,
      headers: {
        'Authorization': `Bearer ${token}`,
        'User-Agent': 'claude-pr-review',
        'Accept': 'application/vnd.github.v3+json'
      }
    };
    https.get(options, (res) => {
      let data = '';
      res.on('data', chunk => { data += chunk; });
      res.on('end', () => {
        try { resolve(JSON.parse(data)); } catch (e) { reject(e); }
      });
    }).on('error', reject);
  });
}

async function fetchLinkedIssues(prBody, token, repo) {
  const patterns = [
    /(?:closes?|fixes?|resolves?)\s+#(\d+)/gi,
    /issues\/(\d+)/gi
  ];
  const numbers = new Set();
  let match;
  for (const pattern of patterns) {
    while ((match = pattern.exec(prBody)) !== null) {
      numbers.add(parseInt(match[1]));
    }
  }
  const issues = [];
  for (const number of numbers) {
    try {
      const data = await fetchIssue(token, repo, number);
      if (data.number) {
        issues.push({
          number: data.number,
          title: data.title,
          body: data.body || '',
          labels: (data.labels || []).map(l => l.name)
        });
      }
    } catch (e) {
      console.log(`Could not fetch issue #${number}: ${e.message}`);
    }
  }
  return issues;
}

async function makeClaudeRequest() {
  const diff = fs.readFileSync('pr_diff.txt', 'utf8');
  let readmeContext = '';
  try {
    readmeContext = fs.readFileSync('README.md', 'utf8');
  } catch (e) {
    console.log('README.md not found');
  }

  let agentsContext = '';
  try {
    agentsContext = fs.readFileSync('AGENTS.md', 'utf8');
  } catch (e) {
    console.log('AGENTS.md not found');
  }

  const title = process.env.PR_TITLE || 'No title';
  const description = process.env.PR_BODY || 'No description';
  const author = process.env.PR_AUTHOR || 'Unknown';
  const token = process.env.GITHUB_TOKEN || '';
  const repo = process.env.GITHUB_REPOSITORY || '';

  const linkedIssues = token && repo
    ? await fetchLinkedIssues(description, token, repo)
    : [];

  const prompt = `You are reviewing a pull request for the ProjectMarworyn project.

## Project Context

This is a .NET 10 console application for population simulation. Please review the code with the project's standards in mind.

### Project Standards (from README.md):
\`\`\`
${readmeContext}
\`\`\`

### AI Agent Supplementary Instructions (from AGENTS.md):
\`\`\`
${agentsContext}
\`\`\`

## Pull Request Details
**Title:** ${title}
**Description:** ${description}
**Author:** ${author}
${linkedIssues.length > 0 ? `
## Linked Issues

${linkedIssues.map(issue => `### Issue #${issue.number}: ${issue.title}
${issue.labels.length > 0 ? `**Labels:** ${issue.labels.join(', ')}` : ''}

${issue.body}`).join('\n\n')}
` : ''}
## Changes
\`\`\`diff
${diff}
\`\`\`

## Review Instructions

Please provide a thorough code review covering:
1. **Code Quality**: Does the code follow the project's coding standards?
2. **Architecture**: Does it fit the established patterns (DI, service extension pattern)?
3. **Potential Issues**: Any bugs, edge cases, or problems?
4. **Best Practices**: .NET 10 best practices and patterns
5. **Performance**: Any performance concerns?
6. **Testing**: Should unit tests be added or updated?
7. **Linked Issues**: If linked issues are provided, does this PR fully address them? Call out any gaps.
8. **Positive Feedback**: What's done well?

Format your review as:
- Use markdown
- Start with a summary (approve/request changes/comment)
- Use emoji to categorize feedback (🎯 for critical, ⚠️ for suggestions, ✅ for positives)
- Be constructive and specific
- Provide code examples for suggestions when helpful`;

  const client = new Anthropic();

  const message = await client.messages.create({
    model: 'claude-sonnet-4-6',
    max_tokens: 4096,
    messages: [{ role: 'user', content: prompt }]
  });

  return message.content[0].text;
}

makeClaudeRequest()
  .then(review => {
    fs.writeFileSync('claude_review.txt', review);
    console.log('Review generated successfully');
  })
  .catch(error => {
    console.error('Error:', error.message);
    process.exit(1);
  });
