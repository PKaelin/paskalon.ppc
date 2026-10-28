<script setup lang="ts">
import { computed, toRef } from 'vue';
import DeviceSummary from './DeviceSummary.vue';
import { useDeviceSubscription } from '../composables/useDeviceSubscription';
import { PcsStateNames, type PcsDto } from '../types/der';
import { formatBoolean, formatEnum, formatQuantity, Kilo, Mega, NoValue, type PropertyRow } from '../utils/deviceValues';

const props = defineProps<{
    /** Device ID of the power conversion system. */
    deviceId: number;
}>();

const { status, device, received } = useDeviceSubscription<PcsDto>(toRef('PcsDto' as const), toRef(() => props.deviceId));

const core = computed(() => device.value?.core ?? null);

const detail = computed(() => device.value?.detail ?? null);

/** One row per DC contactor, a single "None" row if the PCS does not report them. */
const dcContactorRows = computed<PropertyRow[]>(() => {
    const contactors = detail.value?.isDcContactorClosed;

    if (contactors === null || contactors === undefined || contactors.length === 0) {
        return [{ name: 'Is DC Contactor Closed', value: NoValue }];
    }

    return contactors.map((isClosed, index) => ({ name: `Is DC Contactor ${index + 1} Closed`, value: formatBoolean(isClosed) }));
});

const leftRows = computed<PropertyRow[]>(() => [
    { name: 'State', value: formatEnum(core.value?.state, PcsStateNames) },
    { name: 'Active Power Target', value: formatQuantity(detail.value?.activePowerTarget?.watts, Mega, 'MW') },
    { name: 'Active Power', value: formatQuantity(core.value?.activePower?.watts, Mega, 'MW') },
    { name: 'Available Active Power', value: formatQuantity(core.value?.activeAvailablePower?.watts, Mega, 'MW') },
    { name: 'Reactive Power Target', value: formatQuantity(detail.value?.reactivePowerTarget?.voltAmperesReactive, Mega, 'MVAr') },
    { name: 'Reactive Power', value: formatQuantity(core.value?.reactivePower?.voltAmperesReactive, Mega, 'MVAr') },
    { name: 'Available Reactive Power', value: formatQuantity(core.value?.reactiveAvailablePower?.voltAmperesReactive, Mega, 'MVAr') },
    { name: 'DC Current', value: formatQuantity(core.value?.dcCurrent, Kilo, 'kA') },
    { name: 'DC Voltage', value: formatQuantity(core.value?.dcVoltage, Kilo, 'kV') },
    { name: 'AC Current', value: formatQuantity(core.value?.acCurrent, Kilo, 'kA') },
    { name: 'AC Voltage', value: formatQuantity(core.value?.acVoltage, Kilo, 'kV') }
]);

const rightRows = computed<PropertyRow[]>(() => [
    { name: 'Communication Error', value: formatBoolean(core.value?.communicationError) },
    { name: 'Is In Maintenance Mode', value: formatBoolean(detail.value?.isInMaintenanceMode) },
    { name: 'Is AC Breaker Closed', value: formatBoolean(detail.value?.isACBreakerClosed) },
    ...dcContactorRows.value
]);
</script>

<template>
    <DeviceSummary :status="status" :received="received" :found="device !== null" :left-rows="leftRows" :right-rows="rightRows" />
</template>
