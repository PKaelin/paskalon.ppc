<script setup lang="ts">
import { computed, onMounted, ref, useTemplateRef } from 'vue';
import DerTreeItem from './DerTreeItem.vue';
import { filterDerTree, type DerTreeNode } from '../utils/derTree';

const props = defineProps<{
    nodes: DerTreeNode[];
    loading: boolean;
    error: string | null;
}>();

defineEmits<{
    retry: [];
}>();

/** Id of the selected node, null if no node is selected. */
const selectedId = defineModel<string | null>({ required: true });

const filter = ref('');

/** Ids of the nodes whose children are hidden - all nodes are expanded at start up. */
const collapsedIds = ref<Set<string>>(new Set<string>());

const filterInput = useTemplateRef<HTMLInputElement>('filterInput');

const visibleNodes = computed(() => filterDerTree(props.nodes, filter.value));

function toggle(id: string): void {
    const collapsed = new Set<string>(collapsedIds.value);

    if (collapsed.delete(id) === false) {
        collapsed.add(id);
    }

    collapsedIds.value = collapsed;
}

// The tree is mounted when the System tab is opened, so the user can start typing a filter right away.
onMounted(() => filterInput.value?.focus());
</script>

<template>
    <section class="der-tree-panel" aria-label="Object tree">
        <div class="input-group input-group-sm der-tree-filter">
            <span class="input-group-text"><i class="bi bi-search" aria-hidden="true"></i></span>
            <!-- Not disabled while loading - a disabled input cannot take the focus. -->
            <input ref="filterInput"
                   v-model="filter"
                   class="form-control"
                   type="search"
                   placeholder="Filter by name"
                   aria-label="Filter the object tree by name" />
        </div>

        <div v-if="loading" class="d-flex align-items-center gap-2 small">
            <span class="spinner-border spinner-inline" role="status" aria-hidden="true"></span>
            Loading...
        </div>

        <div v-else-if="error" class="alert alert-danger py-2 small d-flex align-items-center justify-content-between gap-2" role="alert">
            <span>{{ error }}</span>
            <button class="btn btn-sm btn-outline-light" type="button" @click="$emit('retry')">Retry</button>
        </div>

        <p v-else-if="nodes.length === 0" class="om-hint">The system has no devices.</p>

        <p v-else-if="visibleNodes.length === 0" class="om-hint">No object matches '{{ filter.trim() }}'.</p>

        <ul v-else class="der-tree-list der-tree-root">
            <DerTreeItem v-for="node in visibleNodes"
                         :key="node.id"
                         :node="node"
                         :level="0"
                         :selected-id="selectedId"
                         :collapsed-ids="collapsedIds"
                         :filter="filter"
                         @select="selectedId = $event"
                         @toggle="toggle" />
        </ul>
    </section>
</template>
