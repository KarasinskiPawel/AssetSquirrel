---
description: This skill provides a security review of code, identifying potential vulnerabilities and suggesting improvements.
disable-model-invocation: 
argument-hint: <branch-or-path>
---

## Security Review Skill

!'git diff' is a command that shows the differences between two versions of code. It can be used to identify changes that may introduce security vulnerabilities.

Audit the changes above for:

1. Injection vulnerabilities (e.g., SQL injection, command injection)
2. Authentication and authorization issues (e.g., missing access controls, insecure password handling)
3. Hardcoded secrets (e.g., API keys, passwords)

Use checklists.md in the skill directory for the full review checklist.

Report findings with severity ratings and remediation steps.