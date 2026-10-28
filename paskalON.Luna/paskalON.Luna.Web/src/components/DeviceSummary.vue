<script setup lang="ts">
import { computed } from 'vue';
import type { HubStatus } from '../composables/useSystemMeterSummary';
import type { PropertyRow } from '../utils/deviceValues';

const props = defineProps<{
    status: HubStatus;
    /** False until the first values of the device were received. */
    received: boolean;
    /** False if the DER configuration no longer has the device. */
    found: boolean;
    /** Rows of the left column, the first row stands apart from the others. */
    leftRows: PropertyRow[];
    /** Rows of the right column, the first row stands apart from the others. */
    rightRows: PropertyRow[];
}>();

const statusText: Record<HubStatus, string> = {
    connecting: 'Connecting…',
    connected: 'Live',
    reconnecting: 'Reconnecting…',
    disconnected: 'Disconnected'
};

const hint = computed(() => {
    if (props.status === 'disconnected' || props.status === 'reconnecting') {
        return 'The Luna service cannot be reached, the values are not updated.';
    }

    if (props.received && props.found === false) {
        return 'The device is no longer in the DER configuration.';
    }

    return null;
});

const columns = computed(() => [props.leftRows, props.rightRows]);
</script>

<template>
    <section aria-labelledby="device-summary-title">
        <div class="dash-section-header">
            <h2 id="device-summary-title" class="dash-section-title">Summary</h2>
            <span class="dash-status" :class="`dash-status-${status}`">
                <span class="dash-status-dot" aria-hidden="true"></span>{{ statusText[status] }}
            </span>
        </div>

        <p v-if="hint" class="dash-hint">
            <i class="bi bi-exclamation-triangle me-1" aria-hidden="true"></i>{{ hint }}
        </p>

        <div v-if="received === false && hint === null" class="d-flex align-items-center gap-2 small">
            <span class="spinner-border spinner-inline" role="status" aria-hidden="true"></span>
            Loading...
        </div>

        <div v-else-if="received && found" class="pm-columns">
            <dl v-for="(rows, column) in columns" :key="column" class="pm-values">
                <template v-for="(row, index) in rows" :key="row.name">
                    <dt :class="{ 'pm-values-first': index === 0 }">{{ row.name }}</dt>
                    <dd :class="{ 'pm-values-first': index === 0 }">{{ row.value }}</dd>
                </template>
            </dl>
        </div>
    </section>
</template>
