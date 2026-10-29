<script setup lang="ts">
import { computed, toRef } from 'vue';
import DeviceSummary from './DeviceSummary.vue';
import { useDeviceSubscription } from '../composables/useDeviceSubscription';
import { PowerMeterStateNames, type PowerMeterDto, type PowerMeterType } from '../types/der';
import { communicationErrorRow, formatEnum, formatQuantity, Kilo, Mega, type PropertyRow } from '../utils/deviceValues';

const props = defineProps<{
    /** Type of the power meter (system, auxiliary or external). */
    meterType: PowerMeterType;
    /** Device ID of the power meter. */
    deviceId: number;
}>();

const { status, device, received } = useDeviceSubscription<PowerMeterDto>(toRef(() => props.meterType), toRef(() => props.deviceId));

const core = computed(() => device.value?.core ?? null);

const detail = computed(() => device.value?.detail ?? null);

const leftRows = computed<PropertyRow[]>(() => [
    { name: 'State', value: formatEnum(core.value?.state, PowerMeterStateNames) },
    { name: 'Active Power', value: formatQuantity(core.value?.activePower?.watts, Mega, 'MW') },
    { name: 'Reactive Power', value: formatQuantity(core.value?.reactivePower?.voltAmperesReactive, Mega, 'MVAr') },
    { name: 'Apparent Power', value: formatQuantity(core.value?.apparentPower?.voltAmperes, Mega, 'MVA') },
    { name: 'Power Factor', value: formatQuantity(core.value?.powerFactor, 1, '') },
    { name: 'Frequency', value: formatQuantity(core.value?.frequency, 1, 'Hz') },
    { name: 'Energy Delivered', value: formatQuantity(detail.value?.energyDelivered?.wattHours, Mega, 'MWh') },
    { name: 'Reactive Energy Delivered', value: formatQuantity(detail.value?.reactiveEnergyDelivered?.voltAmperesReactiveHours, Mega, 'MVArh') },
    { name: 'Energy Received', value: formatQuantity(detail.value?.energyReceived?.wattHours, Mega, 'MWh') },
    { name: 'Reactive Energy Received', value: formatQuantity(detail.value?.reactiveEnergyReceived?.voltAmperesReactiveHours, Mega, 'MVArh') }
]);

const rightRows = computed<PropertyRow[]>(() => [
    communicationErrorRow(core.value?.communicationError),
    { name: 'Active Power A', value: formatQuantity(detail.value?.activePowerA?.watts, Mega, 'MW') },
    { name: 'Active Power B', value: formatQuantity(detail.value?.activePowerB?.watts, Mega, 'MW') },
    { name: 'Active Power C', value: formatQuantity(detail.value?.activePowerC?.watts, Mega, 'MW') },
    { name: 'Reactive Power A', value: formatQuantity(detail.value?.reactivePowerA?.voltAmperesReactive, Mega, 'MVAr') },
    { name: 'Reactive Power B', value: formatQuantity(detail.value?.reactivePowerB?.voltAmperesReactive, Mega, 'MVAr') },
    { name: 'Reactive Power C', value: formatQuantity(detail.value?.reactivePowerC?.voltAmperesReactive, Mega, 'MVAr') },
    { name: 'Voltage A', value: formatQuantity(core.value?.voltageA, Kilo, 'kV') },
    { name: 'Voltage B', value: formatQuantity(core.value?.voltageB, Kilo, 'kV') },
    { name: 'Voltage C', value: formatQuantity(core.value?.voltageC, Kilo, 'kV') },
    { name: 'Current A', value: formatQuantity(core.value?.currentA, Kilo, 'kA') },
    { name: 'Current B', value: formatQuantity(core.value?.currentB, Kilo, 'kA') },
    { name: 'Current C', value: formatQuantity(core.value?.currentC, Kilo, 'kA') }
]);
</script>

<template>
    <DeviceSummary :status="status" :received="received" :found="device !== null" :left-rows="leftRows" :right-rows="rightRows" />
</template>
