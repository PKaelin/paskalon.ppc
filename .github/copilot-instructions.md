# Copilot Instructions

## Project Guidelines
- **Spacing:** Always use two carriage returns/line feeds (CR/LF) after class member definitions, such as methods and properties.
- **Variable Declaration:** Never use the implicit `var` keyword for variables; always use explicit types (e.g., `int x = 5;`, `string name = "";`).
- **Exception Handling:** Do not use `if/else` blocks to manually validate arguments. Instead, always use the modern .NET static `ThrowIf` or `Throw` helper methods
  - Use `ArgumentException.ThrowIfNullOrEmpty(...)` or `ArgumentException.ThrowIfNullOrWhiteSpace(...)`
  - Use `ArgumentNullException.ThrowIfNull(...)`
  - Use `ArgumentOutOfRangeException.ThrowIfLessThan(...)` or `ArgumentOutOfRangeException.ThrowIfGreaterThan(...)`
- **Boolean Comparisons:** Avoid using the prefix `!` operator for negative boolean checks as it can be easily missed. Instead, use C# pattern matching or descriptive naming:
  - Use `is false` or `is not true` for explicit negative checks (e.g., `if (isValid is false)`). 
  - Prefer naming variables using positive phrasing so negative checks are rarely needed.
- **XML Documentation Comments**: All public and internal classes, methods, properties, fields must have XML documentation comments. 
    - Use <inheritdoc/> when documentation can be inherited from a base class or interface.    
    - XML documentation tags must use multi-line formatting. Opening and closing tags must be on separate lines, with the documentation text on its own line(s).
    - Exception: <inheritdoc/> is always a self-closing single-line tag and must not be wrapped in <summary> or formatted across multiple lines.
- **Unit Tests:** Use MSTest, use Moq without fluent-style APIs and following conventions:
    - Test class names must begin with the name of the class that is tested and end with Test (e.g. class Device results in unit test class DeviceTest).
    - Test method names shall start with the class name, followed by description what it tests and end with Test (e.g. class Device method Run results in unit test Run[Description]Test).
    - Test class and method names shall not contain any underscores.
    - Use `Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance` for simple ILoggers and `Microsoft.Extensions.Logging.Testing.FakeLogger<T>` when testing of logging makes sense.
