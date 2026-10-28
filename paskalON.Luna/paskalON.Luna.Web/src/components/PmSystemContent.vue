<script setup lang="ts">
import { computed, toRef } from 'vue';
import { useSystemPowerMeter } from '../composables/useSystemPowerMeter';
import type { HubStatus } from '../composables/useSystemMeterSummary';
import { PowerMeterStateNames } from '../types/der';

/** A name and the formatted value of a property. */
interface PropertyRow {
    name: string;
    value: string;
}

const Mega = 1_000_000;

const Kilo = 1_000;

/** Text of a property without value. */
const NoValue = 'None';

const props = defineProps<{
    /** Device ID of the system power meter. */
    deviceId: number;
}>();

const { status, powerMeter, received } = useSystemPowerMeter(toRef(() => props.deviceId));

const formatter = new Intl.NumberFormat(undefined, { minimumFractionDigits: 4, maximumFractionDigits: 4 });

/** Formats a value in the unit (the value divided by the divisor) with 4 decimals, null is shown as "None". */
function quantity(value: number | null | undefined, divisor: number, unit: string): string {
    if (value === null || value === undefined) {
        return NoValue;
    }

    const rounded = Number((value / divisor).toFixed(4));
    const text = formatter.format(rounded === 0 ? 0 : rounded);

    return unit === '' ? text : `${text} ${unit}`;
}

const core = computed(() => powerMeter.value?.core ?? null);

const detail = computed(() => powerMeter.value?.detail ?? null);

const state = computed(() => {
    const value = core.value?.state;

    if (value === null || value === undefined) {
        return NoValue;
    }

    return typeof value === 'string' ? value : PowerMeterStateNames[value] ?? String(value);
});

const communicationError = computed(() => {
    const value = core.value?.communicationError;

    if (value === null || value === undefined) {
        return NoValue;
    }

    return value ? 'True' : 'False';
});

const leftRows = computed<PropertyRow[]>(() => [
    { name: 'Active Power', value: quantity(core.value?.activePower?.watts, Mega, 'MW') },
    { name: 'Reactive Power', value: quantity(core.value?.reactivePower?.voltAmperesReactive, Mega, 'MVAr') },
    { name: 'Apparent Power', value: quantity(core.value?.apparentPower?.voltAmperes, Mega, 'MVA') },
    { name: 'Power Factor', value: quantity(core.value?.powerFactor, 1, '') },
    { name: 'Frequency', value: quantity(core.value?.frequency, 1, 'Hz') },
    { name: 'Energy Delivered', value: quantity(detail.value?.energyDelivered?.wattHours, Mega, 'MWh') },
    { name: 'Reactive Energy Delivered', value: quantity(detail.value?.reactiveEnergyDelivered?.voltAmperesReactiveHours, Mega, 'MVArh') },
    { name: 'Energy Received', value: quantity(detail.value?.energyReceived?.wattHours, Mega, 'MWh') },
    { name: 'Reactive Energy Received', value: quantity(detail.value?.reactiveEnergyReceived?.voltAmperesReactiveHours, Mega, 'MVArh') }
]);

const rightRows = computed<PropertyRow[]>(() => [
    { name: 'Active Power A', value: quantity(detail.value?.activePowerA?.watts, Mega, 'MW') },
    { name: 'Active Power B', value: quantity(detail.value?.activePowerB?.watts, Mega, 'MW') },
    { name: 'Active Power C', value: quantity(detail.value?.activePowerC?.watts, Mega, 'MW') },
    { name: 'Reactive Power A', value: quantity(detail.value?.reactivePowerA?.voltAmperesReactive, Mega, 'MVAr') },
    { name: 'Reactive Power B', value: quantity(detail.value?.reactivePowerB?.voltAmperesReactive, Mega, 'MVAr') },
    { name: 'Reactive Power C', value: quantity(detail.value?.reactivePowerC?.voltAmperesReactive, Mega, 'MVAr') },
    { name: 'Voltage A', value: quantity(core.value?.voltageA, Kilo, 'kV') },
    { name: 'Voltage B', value: quantity(core.value?.voltageB, Kilo, 'kV') },
    { name: 'Voltage C', value: quantity(core.value?.voltageC, Kilo, 'kV') },
    { name: 'Current A', value: quantity(core.value?.currentA, Kilo, 'kA') },
    { name: 'Current B', value: quantity(core.value?.currentB, Kilo, 'kA') },
    { name: 'Current C', value: quantity(core.value?.currentC, Kilo, 'kA') }
]);

const statusText: Record<HubStatus, string> = {
    connecting: 'Connecting…',
    connected: 'Live',
    reconnecting: 'Reconnecting…',
    disconnected: 'Disconnected'
};

const hint = computed(() => {
    if (status.value === 'disconnected' || status.value === 'reconnecting') {
        return 'The Luna service cannot be reached, the values are not updated.';
    }

    if (received.value && powerMeter.value === null) {
        return 'The system power meter is no longer in the DER configuration.';
    }

    return null;
});
</script>

<template>
    <section aria-labelledby="pm-summary-title">
        <div class="dash-section-header">
            <h2 id="pm-summary-title" class="dash-section-title">Summary</h2>
            <span class="dash-status" :class="`dash-status-${status}`">
                <span class="dash-status-dot" aria-hidden="true"></span>{{ statusText[status] }}
            </span>
        </div>

        <p v-if="hint" class="dash-hint">
            <i class="bi bi-exclamation-triangle me-1" aria-hidden="true"></i>{{ hint }}
        </p>

        <div v-if="received === false && hint === null" class="d-flex align-items-center gap-2 small">
            <span class="spinner-border spinner-inline" role="status" aria-hidden="true"></span>
            Loading...
        </div>

        <div v-else-if="powerMeter !== null" class="pm-columns">
            <dl class="pm-values">
                <dt class="pm-values-first">State</dt>
                <dd class="pm-values-first">{{ state }}</dd>
                <template v-for="row in leftRows" :key="row.name">
                    <dt>{{ row.name }}</dt>
                    <dd>{{ row.value }}</dd>
                </template>
            </dl>

            <dl class="pm-values">
                <dt class="pm-values-first">Communication Error</dt>
                <dd class="pm-values-first">{{ communicationError }}</dd>
                <template v-for="row in rightRows" :key="row.name">
                    <dt>{{ row.name }}</dt>
                    <dd>{{ row.value }}</dd>
                </template>
            </dl>
        </div>
    </section>
</template>
