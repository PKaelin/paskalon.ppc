<script setup lang="ts">
import { computed, toRef } from 'vue';
import BatteryStorageUnitControls from './BatteryStorageUnitControls.vue';
import DeviceSummary from './DeviceSummary.vue';
import { useDeviceSubscription } from '../composables/useDeviceSubscription';
import type { DerBatteryStorageUnitDto } from '../types/der';
import { communicationErrorRow, formatBoolean, formatQuantity, Mega, type PropertyRow } from '../utils/deviceValues';

const props = defineProps<{
    /** Name of the battery storage unit (DER units have no device ID). */
    unitName: string;
}>();

const { status, device, received } = useDeviceSubscription<DerBatteryStorageUnitDto>(toRef('DerBatteryStorageUnitDto' as const), toRef(() => props.unitName));

const leftRows = computed<PropertyRow[]>(() => [
    { name: 'Is In Maintenance Mode', value: formatBoolean(device.value?.isInMaintenanceMode) },
    { name: 'Available Active Power', value: formatQuantity(device.value?.availableActivePower?.watts, Mega, 'MW') },
    { name: 'Available Reactive Power', value: formatQuantity(device.value?.availableReactivePower?.voltAmperesReactive, Mega, 'MVAr') }
]);

const rightRows = computed<PropertyRow[]>(() => [
    communicationErrorRow(device.value?.communicationError)
]);
</script>

<template>
    <BatteryStorageUnitControls :unit-name="unitName" :unit="device" />
    <DeviceSummary :status="status" :received="received" :found="device !== null" :left-rows="leftRows" :right-rows="rightRows" />
</template>
