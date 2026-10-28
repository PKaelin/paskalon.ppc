<script setup lang="ts">
import { computed } from 'vue';
import { derNodeIcons, findMatch, type DerTreeNode } from '../utils/derTree';

const props = defineProps<{
    node: DerTreeNode;
    /** Nesting level, 0 for the root nodes. */
    level: number;
    selectedId: string | null;
    /** Ids of the nodes whose children are hidden. */
    collapsedIds: ReadonlySet<string>;
    /** Filter text - the matching part of the name is highlighted and all nodes are expanded while filtering. */
    filter: string;
}>();

const emit = defineEmits<{
    select: [id: string];
    toggle: [id: string];
}>();

const hasChildren = computed(() => props.node.children.length > 0);

const isFiltering = computed(() => props.filter.trim() !== '');

const expanded = computed(() => hasChildren.value && (isFiltering.value || props.collapsedIds.has(props.node.id) === false));

const isSelected = computed(() => props.node.id === props.selectedId);

/** The name split around the part that matches the filter, null if nothing is highlighted. */
const nameParts = computed(() => {
    const match = findMatch(props.node.name, props.filter);

    if (match === null) {
        return null;
    }

    const name = props.node.name;
    const end = match.start + match.length;

    return { before: name.slice(0, match.start), match: name.slice(match.start, end), after: name.slice(end) };
});
</script>

<template>
    <li class="der-tree-item">
        <div class="der-tree-row" :class="{ active: isSelected }" :style="{ '--der-tree-level': level }">
            <button v-if="hasChildren"
                    class="der-tree-toggle"
                    type="button"
                    :disabled="isFiltering"
                    :aria-expanded="expanded"
                    :aria-label="expanded ? `Collapse ${node.name}` : `Expand ${node.name}`"
                    @click="emit('toggle', node.id)">
                <i class="bi" :class="expanded ? 'bi-chevron-down' : 'bi-chevron-right'" aria-hidden="true"></i>
            </button>
            <span v-else class="der-tree-toggle" aria-hidden="true"></span>

            <button class="der-tree-label"
                    type="button"
                    :title="node.name"
                    :aria-current="isSelected ? 'true' : undefined"
                    @click="emit('select', node.id)">
                <i class="bi" :class="derNodeIcons[node.type]" aria-hidden="true"></i>
                <span class="der-tree-name">
                    <template v-if="nameParts">{{ nameParts.before }}<mark>{{ nameParts.match }}</mark>{{ nameParts.after }}</template>
                    <template v-else>{{ node.name }}</template>
                </span>
            </button>
        </div>

        <ul v-if="expanded" class="der-tree-list">
            <DerTreeItem v-for="child in node.children"
                         :key="child.id"
                         :node="child"
                         :level="level + 1"
                         :selected-id="selectedId"
                         :collapsed-ids="collapsedIds"
                         :filter="filter"
                         @select="emit('select', $event)"
                         @toggle="emit('toggle', $event)" />
        </ul>
    </li>
</template>
