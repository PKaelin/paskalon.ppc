import { onBeforeUnmount, onMounted, ref, type Ref } from 'vue';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { systemHubUrl } from '../api/systemApi';
import type { CurrentSetpointsDto } from '../types/operatingModes';
import type { HubStatus } from './useSystemMeterSummary';

export interface SetpointStatus {
    status: Ref<HubStatus>;
    /** The last pushed setpoints, null until the first ones were received. */
    setpoints: Ref<CurrentSetpointsDto | null>;
}

/**
 * Receives the applied setpoints of the system hub (Hub<ISystemClient> on the server):
 *
 *   SetpointsUpdated(CurrentSetpointsDto setpoints)  -> every 2 seconds
 *
 * The hub is connected while the calling component is mounted, so the web service only publishes
 * while the setpoints are shown.
 */
export function useSetpointStatus(): SetpointStatus {
    const status = ref<HubStatus>('disconnected');
    const setpoints = ref<CurrentSetpointsDto | null>(null);

    let connection: HubConnection | null = null;
    let retryTimer: ReturnType<typeof setTimeout> | undefined;
    let isStopped = false;

    async function start(): Promise<void> {
        if (isStopped || (connection !== null && connection.state !== HubConnectionState.Disconnected)) {
            return;
        }

        const hub = new HubConnectionBuilder()
            .withUrl(systemHubUrl())
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(import.meta.env.DEV ? LogLevel.Information : LogLevel.Warning)
            .build();

        hub.on('SetpointsUpdated', (value: CurrentSetpointsDto) => {
            setpoints.value = value;
        });
        hub.onreconnecting(() => {
            status.value = 'reconnecting';
        });
        hub.onreconnected(() => {
            status.value = 'connected';
        });
        hub.onclose(() => {
            status.value = 'disconnected';

            if (isStopped === false) {
                scheduleRetry();
            }
        });

        connection = hub;
        status.value = 'connecting';

        try {
            await hub.start();
            status.value = 'connected';
        } catch {
            status.value = 'disconnected';
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

    return { status, setpoints };
}
