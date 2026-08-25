# Copilot Instructions

## Project Guidelines
- For MSTest unit tests, use Moq without fluent-style APIs; test method names must start with the class name, end with Test, and contain no underscores; test class names must be the class name ending in Test; use NullLogger.Instance when logging is not under test and FakeLogger<T> when it is.; Always use two CR/LF after class member definitions like methods and properties.