<script setup lang="ts">
import { computed } from 'vue';
import AccordionSection from './AccordionSection.vue';
import PropertyGrid from './PropertyGrid.vue';
import ModbusEndpointGrid from './ModbusEndpointGrid.vue';
import C37EndpointGrid from './C37EndpointGrid.vue';
import type { TreeNode } from '@/types/tree';

/**
 * Renders one node of the tree and - recursively - all of its children.
 * Because every level (group, circuit, unit, device, meter) is described by the same
 * TreeNode structure, one component covers the complete hierarchy.
 */
const props = defineProps<{
    node: TreeNode;
    level: number;
    matchedIds: Set<string>;
}>();

const deviceId = computed(() => props.node.deviceId ?? -1);

const counter = computed(() => {
    const parts: string[] = [];

    if (props.node.children.length > 0) {
        parts.push(`${props.node.children.length} section${props.node.children.length === 1 ? '' : 's'}`);
    }

    const endpointCount =
        (props.node.modbusEndpoints?.length ?? 0) + (props.node.c37Endpoints?.length ?? 0);

    if (endpointCount > 0) {
        parts.push(`${endpointCount} endpoint${endpointCount === 1 ? '' : 's'}`);
    }

    return parts.join(' · ');
});
</script>

<template>
    <AccordionSection :node-id="node.id"
                      :title="node.name"
                      :type-label="node.typeLabel"
                      :icon="node.icon"
                      :level="level"
                      :counter="counter"
                      :matched="matchedIds.has(node.id)">
        <PropertyGrid :rows="node.properties" />

        <ModbusEndpointGrid v-if="node.modbusEndpoints && node.modbusEndpoints.length > 0"
                            :endpoints="node.modbusEndpoints"
                            :device-id="deviceId"
                            :device-name="node.name" />

        <C37EndpointGrid v-if="node.c37Endpoints && node.c37Endpoints.length > 0"
                         :endpoints="node.c37Endpoints"
                         :device-id="deviceId"
                         :device-name="node.name" />

        <div v-if="node.children.length > 0" class="mt-2">
            <TreeSection v-for="child in node.children"
                         :key="child.id"
                         :node="child"
                         :level="level + 1"
                         :matched-ids="matchedIds" />
        </div>
    </AccordionSection>
</template>
