<script setup lang="ts">
import { nextTick, useTemplateRef, watch } from 'vue';

/** One action of the dialog. */
export interface DialogAction {
    id: string;
    label: string;
    /** Bootstrap button variant, e.g. 'success' or 'outline-light'. */
    variant: string;
}

defineProps<{
    title: string;
    actions: DialogAction[];
}>();

const emit = defineEmits<{
    choose: [id: string];
}>();

/** True while the dialog is shown. */
const open = defineModel<boolean>({ required: true });

const dialog = useTemplateRef<HTMLDivElement>('dialog');

// Focus the first action when the dialog opens, so it can be used with the keyboard right away.
watch(open, async (isOpen) => {
    if (isOpen) {
        await nextTick();
        dialog.value?.querySelector<HTMLButtonElement>('.action-dialog-actions button')?.focus();
    }
});

function choose(id: string): void {
    open.value = false;
    emit('choose', id);
}
</script>

<template>
    <Teleport to="body">
        <template v-if="open">
            <div class="modal-backdrop fade show"></div>
            <div ref="dialog"
                 class="modal fade show d-block"
                 role="dialog"
                 aria-modal="true"
                 :aria-label="title"
                 tabindex="-1"
                 @click.self="open = false"
                 @keydown.esc="open = false">
                <div class="modal-dialog modal-dialog-centered modal-sm">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h2 class="modal-title fs-6">{{ title }}</h2>
                            <button type="button" class="btn-close" aria-label="Close" @click="open = false"></button>
                        </div>
                        <div class="modal-body action-dialog-actions">
                            <button v-for="action in actions"
                                    :key="action.id"
                                    class="btn"
                                    :class="`btn-${action.variant}`"
                                    type="button"
                                    @click="choose(action.id)">
                                {{ action.label }}
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </template>
    </Teleport>
</template>
