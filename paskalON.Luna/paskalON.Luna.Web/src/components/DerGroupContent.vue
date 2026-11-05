<script setup lang="ts">
import { computed, toRef } from 'vue';
import DeviceSummary from './DeviceSummary.vue';
import { useDeviceSubscription } from '../composables/useDeviceSubscription';
import type { DerGroupDto } from '../types/der';
import { pcsSummaryRows } from '../utils/pcsSummary';

const props = defineProps<{
    /** Name of the DER group (DER groups have no device ID). */
    groupName: string;
}>();

const { status, device, received } = useDeviceSubscription<DerGroupDto>(toRef('DerGroupDto' as const), toRef(() => props.groupName));

/** Summed up values of the PCSs of all DER units (battery storage and solar) of all circuits of the group. */
const pcsSummary = computed(() => pcsSummaryRows((device.value?.derCircuits ?? []).flatMap(circuit => circuit.derUnits ?? [])));
</script>

<template>
    <DeviceSummary title="Summary Power Conversion Systems"
                   :status="status"
                   :received="received"
                   :found="device !== null"
                   :left-rows="pcsSummary.leftRows"
                   :right-rows="pcsSummary.rightRows" />
</template>
