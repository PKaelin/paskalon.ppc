import { onBeforeUnmount, onMounted, ref, shallowRef, watch, type Ref, type ShallowRef } from 'vue';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { deviceHubUrl } from '../api/deviceApi';
import type { HubStatus } from './useSystemMeterSummary';
import type { PowerMeterType } from '../types/der';

/** The device DTO types the device hub publishes. */
export type SubscribableDeviceType = PowerMeterType | 'PcsDto';

export interface DeviceSubscription<T> {
    status: Ref<HubStatus>;
    /** The last pushed device, null if the DER configuration has no device of the type with the device ID. */
    device: ShallowRef<T | null>;
    /** False until the first values of the current device were received. */
    received: Ref<boolean>;
}

/** Hub method to subscribe to a device and the message with its values, by device type. */
const hubMethods: Record<SubscribableDeviceType, { subscribe: string; updated: string }> = {
    PmSystemDto: { subscribe: 'SubscribeSystemPowerMeter', updated: 'SystemPowerMeterUpdated' },
    PmAuxiliaryDto: { subscribe: 'SubscribeAuxiliaryPowerMeter', updated: 'AuxiliaryPowerMeterUpdated' },
    PmExternalDto: { subscribe: 'SubscribeExternalPowerMeter', updated: 'ExternalPowerMeterUpdated' },
    PcsDto: { subscribe: 'SubscribePcs', updated: 'PcsUpdated' }
};

/**
 * Receives the values of one device from the device hub (Hub<IDeviceHubClient> on the server):
 *
 *   Subscribe{SystemPowerMeter|AuxiliaryPowerMeter|ExternalPowerMeter|Pcs}(deviceId)   <- on connect and when the device changes
 *   {SystemPowerMeter|AuxiliaryPowerMeter|ExternalPowerMeter|Pcs}Updated(deviceId, dto) -> on subscribe, then every 2 seconds
 *
 * The hub is connected while the calling component is mounted, so the web service only publishes
 * the device while it is shown.
 */
export function useDeviceSubscription<T>(deviceType: Readonly<Ref<SubscribableDeviceType>>, deviceId: Readonly<Ref<number>>): DeviceSubscription<T> {
    const status = ref<HubStatus>('disconnected');
    const device = shallowRef<T | null>(null);
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
            await connection.invoke(hubMethods[deviceType.value].subscribe, deviceId.value);
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

        for (const [type, methods] of Object.entries(hubMethods) as [SubscribableDeviceType, { updated: string }][]) {
            hub.on(methods.updated, (id: number, value: T | null) => {
                // Values of the previously shown device can still arrive right after the selection changed.
                if (type !== deviceType.value || id !== deviceId.value) {
                    return;
                }

                device.value = value;
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

    watch([deviceType, deviceId], () => {
        device.value = null;
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

    return { status, device, received };
}
