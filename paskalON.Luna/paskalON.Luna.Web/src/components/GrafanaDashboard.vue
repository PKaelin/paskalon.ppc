<script setup lang="ts">
/**
 * Shows a Grafana dashboard, referenced by its name, filling the size of its parent.
 * Everything about Grafana itself (address, authentication, parameters) is in api/grafana.ts.
 */
import { onBeforeUnmount, onMounted, ref, watch } from 'vue';
import { dashboardUrl, ensureReachable } from '../api/grafana';
import type { GrafanaDashboardParameters } from '../types/grafana';

type DashboardState = 'loading' | 'ready' | 'error';

const props = withDefaults(
    defineProps<{
        /** Name of the dashboard, as configured in grafana.dashboards. */
        name: string;
        /** Optional time range, refresh interval and variables of the dashboard. */
        parameters?: GrafanaDashboardParameters;
        /** Time in milliseconds the dashboard may take to load before an error is shown. */
        loadTimeoutMs?: number;
    }>(),
    {
        parameters: () => ({}),
        loadTimeoutMs: 20_000
    }
);

const container = ref<HTMLElement | null>(null);

const state = ref<DashboardState>('loading');

const errorMessage = ref<string | null>(null);

const frameUrl = ref<string | null>(null);

/** Changes on every load, so a retry recreates the frame even if the URL is the same. */
const frameKey = ref(0);

/** The frame is only created once the container has a size, Grafana lays out its panels on load. */
const hasSize = ref(false);

let abortController: AbortController | null = null;

let loadTimer: ReturnType<typeof setTimeout> | undefined;

let resizeObserver: ResizeObserver | null = null;

function clearLoadTimer(): void {
    if (loadTimer !== undefined) {
        clearTimeout(loadTimer);
        loadTimer = undefined;
    }
}

function fail(message: string): void {
    clearLoadTimer();
    frameUrl.value = null;
    errorMessage.value = message;
    state.value = 'error';
}

async function load(): Promise<void> {
    abortController?.abort();
    abortController = new AbortController();
    const signal = abortController.signal;

    clearLoadTimer();
    state.value = 'loading';
    errorMessage.value = null;
    frameUrl.value = null;

    try {
        const url = dashboardUrl(props.name, props.parameters);
        await ensureReachable(signal);

        frameKey.value++;
        frameUrl.value = url;
        loadTimer = setTimeout(() => fail(`The Grafana dashboard '${props.name}' did not load in time.`), props.loadTimeoutMs);
    } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
            return;
        }

        fail(error instanceof Error ? error.message : String(error));
    }
}

function onFrameLoad(): void {
    if (state.value === 'loading') {
        clearLoadTimer();
        state.value = 'ready';
    }
}

watch(() => [props.name, props.parameters], () => void load(), { deep: true });

onMounted(() => {
    if (container.value !== null) {
        resizeObserver = new ResizeObserver((entries) => {
            const size = entries[0]?.contentRect;
            hasSize.value = size !== undefined && size.width > 0 && size.height > 0;
        });
        resizeObserver.observe(container.value);
    }

    void load();
});

onBeforeUnmount(() => {
    abortController?.abort();
    clearLoadTimer();
    resizeObserver?.disconnect();
});
</script>

<template>
    <div ref="container" class="grafana-dashboard" :aria-busy="state === 'loading'">
        <iframe v-if="frameUrl !== null && hasSize"
                :key="frameKey"
                class="grafana-dashboard-frame"
                :class="{ 'grafana-dashboard-frame-hidden': state !== 'ready' }"
                :src="frameUrl"
                :title="name"
                referrerpolicy="strict-origin-when-cross-origin"
                @load="onFrameLoad"></iframe>

        <div v-if="state === 'loading'" class="grafana-dashboard-overlay">
            <div class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></div>
            <span>Loading {{ name }}…</span>
        </div>

        <div v-else-if="state === 'error'" class="grafana-dashboard-overlay" role="alert">
            <span class="dash-hint mb-0">
                <i class="bi bi-exclamation-triangle me-1" aria-hidden="true"></i>{{ errorMessage }}
            </span>
            <button class="btn btn-link btn-sm p-0" type="button" @click="load">Retry</button>
        </div>
    </div>
</template>
