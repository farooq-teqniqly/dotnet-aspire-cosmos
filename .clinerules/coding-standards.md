---
description: Coding standards and best practices for .NET development
author: team
version: 1.0
globs: ["**/*"]
tags: ["coding-standards", "dotnet", "best-practices"]
---

# Coding Standards

## Project Structure

- **Organize by feature**: Group related files by feature/domain rather than by type
- **Consistent naming**: Use PascalCase for directories and file names
- **Layer separation**: Maintain clear separation between data access, business logic, and presentation layers

## Code Quality

- **SOLID principles**: Follow Single Responsibility, Open-Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion principles
- **DRY principle**: Don't Repeat Yourself - extract common code into reusable components
- **Clean code**: Write self-documenting code with meaningful names and clear structure

## C# Specific Guidelines

- **Async/await**: Use async/await for I/O operations and long-running tasks
- **Exception handling**: Use specific exception types and avoid catching generic Exception
- **Null safety**: Prefer nullable reference types and use null-coalescing operators appropriately
- **LINQ**: Use LINQ for data transformations when it improves readability

## Testing

- **Unit tests**: Write unit tests for all business logic
- **Test naming**: Use descriptive test method names following the pattern `MethodName_Scenario_ExpectedResult`
- **Test coverage**: Aim for high test coverage, especially for critical business logic

## Documentation

- **XML comments**: Document public APIs with XML comments
- **README files**: Maintain up-to-date README files for projects and major components
- **Code comments**: Use comments sparingly, preferring self-documenting code

## Dependencies

- **Minimal dependencies**: Only add dependencies that are necessary and well-maintained
- **Version pinning**: Use specific versions for dependencies to ensure reproducible builds
- **Security**: Regularly update dependencies to address security vulnerabilities

## Performance

- **Efficient algorithms**: Choose appropriate data structures and algorithms for the task
- **Resource management**: Properly dispose of resources using `using` statements or `IDisposable`
- **Async best practices**: Avoid blocking calls in async methods

## Project Creation

- **Web API projects**: When creating C# Web API projects, always use the following options:
  - `--use-program-main`: Use the new program style main entry point
  - `--use-controllers`: Generate controllers instead of minimal APIs
- **Project organization**: Place each new project in its own dedicated folder for better organization
