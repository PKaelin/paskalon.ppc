import { onBeforeUnmount, onMounted, ref, watch, type Ref } from 'vue';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { deviceHubUrl } from '../api/deviceApi';
import type { HubStatus } from './useSystemMeterSummary';
import type { PmSystemDto } from '../types/der';

export interface SystemPowerMeter {
    status: Ref<HubStatus>;
    /** The last pushed system power meter, null if the DER configuration has no system power meter with the device ID. */
    powerMeter: Ref<PmSystemDto | null>;
    /** False until the first values of the current device ID were received. */
    received: Ref<boolean>;
}

/**
 * Receives the values of one system power meter from the device hub (Hub<IDeviceHubClient> on the server):
 *
 *   SubscribeSystemPowerMeter(deviceId)                              <- sent on connect and when the device ID changes
 *   SystemPowerMeterUpdated(int deviceId, PmSystemDto? powerMeter)   -> on subscribe, then every 2 seconds
 *
 * The hub is connected while the calling component is mounted, so the web service only publishes
 * the system power meter while it is shown.
 */
export function useSystemPowerMeter(deviceId: Readonly<Ref<number>>): SystemPowerMeter {
    const status = ref<HubStatus>('disconnected');
    const powerMeter = ref<PmSystemDto | null>(null);
    const received = ref(false);

    let connection: HubConnection | null = null;
    let retryTimer: ReturnType<typeof setTimeout> | undefined;
    let isStopped = false;

    /** Subscribes the shown device - also after a reconnect, the server forgets the subscription of a lost connection. */
    async function subscribe(): Promise<void> {
        if (connection === null || connection.state !== HubConnectionState.Connected) {
            return;
        }

        try {
            await connection.invoke('SubscribeSystemPowerMeter', deviceId.value);
        } catch {
            // The connection was lost while subscribing, it subscribes again when it is reconnected.
        }
    }

    async function start(): Promise<void> {
        if (isStopped || (connection !== null && connection.state !== HubConnectionState.Disconnected)) {
            return;
        }

        const hub = new HubConnectionBuilder()
            .withUrl(deviceHubUrl())
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(import.meta.env.DEV ? LogLevel.Information : LogLevel.Warning)
            .build();

        hub.on('SystemPowerMeterUpdated', (id: number, value: PmSystemDto | null) => {
            // Values of the previously shown device can still arrive right after the device ID changed.
            if (id !== deviceId.value) {
                return;
            }

            powerMeter.value = value;
            received.value = true;
        });
        hub.onreconnecting(() => {
            status.value = 'reconnecting';
        });
        hub.onreconnected(() => {
            status.value = 'connected';
            void subscribe();
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
            await subscribe();
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

    watch(deviceId, () => {
        powerMeter.value = null;
        received.value = false;
        void subscribe();
    });

    onMounted(() => {
        void start();
    });

    onBeforeUnmount(() => {
        isStopped = true;
        clearTimeout(retryTimer);
        // Stopping the connection removes the subscription on the server.
        void connection?.stop();
        connection = null;
    });

    return { status, powerMeter, received };
}
