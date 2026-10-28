import { onBeforeUnmount, onMounted, ref, watch, type Ref } from 'vue';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { deviceHubUrl } from '../api/deviceApi';
import type { HubStatus } from './useSystemMeterSummary';
import type { PowerMeterDto, PowerMeterType } from '../types/der';

export interface PowerMeter {
    status: Ref<HubStatus>;
    /** The last pushed power meter, null if the DER configuration has no power meter of the type with the device ID. */
    powerMeter: Ref<PowerMeterDto | null>;
    /** False until the first values of the current power meter were received. */
    received: Ref<boolean>;
}

/** Hub method to subscribe to a power meter and the message with its values, by power meter type. */
const hubMethods: Record<PowerMeterType, { subscribe: string; updated: string }> = {
    PmSystemDto: { subscribe: 'SubscribeSystemPowerMeter', updated: 'SystemPowerMeterUpdated' },
    PmAuxiliaryDto: { subscribe: 'SubscribeAuxiliaryPowerMeter', updated: 'AuxiliaryPowerMeterUpdated' },
    PmExternalDto: { subscribe: 'SubscribeExternalPowerMeter', updated: 'ExternalPowerMeterUpdated' }
};

/**
 * Receives the values of one power meter from the device hub (Hub<IDeviceHubClient> on the server):
 *
 *   Subscribe{System|Auxiliary|External}PowerMeter(deviceId)                <- sent on connect and when the power meter changes
 *   {System|Auxiliary|External}PowerMeterUpdated(int deviceId, Pm*Dto?)     -> on subscribe, then every 2 seconds
 *
 * The hub is connected while the calling component is mounted, so the web service only publishes
 * the power meter while it is shown.
 */
export function usePowerMeter(meterType: Readonly<Ref<PowerMeterType>>, deviceId: Readonly<Ref<number>>): PowerMeter {
    const status = ref<HubStatus>('disconnected');
    const powerMeter = ref<PowerMeterDto | null>(null);
    const received = ref(false);

    let connection: HubConnection | null = null;
    let retryTimer: ReturnType<typeof setTimeout> | undefined;
    let isStopped = false;

    /** Subscribes the shown power meter - also after a reconnect, the server forgets the subscription of a lost connection. */
    async function subscribe(): Promise<void> {
        if (connection === null || connection.state !== HubConnectionState.Connected) {
            return;
        }

        try {
            await connection.invoke(hubMethods[meterType.value].subscribe, deviceId.value);
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

        for (const [type, methods] of Object.entries(hubMethods) as [PowerMeterType, { updated: string }][]) {
            hub.on(methods.updated, (id: number, value: PowerMeterDto | null) => {
                // Values of the previously shown power meter can still arrive right after the selection changed.
                if (type !== meterType.value || id !== deviceId.value) {
                    return;
                }

                powerMeter.value = value;
                received.value = true;
            });
        }

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

    watch([meterType, deviceId], () => {
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
