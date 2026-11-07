#!/bin/sh
# -----------------------------------------------------------------------------
# Starts RabbitMQ and imports the PPC definitions (definitions.json) once the node is running.
#
# The definitions are not imported at node boot (definitions.local.path), because then RabbitMQ
# does not create the default user of ./secrets/rabbitmq_config on a new volume.
# The import is repeated at every start, it only adds missing exchanges, queues and bindings.
# -----------------------------------------------------------------------------

# The CLI tools must run as the rabbitmq user like the server,
# otherwise they create the Erlang cookie as root and the server cannot read it.
if [ "$(id -u)" = "0" ]; then
    AS_RABBITMQ="gosu rabbitmq"
else
    AS_RABBITMQ=""
fi

(
    # Wait for the server to create the Erlang cookie, then until the node is running
    until [ -f /var/lib/rabbitmq/.erlang.cookie ] && $AS_RABBITMQ rabbitmq-diagnostics -q check_running > /dev/null 2>&1; do
        sleep 2
    done

    if $AS_RABBITMQ rabbitmqctl -q import_definitions /etc/rabbitmq/definitions.json; then
        echo "Imported the PPC definitions from /etc/rabbitmq/definitions.json."
    else
        echo "Importing the PPC definitions from /etc/rabbitmq/definitions.json failed." >&2
    fi
) &

exec docker-entrypoint.sh rabbitmq-server
