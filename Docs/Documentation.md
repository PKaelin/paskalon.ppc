# Documentation Structure And Definition
Defines where documentation lives in this repository, what each part contains and the rules for keeping it current. Read this before adding or moving documentation.


## Principles
- **Ownership decides location:** content that spans multiple solutions lives at repository level (`/Docs`); content owned by one solution lives in that solution's `Docs` folder, next to its code.
- **A solution** is one independently deployable microservice folder (e.g. `paskalON.OperatingModes`) including its domain, application, infrastructure, service and test projects.
- **Every folder has an `Overview.md`** as its entry point.
- **Docs change with the code:** documentation is updated in the same pull request as the change it describes.


## Repository Root
The repository root contains the following structure:
```
README.md                               -> What is in the repository.
|__ Docs/                               -> Base folder for all documentation that spans multiple solutions, including architecture, design, development, test, etc.
|      |__01-Define/
|         |__Overview.md                -> Links or content of functional, non-functional, quality, availability, security, and other requirements of the solution.
|      |__02-Architect/                 -> Architecture describes the big picture and the relationships between major building blocks.
|      |  |__Overview.md                -> Overview of the solution referencing context, landscape, deployment and security.
|      |  |__SystemContext.md           -> Links or content of the context of the solution, including external systems and actors.
|      |  |__SolutionLandscape.md       -> Links or content of the landscape of the solutions, including internal systems and actors.
|      |  |__SystemDeployment.md        -> Links or content of the deployment details of the solution.
|      |  |__SystemSecurity.md          -> Links or content of the security considerations of the solution.
|      |__03-Design/                    -> Designs spanning multiple solutions (Cross-service workflows, system-wide failover, end-to-end control flow).
|      |  |__Overview.md                -> Links or content of the designs of the solution.
|      |  |__MetricPublisher.md         -> E.g. the metrics publisher
|      |  |__MessageSequencing.md       -> E.g. the message sequencing
|      |  |__DependencyHeartbeat.md     -> E.g. the dependency heartbeat
|      |  |__etc.
|      |__04-Develop/
|      |  |__Overview.md                -> Overview of the development process, including links to standards, guidelines, repo structure, code review, packaging, versioning, etc.
|      |  |__DevelopmentEnvironment.md  -> How to set up the development environment, including IDEs, tools, and dependencies.
|      |  |__AiInstructions.md          -> AI instructions for generating code, documentation, and other artifacts.
|      |__05-Test/
|      |  |__Overview.md                -> Overview of the testing process, including links to standards, guidelines, unit, integration, performance, HIL, etc.
|      |__06-Release/
|      |  |__Overview.md                -> Overview of the release process, including links to standards, guidelines, strategy, checklists, notes etc.
|      |__ Manual/                      -> End-user manual, versioned in subfolders (V0.1, V1.0, etc.)
|      |   |__Overview.md
|      |   |__V0.1/                     -> End-user manual, versioned in subfolders (V0.1, V1.0, etc.)
|      |   |__V1.0/
|      |__ Marketing/                   -> Moving out of the engineering repo later
|      |   |__V0.1/                     -> Product Marketing, versioned in subfolders (V0.1, V1.0, etc.)
|      |   |__V1.0/
|      |__Assets/                       -> Assets for the root documentation, like diagrams, images, logos, icons, etc.
|      |     |__Diagrams/               -> Dont put solutions diagrams here, put them in the solution Doc folder.
|      |     |__Images/
|      |     |__Logos/
|      |     |__Icons/
```


## Solution Root
The solution root contains the following structure:
```
README.md                               -> Features, Solution Structure, Link to solutions Overview.md, Links to main documentation.
|__ Docs/                               -> Base folder for all documentation that is owned by this solution, including definitions, designs, etc.
|     |__ Overview.md                   -> Purpose, scope, responsibilities, and boundaries of the solution
|     │__ Dependencies.md               -> Links or content of internal and external dependencies including other solutions, services, and systems.
│     |__ Interfaces.md                 -> Links or content of APIs, messaging contracts, events, and interfaces exposed to or consumed by the solution.
│     |__ DomainModel.md                -> Links or content of the domain model for the solution.
│     |__ Configuration.md              -> Links or content of configuration settings and parameters for the solution.
|     |__Design/                        -> How functionality within this solution works (Power allocation, operating mode evaluation, ramping).
|     |  |__Overview.md                 -> Overview of the solution's important functional and technical designs.
|     |  |__DeviceState.md              -> E.g. the device states like a PCS device.
|     |  |__PowerAllocation.md          -> E.g. the power allocator.
|     |  |__etc.
|     |__Assets/
|        |__Diagrams/
|        |__Images/
```

### Solution Root Documentation Requirements
Every README.md file in the solution root should contain a link to the solution's ./Docs/Overview.md file.
Every ./Docs/Overview.md file should contain a link to the solution's README.md file.

Every README.md and ./Docs/Overview.md file should contain a section called "Repository Documentation" that contains the following links to the root repository:
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)

