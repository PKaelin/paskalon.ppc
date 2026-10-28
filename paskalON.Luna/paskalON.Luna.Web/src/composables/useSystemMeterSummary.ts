import { onBeforeUnmount, onMounted, ref, type Ref } from 'vue';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { systemHubUrl } from '../api/systemApi';
import type { SystemMeterSummaryDto } from '../types/system';

export type HubStatus = 'connecting' | 'connected' | 'reconnecting' | 'disconnected';

export interface SystemMeterSummary {
    status: Ref<HubStatus>;
    /** The last pushed summary, null if no system power meter delivers data. */
    summary: Ref<SystemMeterSummaryDto | null>;
    /** False until the first summary was received. */
    received: Ref<boolean>;
}

/**
 * Receives the system meter summary of the system hub (Hub<ISystemClient> on the server):
 *
 *   SystemMeterSummaryUpdated(SystemMeterSummaryDto? summary)  -> every 2 seconds, null without system power meter
 *
 * The hub is connected while the calling component is mounted, so the web service only publishes
 * while the dashboard is shown.
 */
export function useSystemMeterSummary(): SystemMeterSummary {
    const status = ref<HubStatus>('disconnected');
    const summary = ref<SystemMeterSummaryDto | null>(null);
    const received = ref(false);

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

        hub.on('SystemMeterSummaryUpdated', (value: SystemMeterSummaryDto | null) => {
            summary.value = value;
            received.value = true;
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

    return { status, summary, received };
}
