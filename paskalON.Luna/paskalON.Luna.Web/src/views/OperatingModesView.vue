<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, useTemplateRef } from 'vue';
import OperatingModeList, { type OperatingModeDrop } from '../components/OperatingModeList.vue';
import SetpointEditor from '../components/SetpointEditor.vue';
import { getActiveOperatingModes, getAvailableOperatingModes, setOperatingModes } from '../api/operatingModeApi';
import type { OperatingModeStackDto } from '../types/operatingModes';
import { proposeDrop, proposeRemove, type StackProposal } from '../utils/stackRules';
import { useNotifications } from '../composables/useNotifications';

/** State of one operating mode list. */
interface ListState {
    modes: OperatingModeStackDto[];
    loading: boolean;
    error: string | null;
}

const { notify } = useNotifications();

/** All stackable operating modes, including the ones in the stack. */
const available = ref<ListState>({ modes: [], loading: true, error: null });

/** The stack as the operating mode service has it. */
const active = ref<ListState>({ modes: [], loading: true, error: null });

/** The stack as edited by the user, applied with "Apply Changes". */
const draft = ref<OperatingModeStackDto[]>([]);

const applying = ref(false);

const setpointEditor = useTemplateRef<InstanceType<typeof SetpointEditor>>('setpointEditor');

const abortController = new AbortController();

/** The available operating modes that are not in the edited stack. */
const availableModes = computed(() => {
    const names = new Set(draft.value.map((mode) => mode.operatingModeName));

    return available.value.modes.filter((mode) => names.has(mode.operatingModeName) === false);
});

const isDirty = computed(() => names(draft.value).join('\n') !== names(active.value.modes).join('\n'));

function names(modes: OperatingModeStackDto[]): string[] {
    return modes.map((mode) => mode.operatingModeName);
}

async function loadList(state: ListState, fetchModes: (signal: AbortSignal) => Promise<OperatingModeStackDto[]>): Promise<boolean> {
    state.loading = true;
    state.error = null;

    try {
        state.modes = await fetchModes(abortController.signal);

        return true;
    } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
            return false;
        }

        state.error = error instanceof Error ? error.message : String(error);

        return false;
    } finally {
        state.loading = false;
    }
}

function loadAvailable(): Promise<boolean> {
    return loadList(available.value, getAvailableOperatingModes);
}

/** Loads the stack of the operating mode service, the edited stack starts over from it. */
async function loadActive(): Promise<boolean> {
    const isLoaded = await loadList(active.value, getActiveOperatingModes);

    if (isLoaded) {
        draft.value = [...active.value.modes];
    }

    return isLoaded;
}

function findMode(drop: OperatingModeDrop): OperatingModeStackDto | undefined {
    const source = drop.from === 'active' ? draft.value : available.value.modes;

    return source.find((mode) => mode.operatingModeName === drop.operatingModeName);
}

/** Applies a drop to the edited stack if it keeps the stack valid, otherwise the drop is refused with the reason. */
function onDrop(drop: OperatingModeDrop): void {
    const mode = findMode(drop);

    if (mode === undefined || drop.to === 'available' && drop.from === 'available') {
        return;
    }

    const proposal: StackProposal = drop.to === 'active'
        ? proposeDrop(draft.value, mode, drop.index, drop.from === 'active')
        : proposeRemove(draft.value, mode);

    if (proposal.ok === false) {
        notify(`'${mode.operatingModeName}' cannot be moved there: ${proposal.reason}`, 'warning', 6000);

        return;
    }

    draft.value = proposal.stack;

    if (proposal.replaced.length > 0) {
        notify(`'${mode.operatingModeName}' replaces ${proposal.replaced.map((entry) => `'${entry.operatingModeName}'`).join(' and ')}.`, 'info', 4000);
    }
}

/** Removes all operating modes from the edited stack - an empty stack is always valid, it is applied with "Apply Changes". */
function clearAll(): void {
    draft.value = [];
}

function discardChanges(): void {
    draft.value = [...active.value.modes];
}

/**
 * Writes the edited stack in one atomic change, then applies the setpoints like the "Apply" button of the setpoints.
 * An empty stack sets the setpoints to 0 - the operating mode service ramps the removed modes down to 0.
 */
async function applyChanges(): Promise<void> {
    const editor = setpointEditor.value;
    const isStackEmpty = draft.value.length === 0;

    if (editor === null || (isStackEmpty === false && editor.isValid !== true)) {
        notify('Enter valid active and reactive setpoints before applying the changes.', 'warning', 6000);

        return;
    }

    applying.value = true;

    try {
        try {
            await setOperatingModes(names(draft.value));
        } catch (error) {
            const message = error instanceof Error ? error.message : String(error);
            notify(`The operating mode stack was rejected: ${message}`, 'danger', 0);

            return;
        }

        notify(isStackEmpty ? 'All operating modes removed, the system ramps down to 0.' : 'Operating mode stack applied.', 'success', 4000);
        await (isStackEmpty ? editor.applyZero() : editor.apply());
        await loadActive();
    } finally {
        applying.value = false;
    }
}

onMounted(() => {
    void loadAvailable();
    void loadActive();
});

onBeforeUnmount(() => abortController.abort());
</script>

<template>
    <div class="om-layout">
        <SetpointEditor ref="setpointEditor" class="om-area-setpoints" />

        <OperatingModeList class="om-area-available"
                           title="Available Operating Modes"
                           variant="available"
                           :modes="availableModes"
                           :loading="available.loading"
                           :error="available.error"
                           :locked="applying"
                           @retry="loadAvailable"
                           @drop="onDrop" />

        <OperatingModeList class="om-area-active"
                           title="Active Operating Modes"
                           variant="active"
                           :modes="draft"
                           :loading="active.loading"
                           :error="active.error"
                           :locked="applying"
                           @retry="loadActive"
                           @drop="onDrop">
            <template #footer>
                <p v-if="draft.length === 0" class="om-hint mt-2 mb-0">
                    <i class="bi bi-info-circle me-1" aria-hidden="true"></i>No operating modes in the stack.
                    <template v-if="isDirty">Applying it sets the setpoints to 0 and ramps the system down.</template>
                </p>
                <div v-if="draft.length > 0 || isDirty || applying" class="om-stack-actions">
                    <button v-if="draft.length > 0"
                            class="btn btn-sm btn-outline-danger me-auto"
                            type="button"
                            :disabled="applying"
                            @click="clearAll">
                        Clear All
                    </button>
                    <template v-if="isDirty || applying">
                        <button class="btn btn-sm btn-outline-light" type="button" :disabled="applying" @click="discardChanges">
                            Discard Changes
                        </button>
                        <button class="btn btn-sm btn-secondary" type="button" :disabled="applying" @click="applyChanges">
                            <span v-if="applying" class="spinner-border spinner-inline me-1" role="status" aria-hidden="true"></span>
                            Apply Changes
                        </button>
                    </template>
                </div>
            </template>
        </OperatingModeList>
    </div>
</template>
