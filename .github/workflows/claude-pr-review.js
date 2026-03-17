const fs = require('fs');
const https = require('https');

async function makeClaudeRequest() {
  // Read files
  const diff = fs.readFileSync('pr_diff.txt', 'utf8');
  let agentsContext = '';
  try {
    agentsContext = fs.readFileSync('AGENTS.md', 'utf8');
  } catch (e) {
    console.log('AGENTS.md not found');
  }

  const title = process.env.PR_TITLE || 'No title';
  const description = process.env.PR_BODY || 'No description';
  const author = process.env.PR_AUTHOR || 'Unknown';

  const prompt = `You are reviewing a pull request for the ProjectMarworyn project.

## Project Context

This is a .NET 9 console application for population simulation. Please review the code with the project's standards in mind.

### Project Standards (from AGENTS.md):
\`\`\`
${agentsContext}
\`\`\`

## Pull Request Details
**Title:** ${title}
**Description:** ${description}
**Author:** ${author}

## Changes
\`\`\`diff
${diff}
\`\`\`

## Review Instructions

Please provide a thorough code review covering:
1. **Code Quality**: Does the code follow the project's coding standards?
2. **Architecture**: Does it fit the established patterns (DI, service extension pattern)?
3. **Potential Issues**: Any bugs, edge cases, or problems?
4. **Best Practices**: .NET 9 best practices and patterns
5. **Performance**: Any performance concerns?
6. **Testing**: Should unit tests be added or updated?
7. **Positive Feedback**: What's done well?

Format your review as:
- Use markdown
- Start with a summary (approve/request changes/comment)
- Use emoji to categorize feedback (🎯 for critical, ⚠️ for suggestions, ✅ for positives)
- Be constructive and specific
- Provide code examples for suggestions when helpful`;

  const requestBody = JSON.stringify({
    model: 'claude-sonnet-4-20250514',
    max_tokens: 4096,
    messages: [
      {
        role: 'user',
        content: prompt
      }
    ]
  });

  const options = {
    hostname: 'api.anthropic.com',
    path: '/v1/messages',
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'x-api-key': process.env.ANTHROPIC_API_KEY,
      'anthropic-version': '2023-06-01',
      'Content-Length': Buffer.byteLength(requestBody)
    }
  };

  return new Promise((resolve, reject) => {
    const req = https.request(options, (res) => {
      let data = '';

      res.on('data', (chunk) => {
        data += chunk;
      });

      res.on('end', () => {
        if (res.statusCode >= 200 && res.statusCode < 300) {
          resolve(data);
        } else {
          reject(new Error(`API request failed with status ${res.statusCode}: ${data}`));
        }
      });
    });

    req.on('error', (error) => {
      reject(error);
    });

    req.write(requestBody);
    req.end();
  });
}

makeClaudeRequest()
  .then(responseData => {
    const response = JSON.parse(responseData);
    const review = response.content[0].text;
    fs.writeFileSync('claude_review.txt', review);
    console.log('Review generated successfully');
  })
  .catch(error => {
    console.error('Error:', error.message);
    process.exit(1);
  });
