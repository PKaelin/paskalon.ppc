import { onBeforeUnmount, onMounted, ref, type Ref } from 'vue';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { alarmHubUrl } from '../api/alarmApi';
import type { AlarmSummaryDto } from '../types/alarm';

/**
 * Receives the alarm summary of the alarm hub (Hub<IAlarmClient> on the server):
 *
 *   AlarmSummaryUpdated(AlarmSummaryDto? summary)  -> when connected and every time a new alarm action changed it
 *
 * The returned summary is null while it is unknown: the hub is not connected, or the web service has not
 * received a summary from the alarm service yet.
 */
export function useAlarmSummary(): Ref<AlarmSummaryDto | null> {
    const summary = ref<AlarmSummaryDto | null>(null);

    let connection: HubConnection | null = null;
    let retryTimer: ReturnType<typeof setTimeout> | undefined;
    let isStopped = false;

    async function start(): Promise<void> {
        if (isStopped || (connection !== null && connection.state !== HubConnectionState.Disconnected)) {
            return;
        }

        const hub = new HubConnectionBuilder()
            .withUrl(alarmHubUrl())
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(import.meta.env.DEV ? LogLevel.Information : LogLevel.Warning)
            .build();

        hub.on('AlarmSummaryUpdated', (value: AlarmSummaryDto | null) => {
            summary.value = value;
        });
        // The hub sends the current summary again after reconnecting, until then the counts are unknown
        hub.onreconnecting(() => {
            summary.value = null;
        });
        hub.onclose(() => {
            summary.value = null;

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

    return summary;
}
