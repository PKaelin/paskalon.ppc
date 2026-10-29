<script setup lang="ts">
import { computed, ref } from 'vue';
import ActionDialog, { type DialogAction } from './ActionDialog.vue';
import { connectBatteryBank, disconnectBatteryBank, setMaintenance, standbyPcs, startPcs, stopPcs } from '../api/deviceApi';
import { useNotifications } from '../composables/useNotifications';
import { BatteryStorageUnitBatteryBankStateNames, PcsStateNames, type DerBatteryStorageUnitDto } from '../types/der';
import { formatEnum } from '../utils/deviceValues';

/** A control of the battery storage unit, also the control that is busy sending a command. */
type Control = 'maintenance' | 'pcs' | 'batteryBanks';

const props = defineProps<{
    /** Name of the battery storage unit. */
    unitName: string;
    /** The last pushed battery storage unit, null until it was received or if it is no longer in the DER configuration. */
    unit: DerBatteryStorageUnitDto | null;
}>();

const { notify } = useNotifications();

const pcsActions: DialogAction[] = [
    { id: 'start', label: 'Start PCS', variant: 'success' },
    { id: 'stop', label: 'Stop PCS', variant: 'danger' },
    { id: 'standby', label: 'Standby PCS', variant: 'secondary' }
];

const batteryBankActions: DialogAction[] = [
    { id: 'connect', label: 'Connect All BB', variant: 'success' },
    { id: 'disconnect', label: 'Disconnect All BB', variant: 'danger' }
];

/** The control sending a command, null if none - one command at a time. */
const busy = ref<Control | null>(null);

const isPcsDialogOpen = ref(false);

const isBatteryBankDialogOpen = ref(false);

const isUnavailable = computed(() => props.unit === null || busy.value !== null);

const maintenanceMode = computed(() => {
    if (props.unit === null) {
        return '—';
    }

    return props.unit.isInMaintenanceMode ? 'On' : 'Off';
});

const pcsState = computed(() => (props.unit === null ? '—' : formatEnum(props.unit.pcsState, PcsStateNames)));

const batteryBankState = computed(() => (props.unit === null ? '—' : formatEnum(props.unit.batteryBankState, BatteryStorageUnitBatteryBankStateNames)));

/** Sends a command of a control and notifies the outcome. */
async function send(control: Control, success: string, command: () => Promise<void>): Promise<void> {
    busy.value = control;

    try {
        await command();
        notify(success, 'success', 4000);
    } catch (error) {
        notify(error instanceof Error ? error.message : String(error), 'danger', 0);
    } finally {
        busy.value = null;
    }
}

function changeMaintenanceMode(): void {
    if (props.unit === null) {
        return;
    }

    const intoMaintenance = props.unit.isInMaintenanceMode === false;
    const success = intoMaintenance ? `'${props.unitName}' put into maintenance.` : `'${props.unitName}' taken out of maintenance.`;

    void send('maintenance', success, () => setMaintenance(props.unitName, intoMaintenance));
}

function changeMaintenanceTargets(): void {
    notify('Not implemented yet', 'info', 4000);
}

function onPcsAction(action: string): void {
    const pcs = props.unit?.powerConversionSystem;

    if (pcs === undefined) {
        return;
    }

    switch (action) {
        case 'start':
            void send('pcs', `Start command sent to '${pcs.name}'.`, () => startPcs(pcs.deviceId));
            break;
        case 'stop':
            void send('pcs', `Stop command sent to '${pcs.name}'.`, () => stopPcs(pcs.deviceId));
            break;
        case 'standby':
            void send('pcs', `Standby command sent to '${pcs.name}'.`, () => standbyPcs(pcs.deviceId));
            break;
    }
}

/** Sends the command to every battery bank of the unit, a failing battery bank does not stop the others. */
async function sendToAllBatteryBanks(command: (deviceId: number) => Promise<void>): Promise<void> {
    const batteryBanks = props.unit?.batteryBanks ?? [];
    const results = await Promise.allSettled(batteryBanks.map((batteryBank) => command(batteryBank.deviceId)));
    const failed = batteryBanks.filter((_, index) => results[index].status === 'rejected');

    if (failed.length > 0) {
        throw new Error(`The command failed for ${failed.map((batteryBank) => `'${batteryBank.name}'`).join(', ')}.`);
    }
}

function onBatteryBankAction(action: string): void {
    const count = props.unit?.batteryBanks.length ?? 0;

    if (count === 0) {
        notify(`'${props.unitName}' has no battery bank.`, 'warning', 4000);

        return;
    }

    if (action === 'connect') {
        void send('batteryBanks', `Connect command sent to ${count} battery bank(s).`, () => sendToAllBatteryBanks(connectBatteryBank));
    } else if (action === 'disconnect') {
        void send('batteryBanks', `Disconnect command sent to ${count} battery bank(s).`, () => sendToAllBatteryBanks(disconnectBatteryBank));
    }
}
</script>

<template>
    <section class="unit-controls-section" aria-labelledby="unit-controls-title">
        <div class="dash-section-header">
            <h2 id="unit-controls-title" class="dash-section-title">Controls</h2>
        </div>

        <div class="unit-controls">
            <div class="unit-control">
                <span class="unit-control-label">Maintenance Mode</span>
                <span class="unit-control-value">{{ maintenanceMode }}</span>
                <button class="btn btn-sm btn-outline-light" type="button" :disabled="isUnavailable" @click="changeMaintenanceMode">
                    <span v-if="busy === 'maintenance'" class="spinner-border spinner-inline me-1" role="status" aria-hidden="true"></span>
                    Change
                </button>
            </div>

            <div class="unit-control">
                <span class="unit-control-label">Maintenance Targets</span>
                <span class="unit-control-value">Disabled</span>
                <button class="btn btn-sm btn-outline-light" type="button" @click="changeMaintenanceTargets">Change</button>
            </div>

            <div class="unit-control">
                <span class="unit-control-label">PCS State</span>
                <span class="unit-control-value">{{ pcsState }}</span>
                <button class="btn btn-sm btn-outline-light" type="button" :disabled="isUnavailable" @click="isPcsDialogOpen = true">
                    <span v-if="busy === 'pcs'" class="spinner-border spinner-inline me-1" role="status" aria-hidden="true"></span>
                    Change
                </button>
            </div>

            <div class="unit-control">
                <span class="unit-control-label">Battery Bank State</span>
                <span class="unit-control-value">{{ batteryBankState }}</span>
                <button class="btn btn-sm btn-outline-light" type="button" :disabled="isUnavailable" @click="isBatteryBankDialogOpen = true">
                    <span v-if="busy === 'batteryBanks'" class="spinner-border spinner-inline me-1" role="status" aria-hidden="true"></span>
                    Change
                </button>
            </div>
        </div>

        <ActionDialog v-model="isPcsDialogOpen" :title="`PCS of ${unitName}`" :actions="pcsActions" @choose="onPcsAction" />
        <ActionDialog v-model="isBatteryBankDialogOpen" :title="`Battery banks of ${unitName}`" :actions="batteryBankActions" @choose="onBatteryBankAction" />
    </section>
</template>
