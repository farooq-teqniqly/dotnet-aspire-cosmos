---
description: Git workflow and remote operations safety guidelines
author: team
version: 1.0
globs: ["**/*"]
tags: ["git-workflow", "safety"]
---

# Git Workflow Rules

## Branch Management

- **Never commit directly to main branch**: All code changes must be committed to feature branches only
- **Feature branch requirement**: Create and use descriptive feature branches for all development work
- **Branch selection**: If unsure which feature branch to use, always ask the user for clarification before proceeding

## Remote Operations

- **No unauthorized pushes**: Never push to remote repositories without explicit user permission
- **Push confirmation**: Always confirm with the user before executing any `git push` commands
- **Remote safety**: Prioritize local commits over remote operations to maintain user control

## General Guidelines

- Follow standard git branching workflow (feature branches, pull requests, etc.)
- Maintain clean commit history with meaningful commit messages
- Respect user's git workflow preferences and security boundaries
