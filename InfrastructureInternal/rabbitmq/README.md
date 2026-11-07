# RabbitMQ definitions
Exchanges, queues and bindings of the PPC that exist independent of the running services.

- `definitions.json`: the definitions (RabbitMQ definitions export format, without users and permissions).
- `import-definitions.sh`: entrypoint of the RabbitMQ container, starts RabbitMQ and imports `definitions.json` once the node is running.

## Why not import at node boot
With `definitions.local.path` RabbitMQ skips creating the default user of `./secrets/rabbitmq_config` on a new volume
("Will not seed default virtual host and user: have definitions to load"), so no service could log in.
The import after the start keeps the default user and only adds missing definitions, it is repeated at every start.

## Definitions
| Exchange | Type | Queue | Routing key | Consumer |
|----------|------|-------|-------------|----------|
| ppc:alarms:actions | topic (durable) | ppc:luna:alarm-actions (quorum, durable) | ShowAlarm | Luna |

- The alarm service publishes the alarm actions mandatory: an action without a bound queue is kept in its outbox and retried.
- A new consumer of alarm actions gets its own queue bound with the action names (routing keys) it executes.
- The import does not change or delete existing definitions: to change the type or arguments of an existing queue, delete it first (e.g. in the management UI).
