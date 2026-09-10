# General Testing Instructions


## Technology & Conventions
- Use MSTest by Microsoft.
- Do not use fluent API for asserts.
- Ensure resources are disposed after use.


## Test File Structure & Naming
- Tests must be deterministic.
- Test class and method names shall not contain any underscores.    
- Every test must strictly follow the **Arrange-Act-Assert** pattern separated by single blank line.
- Tests must not depend on execution order.
- Use async tests with `Task` rather than blocking on async code.
- Never use `.Result`, `.Wait()`, `Thread.Sleep()` for synchronization.
- Use data-driven testing (Data Rows) where applicable instead of writing separate test methods.
- Use realistic, fully instantiated test data in the Arrange phase and do not just use simple primitives or empty mock objects if the system behavior depends on real data structures.
- Use the short form of types (e.g., `IPEndPoint`) and ensure the necessary using directive (e.g., `using System.Net;`) is added at the top of the file namespace.
