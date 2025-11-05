---
description: Workflow for implementing code review feedback
author: team
version: 1.0
globs: ["**/*"]
tags: ["code-review", "workflow"]
---

# Code Review Implementation Workflow

## Process Overview

When conducting and implementing code review results:

1.  **Generate Review Document**

    -   Perform comprehensive code review per coding standards
    -   Create a timestamped Markdown file in the `code_reviews` folder (e.g., `code_reviews/code-review-results-2025-11-05-143022.md`)
    -   Document all findings with severity levels, file references, and suggested fixes

2.  **User Feedback Process**

    -   User reviews the code review document
    -   For each finding, user adds feedback directly in the document
    -   Look for: `**Feedback**: Accepted, you may proceed with this change.`
    -   Only implement changes that have been explicitly accepted

3.  **Implementation Process**

    -   Create a comprehensive todo list of all accepted changes
    -   Implement changes one at a time
    -   After each change, run tests to verify nothing broke
    -   If tests fail, stop and notify the user before proceeding
    -   Update the todo list as changes are completed
    -   **Document completion**: After successfully implementing and testing each change, add a note to the code review document indicating the change
        was made and tested, along with the date and time

4.  **Test Verification**

    -   Run `dotnet test` after each significant change
    -   All tests must pass before proceeding to next change
    -   If any tests fail, investigate and fix before continuing

5.  **Completion**

    -   Run final test verification to ensure all tests still pass
    -   Present summary of all changes implemented
    -   Include test results in completion report

## Key Principles

-   **Never assume**: Always wait for explicit user approval via feedback
-   **Test frequently**: Run tests after each change to catch issues early
-   **Stop on failure**: If tests fail, notify user immediately
-   **Track progress**: Use todo lists to show progress throughout implementation
-   **One change at a time**: Implement changes incrementally for better control
