const fs = require('fs');
const Anthropic = require('@anthropic-ai/sdk');

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

  let linkedIssues = [];
  try {
    linkedIssues = JSON.parse(fs.readFileSync('linked_issues.json', 'utf8'));
  } catch (e) {
    console.log('No linked issues found');
  }

  const title = process.env.PR_TITLE || 'No title';
  const description = process.env.PR_BODY || 'No description';
  const author = process.env.PR_AUTHOR || 'Unknown';

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
