# AI Instructions
Instructions that AI assistants follow when generating code, tests, documentation and other artifacts in this repository.


## Instruction Files
The instructions are kept in `.ai` and shared by all assistants:

| File | Content |
|------|---------|
| [coding-standards.instructions.md](/.ai/core/coding-standards.instructions.md) | Coding standard: headers, formatting, naming, exception handling, build and test commands. |
| [architecture.instructions.md](/.ai/core/architecture.instructions.md) | Domain-Driven Design and microservice boundaries. |
| [user-interface.instructions.md](/.ai/core/user-interface.instructions.md) | User interface rules, Luna Web data access and real-time data. |
| [testing-general.instructions.md](/.ai/testing/testing-general.instructions.md) | General testing rules. |
| [testing-unit.instructions.md](/.ai/testing/testing-unit.instructions.md) | Unit test rules. |
| [testing-integration.instructions.md](/.ai/testing/testing-integration.instructions.md) | Integration test rules. |


## Assistant Entry Points
Each assistant has an entry file that references the instruction files above:
- Claude Code: [.claude/CLAUDE.md](/.claude/CLAUDE.md)
- GitHub Copilot: [.github/copilot-instructions.md](/.github/copilot-instructions.md)

When an instruction file is added, reference it in both entry files.


## Repository Documentation
- [Development Overview](./Overview.md)
- [Repository README](/README.md)
