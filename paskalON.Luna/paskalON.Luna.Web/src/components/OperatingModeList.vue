<script setup lang="ts">
import { onBeforeUnmount, useTemplateRef, watch } from 'vue';
import Sortable, { type SortableEvent } from 'sortablejs';
import OperatingModeItem from './OperatingModeItem.vue';
import type { OperatingModeStackDto } from '../types/operatingModes';

/** Variant of a list, also the identifier of the list in a drop event. */
export type ListVariant = 'available' | 'active';

/** An operating mode dragged from one list and dropped on a list (the same or the other one). */
export interface OperatingModeDrop {
    operatingModeName: string;
    from: ListVariant;
    to: ListVariant;
    /** Position in the target list where the operating mode was dropped. */
    index: number;
}

const props = defineProps<{
    title: string;
    modes: OperatingModeStackDto[];
    variant: ListVariant;
    loading: boolean;
    error: string | null;
    /** True to block dragging, e.g. while the changes are applied. */
    locked?: boolean;
}>();

const emit = defineEmits<{
    retry: [];
    drop: [drop: OperatingModeDrop];
}>();

const list = useTemplateRef<HTMLUListElement>('list');

let sortable: Sortable | undefined;

/** Node after the dragged item before the drag - used to put the item back where Vue rendered it. */
let nextSibling: Node | null = null;

function onStart(event: SortableEvent): void {
    nextSibling = event.item.nextSibling;
}

function onEnd(event: SortableEvent): void {
    const { item, from, to, newIndex } = event;

    // SortableJS moved the DOM node. Put it back so Vue stays the owner of the DOM and renders the change from the data.
    item.remove();
    from.insertBefore(item, nextSibling);
    nextSibling = null;

    const operatingModeName = item.dataset.name;
    const target = to.dataset.list as ListVariant | undefined;

    if (operatingModeName === undefined || target === undefined || newIndex === undefined) {
        return;
    }

    if (from === to && event.oldIndex === newIndex) {
        return;
    }

    emit('drop', { operatingModeName, from: props.variant, to: target, index: newIndex });
}

// The list element only exists while the modes are shown, so SortableJS is attached whenever it appears.
watch(list, (element) => {
    sortable?.destroy();
    sortable = undefined;

    if (element === null) {
        return;
    }

    sortable = Sortable.create(element, {
        group: 'operating-modes',
        // The available modes are ordered by name, only the stack can be reordered
        sort: props.variant === 'active',
        animation: 150,
        disabled: props.locked === true,
        ghostClass: 'om-item-ghost',
        chosenClass: 'om-item-chosen',
        onStart,
        onEnd
    });
}, { flush: 'post' });

watch(() => props.locked, (locked) => sortable?.option('disabled', locked === true));

onBeforeUnmount(() => sortable?.destroy());
</script>

<template>
    <section class="om-panel">
        <h2 class="om-heading">{{ title }}</h2>

        <div v-if="loading" class="d-flex align-items-center gap-2 small">
            <span class="spinner-border spinner-inline" role="status" aria-hidden="true"></span>
            Loading...
        </div>

        <div v-else-if="error" class="alert alert-danger py-2 small d-flex align-items-center justify-content-between gap-2" role="alert">
            <span>{{ error }}</span>
            <button class="btn btn-sm btn-outline-light" type="button" @click="$emit('retry')">Retry</button>
        </div>

        <template v-else>
            <!-- The list stays rendered when it is empty, so operating modes can still be dropped on it. -->
            <ul ref="list" class="om-list" :class="[`om-list-${variant}`, { 'om-list-empty': modes.length === 0 }]" :data-list="variant">
                <OperatingModeItem v-for="mode in modes"
                                   :key="mode.operatingModeName"
                                   :mode="mode"
                                   :variant="variant"
                                   :data-name="mode.operatingModeName" />
            </ul>

            <slot name="footer"></slot>
        </template>
    </section>
</template>
