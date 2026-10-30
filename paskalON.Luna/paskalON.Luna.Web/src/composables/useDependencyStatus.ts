import { onBeforeUnmount, onMounted, ref, type Ref } from 'vue';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { dependencyHubUrl } from '../api/systemApi';
import type { DependencyStatusDto } from '../types/system';

/**
 * Receives the availability of the dependencies of the dependency hub (Hub<IDependencyClient> on the server):
 *
 *   DependencyStatusUpdated(IReadOnlyList<DependencyStatusDto> statuses)  -> periodically
 *
 * The returned statuses are empty while the hub is not connected, because the availability is unknown then.
 */
export function useDependencyStatus(): Ref<DependencyStatusDto[]> {
    const statuses = ref<DependencyStatusDto[]>([]);

    let connection: HubConnection | null = null;
    let retryTimer: ReturnType<typeof setTimeout> | undefined;
    let isStopped = false;

    async function start(): Promise<void> {
        if (isStopped || (connection !== null && connection.state !== HubConnectionState.Disconnected)) {
            return;
        }

        const hub = new HubConnectionBuilder()
            .withUrl(dependencyHubUrl())
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(import.meta.env.DEV ? LogLevel.Information : LogLevel.Warning)
            .build();

        hub.on('DependencyStatusUpdated', (value: DependencyStatusDto[]) => {
            statuses.value = value;
        });
        hub.onreconnecting(() => {
            statuses.value = [];
        });
        hub.onclose(() => {
            statuses.value = [];

            if (isStopped === false) {
                scheduleRetry();
            }
        });

        connection = hub;

        try {
            await hub.start();
        } catch {
            // withAutomaticReconnect only covers a connection that was established once,
            // so the very first attempt is retried here.
            scheduleRetry();
        }
    }

    function scheduleRetry(): void {
        clearTimeout(retryTimer);
        retryTimer = setTimeout(() => void start(), 5000);
    }

    onMounted(() => {
        void start();
    });

    onBeforeUnmount(() => {
        isStopped = true;
        clearTimeout(retryTimer);
        void connection?.stop();
        connection = null;
    });

    return statuses;
}
