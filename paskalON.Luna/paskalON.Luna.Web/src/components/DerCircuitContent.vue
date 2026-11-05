<script setup lang="ts">
import { computed, toRef } from 'vue';
import DeviceSummary from './DeviceSummary.vue';
import { useDeviceSubscription } from '../composables/useDeviceSubscription';
import { PowerMeterStateNames, type DerCircuitDto, type PcsCoreDto } from '../types/der';
import { communicationErrorRow, formatEnum, formatQuantity, Mega, NoValue, type PropertyRow } from '../utils/deviceValues';

const props = defineProps<{
    /** Name of the DER circuit (DER circuits have no device ID). */
    circuitName: string;
}>();

const { status, device, received } = useDeviceSubscription<DerCircuitDto>(toRef('DerCircuitDto' as const), toRef(() => props.circuitName));

const meter = computed(() => device.value?.circuitPowerMeter ?? null);

const meterCore = computed(() => meter.value?.core ?? null);

/** Cores of the PCSs of all DER units (battery storage and solar) of the circuit, without the PCSs that delivered no core yet. */
const pcsCores = computed<PcsCoreDto[]>(() => (device.value?.derUnits ?? [])
    .map(unit => unit.powerConversionSystem?.core ?? null)
    .filter((core): core is PcsCoreDto => core !== null));

/** True if any PCS has a communication error. A PCS without core yet does not count, like in DerBatteryStorageUnitDto. */
const pcsCommunicationError = computed(() => pcsCores.value.some(core => core.communicationError === true));

/** Sum of the values of the PCSs that have a value, null if no PCS has a value. */
function sum(values: (number | null | undefined)[]): number | null {
    const present = values.filter((value): value is number => value !== null && value !== undefined);

    return present.length === 0 ? null : present.reduce((total, value) => total + value, 0);
}

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

const pcsLeftRows = computed<PropertyRow[]>(() => [
    // The first row of the left column stays empty, so the sums line up with the right column.
    { name: '', value: ' ' },
    { name: 'Active Power Target', value: formatQuantity(sum(pcsCores.value.map(core => core.activePowerTarget?.watts)), Mega, 'MW') },
    { name: 'Active Power', value: formatQuantity(sum(pcsCores.value.map(core => core.activePower?.watts)), Mega, 'MW') },
    { name: 'Available Active Power', value: formatQuantity(sum(pcsCores.value.map(core => core.activeAvailablePower?.watts)), Mega, 'MW') }
]);

const pcsRightRows = computed<PropertyRow[]>(() => [
    communicationErrorRow(pcsCommunicationError.value),
    { name: 'Reactive Power Target', value: formatQuantity(sum(pcsCores.value.map(core => core.reactivePowerTarget?.voltAmperesReactive)), Mega, 'MVAr') },
    { name: 'Reactive Power', value: formatQuantity(sum(pcsCores.value.map(core => core.reactivePower?.voltAmperesReactive)), Mega, 'MVAr') },
    { name: 'Available Reactive Power', value: formatQuantity(sum(pcsCores.value.map(core => core.reactiveAvailablePower?.voltAmperesReactive)), Mega, 'MVAr') }
]);
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
                   :left-rows="pcsLeftRows"
                   :right-rows="pcsRightRows" />
</template>
