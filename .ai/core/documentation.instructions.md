# Documentation Instructions


## General Guidelines
- **Definition:** The documentation structure is defined in `Docs/Documentation.md`. Read it before adding, moving or changing documentation.
- **Docs change with the code:** Update the documentation in the same change as the code it describes. A change is not complete while its documentation is outdated.
- **Ownership decides location:** Content that spans multiple solutions goes to `/Docs`, content owned by one solution goes to that solution's `Docs` folder. Projects have no `Docs` folder of their own.
- **Link, do not duplicate:** Describe a fact in one place and link to it from everywhere else.
- **Text:** Use short, factual English. Document what the code does, never guess: verify names, defaults, routes and topics in the code.
- **Files:** Markdown files use LF line endings and no BOM.


## What To Update
| Code change | Documentation to update |
|-------------|-------------------------|
| Web API endpoint added, changed or removed | Solution `Docs/Interfaces.md` and the REST Client file (`*.http`) of the service project. |
| Published or subscribed topic, exchange or message changed | Solution `Docs/Interfaces.md` and, for configured names, `Docs/Configuration.md`. |
| Environment variable, secret or system configuration property changed | Solution `Docs/Configuration.md`. |
| Project or service dependency added or removed | Solution `Docs/Dependencies.md` and the `Docs/Dependencies.md` of the used solution or library. |
| Domain entity, classification or state changed | Solution `Docs/DomainModel.md`. |
| Functional or technical behavior of one solution changed | The matching page in the solution `Docs/Design` folder, listed in `Docs/Design/Overview.md`. |
| Behavior across solutions changed (e.g. failover, dependency health) | The matching page in `/Docs/03-Design`, listed in `/Docs/03-Design/Overview.md`. |
| Project added, renamed or removed | Project `README.md` and the "Structure" section of the solution `README.md`. |
| Database port, database name or migration process changed | `/Docs/04-Develop/DatabaseMigrations.md` and `InfrastructureInternal/README.md`. |
| AI instruction file added | `.claude/CLAUDE.md`, `.github/copilot-instructions.md` and `/Docs/04-Develop/AiInstructions.md`. |


## New Project README
Every new `.csproj` gets a `README.md` in its folder with:
- Title: the project name and a short description of its purpose.
- A link to the solution README: `Part of the [<Solution> solution](../README.md).`
- For `*.UnitTest` projects a section "Unit Test", for `*.IntegrationTest` projects a section "Integration Test" with the configuration and running requirements (e.g. Docker for Testcontainers).
- The section "Repository Documentation" with the links defined in `Docs/Documentation.md`.


## Diagrams And Images
- Store diagrams of a solution or project in the solution's `Docs/Assets/Diagrams` folder and repository-wide diagrams in `/Docs/Assets/Diagrams`.
- Link images relative to the Markdown file (e.g. `../Assets/Diagrams/Name.drawio.svg`) and encode spaces as `%20`.


## Check
- Run the documentation check before finishing a change: `powershell -NoProfile -File .github/scripts/Test-Documentation.ps1`
- The check also runs in the GitHub workflow `Documentation` for pushes and pull requests on the main branch.
