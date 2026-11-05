<script setup lang="ts">
import { computed, toRef } from 'vue';
import DeviceSummary from './DeviceSummary.vue';
import { useDeviceSubscription } from '../composables/useDeviceSubscription';
import { PowerMeterStateNames, type DerCircuitDto } from '../types/der';
import { communicationErrorRow, formatEnum, formatQuantity, Mega, NoValue, type PropertyRow } from '../utils/deviceValues';
import { pcsSummaryRows } from '../utils/pcsSummary';

const props = defineProps<{
    /** Name of the DER circuit (DER circuits have no device ID). */
    circuitName: string;
}>();

const { status, device, received } = useDeviceSubscription<DerCircuitDto>(toRef('DerCircuitDto' as const), toRef(() => props.circuitName));

const meter = computed(() => device.value?.circuitPowerMeter ?? null);

const meterCore = computed(() => meter.value?.core ?? null);

const meterLeftRows = computed<PropertyRow[]>(() => [
    { name: 'State', value: formatEnum(meterCore.value?.state, PowerMeterStateNames) },
    { name: 'Active Power', value: formatQuantity(meterCore.value?.activePower?.watts, Mega, 'MW') },
    { name: 'Reactive Power', value: formatQuantity(meterCore.value?.reactivePower?.voltAmperesReactive, Mega, 'MVAr') }
]);

const meterRightRows = computed<PropertyRow[]>(() => [
    communicationErrorRow(meterCore.value?.communicationError),
    { name: 'Apparent Power', value: formatQuantity(meterCore.value?.apparentPower?.voltAmperes, Mega, 'MVA') },
    { name: 'Power Factor', value: formatQuantity(meterCore.value?.powerFactor, 1, '') }
]);

/** Summed up values of the PCSs of all DER units (battery storage and solar) of the circuit. */
const pcsSummary = computed(() => pcsSummaryRows(device.value?.derUnits ?? []));
</script>

<template>
    <DeviceSummary title="Summary Circuit Meter"
                   :status="status"
                   :received="received"
                   :found="device !== null"
                   :empty-text="meter === null ? NoValue : null"
                   :left-rows="meterLeftRows"
                   :right-rows="meterRightRows" />
    <DeviceSummary title="Summary Power Conversion Systems"
                   class="pm-section-next"
                   :status="status"
                   :received="received"
                   :found="device !== null"
                   :status-visible="false"
                   :left-rows="pcsSummary.leftRows"
                   :right-rows="pcsSummary.rightRows" />
</template>
