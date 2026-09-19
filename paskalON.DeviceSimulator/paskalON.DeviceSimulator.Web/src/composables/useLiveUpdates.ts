import { onBeforeUnmount, ref, type Ref } from 'vue';
import {
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState,
    LogLevel
} from '@microsoft/signalr';
import { config } from '@/api/config';
import { hubUrl } from '@/api/derApi';
import type { DerDto, DerUnitDto } from '@/types/der';
import { applyDerUpdate, applyDeviceUpdate, applyUnitUpdate, type DeviceUpdate } from '@/utils/derMerge';

export type LiveStatus = 'disabled' | 'connecting' | 'connected' | 'reconnecting' | 'disconnected';

/**
 * Hub contract expected from the web service (Hub<IDerClient> on the server):
 *
 *   UnitUpdated(DerUnitDto unit)          -> one complete unit incl. its devices
 *   DeviceUpdated(DeviceUpdate device)    -> one device: { deviceId, name, endpoints[] }
 *   DerUpdated(DerDto der)                -> the complete DER (fallback / bulk update)
 *
 * The client identifies a unit by its Name and a device by DeviceId + Name, exactly as
 * agreed. Unknown units/devices trigger a full reload, because that means the
 * configuration itself changed.
 */
export interface LiveUpdateOptions {
    /** The loaded model that gets patched. */
    der: Ref<DerDto | null>;
    /** Called when an update refers to something that is not part of the loaded model. */
    onStructureChanged: () => void;
    /** Optional: called for every applied update (e.g. to show a "last update" time). */
    onApplied?: () => void;
}

export interface LiveUpdates {
    status: Ref<LiveStatus>;
    lastMessageAt: Ref<Date | null>;
    messageCount: Ref<number>;
    start: () => Promise<void>;
    stop: () => Promise<void>;
}

export function useLiveUpdates(options: LiveUpdateOptions): LiveUpdates {
    const status = ref<LiveStatus>(config().enableSignalR ? 'disconnected' : 'disabled');
    const lastMessageAt = ref<Date | null>(null);
    const messageCount = ref(0);

    let connection: HubConnection | null = null;

    function applied(success: boolean): void {
        messageCount.value++;
        lastMessageAt.value = new Date();

        if (!success) {
            // The update referenced an unknown unit/device -> reload the configuration.
            options.onStructureChanged();
            return;
        }

        options.onApplied?.();
    }

    function register(hub: HubConnection): void {
        hub.on('UnitUpdated', (unit: DerUnitDto) => {
            const der = options.der.value;
            applied(der ? applyUnitUpdate(der, unit) : false);
        });

        hub.on('DeviceUpdated', (device: DeviceUpdate) => {
            const der = options.der.value;
            applied(der ? applyDeviceUpdate(der, device) : false);
        });

        hub.on('DerUpdated', (updated: DerDto) => {
            const der = options.der.value;
            applied(der ? applyDerUpdate(der, updated) : false);
        });

        // Some services push a batch instead of single messages.
        hub.on('UnitsUpdated', (units: DerUnitDto[]) => {
            const der = options.der.value;
            const success = der ? (units ?? []).every((unit) => applyUnitUpdate(der, unit)) : false;
            applied(success);
        });

        hub.on('DevicesUpdated', (devices: DeviceUpdate[]) => {
            const der = options.der.value;
            const success = der
                ? (devices ?? []).every((device) => applyDeviceUpdate(der, device))
                : false;
            applied(success);
        });

        hub.onreconnecting(() => {
            status.value = 'reconnecting';
        });

        hub.onreconnected(() => {
            status.value = 'connected';
            // The service may have lost the subscription set - the caller re-posts it.
            options.onStructureChanged();
        });

        hub.onclose(() => {
            status.value = 'disconnected';
        });
    }

    async function start(): Promise<void> {
        if (!config().enableSignalR) {
            status.value = 'disabled';
            return;
        }

        if (connection && connection.state !== HubConnectionState.Disconnected) {
            return;
        }

        connection = new HubConnectionBuilder()
            .withUrl(hubUrl())
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(import.meta.env.DEV ? LogLevel.Information : LogLevel.Warning)
            .build();

        register(connection);
        status.value = 'connecting';

        try {
            await connection.start();
            status.value = 'connected';
        } catch {
            status.value = 'disconnected';
            // withAutomaticReconnect only covers a connection that was established once,
            // so retry the very first attempt here.
            setTimeout(() => void start(), 5000);
        }
    }

    async function stop(): Promise<void> {
        if (!connection) {
            return;
        }

        try {
            await connection.stop();
        } finally {
            connection = null;
            status.value = config().enableSignalR ? 'disconnected' : 'disabled';
        }
    }

    onBeforeUnmount(() => {
        void stop();
    });

    return { status, lastMessageAt, messageCount, start, stop };
}
