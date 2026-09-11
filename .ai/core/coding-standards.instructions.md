# Coding Standard Instructions

## Compiling Rules
- When writing or refactoring code, assume the environment is set to "Automatically build with confirmation".
- Ensure all code changes are clean and syntactically correct so compilation passes without triggering unnecessary build-error alerts in the IDE.


## Project Definitions
- **Nullability:** Nullable reference types are strictly enforced (`<Nullable>enable</Nullable>`).


## Copyright Headers
Add the header below at the beginning of all code files that end with `.cs`. Do not add it in generated files, all other file extensions or files called: MSTestSettings.cs.
```
// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
```


## Formatting
- **Spacing:** Always use two single blank lines after class member definitions, such as methods, fields, properties.
- **Newlines:** Always insert a single blank line immediately before `try`, `switch`, `if`, `using` statements, unless they are the very first line inside a code block.
- **XML Documentation Comments:** All public and internal classes, methods, properties, and fields must have XML documentation comments. 
    - Use <inheritdoc/> when documentation can be inherited from a base class or interface.    
    - XML documentation tags must use multi-line formatting. Opening and closing tags must be on separate lines, with the documentation text on its own line(s).
    - Exception: <inheritdoc/> is always a self-closing single-line tag and must not be wrapped in <summary> or formatted across multiple lines.
- **Return:** Always insert a single blank line immediately before a `return` command within a method if there is code before it and unless they are the very first line inside a code block.


## Variables & Conditions
- **Variable Declaration:** Never use the implicit `var` keyword for variables. Always use explicit types (e.g., `int x = 5;`, `string name = "";`).
- **Variable and Property Naming:** Use PascalCase for public events and properties, camelCase for local variables, camelCase with underscore for private variables (e.g. _camelCase).
- **Conditions:**
    - Do not write conditional statements and their return values on a single line.
    - Always wrap multi-line conditional blocks or returned values in parentheses or proper block syntax where applicable, ensuring clean line breaks.
- **Boolean Comparisons:** Avoid using the prefix `!` operator for negative boolean checks as it can be easily missed. Instead, use C# pattern matching or descriptive naming:
  - Use `is false` or `is not true` for explicit negative checks (e.g., `if (isValid is false)`). 
  - Prefer naming variables using positive phrasing so negative checks are rarely needed.
- **Data Lock:** when using a lock object for e.g. `lock(_dataLock) {...}` name it _dataLock.


## Exception handling
- **Exception Handling:** Do not use `if/else` blocks to manually validate arguments. Instead, always use the modern .NET static `ThrowIf` or `Throw` helper methods:
  - Use `ArgumentException.ThrowIfNullOrEmpty(...)` or `ArgumentException.ThrowIfNullOrWhiteSpace(...)`
  - Use `ArgumentNullException.ThrowIfNull(...)`
  - Use `ArgumentOutOfRangeException.ThrowIfLessThan(...)` or `ArgumentOutOfRangeException.ThrowIfGreaterThan(...)`

## Build & Test Commands
- **Build:** Use command `dotnet build` to build.
- **Test:** Use command `dotnet test` to run tests.