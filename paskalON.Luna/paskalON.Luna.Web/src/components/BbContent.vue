<script setup lang="ts">
import { computed, toRef } from 'vue';
import DeviceSummary from './DeviceSummary.vue';
import { useDeviceSubscription } from '../composables/useDeviceSubscription';
import { BatteryBankFlowDirectionNames, BatteryBankStateNames, type BbDto } from '../types/der';
import { formatBoolean, formatEnum, formatQuantity, Kilo, type PropertyRow } from '../utils/deviceValues';

const props = defineProps<{
    /** Device ID of the battery bank. */
    deviceId: number;
}>();

const { status, device, received } = useDeviceSubscription<BbDto>(toRef('BbDto' as const), toRef(() => props.deviceId));

const core = computed(() => device.value?.core ?? null);

const detail = computed(() => device.value?.detail ?? null);

const leftRows = computed<PropertyRow[]>(() => [
    { name: 'State', value: formatEnum(core.value?.state, BatteryBankStateNames) },
    { name: 'State Of Charge', value: formatQuantity(core.value?.stateOfCharge, 1, '%') },
    { name: 'Battery Bank Flow Direction', value: formatEnum(core.value?.batteryBankFlowDirection, BatteryBankFlowDirectionNames) },
    { name: 'Total DC Voltage', value: formatQuantity(core.value?.totalDCVoltage, Kilo, 'kV') },
    { name: 'Total DC Current', value: formatQuantity(core.value?.totalDCCurrent, Kilo, 'kA') }
]);

const rightRows = computed<PropertyRow[]>(() => [
    { name: 'Communication Error', value: formatBoolean(core.value?.communicationError) },
    { name: 'Is In Maintenance Mode', value: formatBoolean(detail.value?.isInMaintenanceMode) },
    { name: 'State Of Health', value: formatQuantity(detail.value?.stateOfHealth, 1, '%') },
    { name: 'Minimum Cell Voltage', value: formatQuantity(detail.value?.minimumCellVoltage, 1, 'V') },
    { name: 'Maximum Cell Voltage', value: formatQuantity(detail.value?.maximumCellVoltage, 1, 'V') },
    { name: 'Minimum Rack Temperature', value: formatQuantity(detail.value?.minimumRackTemperature, 1, '°C') },
    { name: 'Maximum Rack Temperature', value: formatQuantity(detail.value?.maximumRackTemperature, 1, '°C') }
]);
</script>

<template>
    <DeviceSummary :status="status" :received="received" :found="device !== null" :left-rows="leftRows" :right-rows="rightRows" />
</template>
