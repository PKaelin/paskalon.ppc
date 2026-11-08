# RabbitMQ definitions
Exchanges, queues and bindings of the PPC that exist independent of the running services.

- `definitions.json`: the definitions (RabbitMQ definitions export format, without users and permissions).
- `import-definitions.sh`: entrypoint of the RabbitMQ container, starts RabbitMQ and imports `definitions.json` once the node is running.

## Why not import at node boot
With `definitions.local.path` RabbitMQ skips creating the default user of `./secrets/rabbitmq_config` on a new volume
("Will not seed default virtual host and user: have definitions to load"), so no service could log in.
The import after the start keeps the default user and only adds missing definitions, it is repeated at every start.

## Definitions
| Exchange | Type | Publisher | Subscribers |
|----------|------|-----------|-------------|
| ppc:alarms:actions | topic (durable) | Alarm service (routing key = action name) | Luna: every instance with its own temporary queue bound with RaiseAlarm and ClearAlarm |

- The alarm service publishes the alarm actions mandatory: an action without a bound queue (e.g. no Luna instance running)
  is kept in its outbox and retried.
- Subscribers that run as several instances where every instance needs every message (e.g. Luna) create their own
  server named, exclusive and auto delete queue at startup (`RabbitMqSubscriber`), they are not defined here.
- A subscriber that must not lose messages while it is not running needs a durable queue, define it here.
- The import does not change or delete existing definitions: to change the type or arguments of an existing queue,
  or to remove a queue that is no longer used, delete it (e.g. in the management UI).
