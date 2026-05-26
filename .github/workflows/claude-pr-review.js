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

## Critical: Pre-Flight Rules

Before writing any feedback item, check it against the "Do Not Flag As Issues" section of AGENTS.md below. If your observation matches anything listed there, discard it entirely and do not include it in your review in any form. These are hard rules, not guidelines. Violating them by flagging a documented intentional choice is itself an error in your review.

## Required Decision Procedure

For every candidate observation, apply this procedure in order before writing anything:

1. Identify the rule the code would violate.
2. Determine the category of the observation. For formatting, naming, and code style of any kind, the rule must be explicitly documented in README.md or AGENTS.md — if no documented rule exists, discard the observation. Project standards always take precedence over any external convention including Microsoft guidelines. For correctness (bugs, logic errors), security vulnerabilities, or performance issues, a documented project rule is not required.
3. Check every exception and "Do Not Flag" entry that could apply. If any applies, discard the observation.
4. Only if the observation survives steps 1–3 without being discarded may it appear in your output.

You must complete this procedure silently. Do not describe your reasoning, do not show your working, and do not mention candidates that were discarded. The output contains only confirmed violations — nothing else.

## Project Context

This is a .NET 10 MonoGame population simulation. Please review the code with the project's standards in mind.

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
8. **Positive Feedback**: What's done well? Maximum four bullet points.

Format your review as:
- Use markdown
- Start with a summary (approve/request changes/comment)
- Your response must contain only confirmed violations. Complete all analysis internally before writing a single word of output. Do not write up candidates that were discarded — your working is not part of the review.
- The following output patterns are banned entirely:
  - Showing a code snippet and then concluding it is fine, correct, or not a violation
  - Using "However" or "But" to transition from correct code to other code
  - Any sentence containing "this is fine", "this is correct", "no issue", "not a violation", "is compliant", or equivalent phrasing
  - Noting that you checked something and found nothing wrong
  - Phrases like "No formatting violations confirmed" or "The formatting throughout X is consistent"
- If your analysis of a file or block yields zero confirmed violations, that file or block must not appear in your response at all.
- Use emoji to categorize feedback (🎯 for critical, ⚠️ for suggestions, ✅ for positives)
- Every review item's heading must accurately describe the content of that item. Do not write a heading about one topic and then discuss a different topic underneath it.
- Be constructive and specific
- Provide code examples for suggestions when helpful`;

  const client = new Anthropic();

  const stream = client.messages.stream({
    model: 'claude-sonnet-4-6',
    max_tokens: 16000,
    thinking: { type: 'adaptive' },
    output_config: { effort: 'medium' },
    messages: [{ role: 'user', content: prompt }]
  });

  const message = await stream.finalMessage();

  console.log('Stop reason:', message.stop_reason);
  console.log('Output tokens:', message.usage?.output_tokens);
  console.log('Response blocks:', message.content.map(b => `${b.type}(${b.type === 'text' ? b.text.length : b.thinking?.length ?? 0} chars)`).join(', '));

  const textBlock = message.content.find(block => block.type === 'text');
  return textBlock ? textBlock.text : '';
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
