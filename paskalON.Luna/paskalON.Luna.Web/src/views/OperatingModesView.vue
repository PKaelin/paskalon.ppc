<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue';
import OperatingModeList from '../components/OperatingModeList.vue';
import SetpointEditor from '../components/SetpointEditor.vue';
import { getActiveOperatingModes, getAvailableOperatingModes } from '../api/operatingModeApi';
import type { OperatingModeStackDto } from '../types/operatingModes';

/** State of one operating mode list. */
interface ListState {
    modes: OperatingModeStackDto[];
    loading: boolean;
    error: string | null;
}

const available = ref<ListState>({ modes: [], loading: true, error: null });
const active = ref<ListState>({ modes: [], loading: true, error: null });

const abortController = new AbortController();

async function loadList(state: ListState, fetchModes: (signal: AbortSignal) => Promise<OperatingModeStackDto[]>): Promise<void> {
    state.loading = true;
    state.error = null;

    try {
        state.modes = await fetchModes(abortController.signal);
    } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
            return;
        }

        state.error = error instanceof Error ? error.message : String(error);
    } finally {
        state.loading = false;
    }
}

function loadAvailable(): Promise<void> {
    return loadList(available.value, getAvailableOperatingModes);
}

function loadActive(): Promise<void> {
    return loadList(active.value, getActiveOperatingModes);
}

onMounted(() => {
    void loadAvailable();
    void loadActive();
});

onBeforeUnmount(() => abortController.abort());
</script>

<template>
    <div class="om-layout">
        <SetpointEditor class="om-area-setpoints" />

        <OperatingModeList class="om-area-available"
                           title="Available Operating Modes"
                           variant="available"
                           :modes="available.modes"
                           :loading="available.loading"
                           :error="available.error"
                           @retry="loadAvailable" />

        <OperatingModeList class="om-area-active"
                           title="Active Operating Modes"
                           variant="active"
                           :modes="active.modes"
                           :loading="active.loading"
                           :error="active.error"
                           @retry="loadActive" />
    </div>
</template>
