<script setup lang="ts">
import { computed, useId } from 'vue';
import type { HubStatus } from '../composables/useSystemMeterSummary';
import type { PropertyRow, ValueIndicator } from '../utils/deviceValues';

const props = withDefaults(defineProps<{
    status: HubStatus;
    /** False until the first values of the device were received. */
    received: boolean;
    /** False if the DER configuration no longer has the device. */
    found: boolean;
    /** Rows of the left column, the first row stands apart from the others. */
    leftRows: PropertyRow[];
    /** Rows of the right column, the first row stands apart from the others. */
    rightRows: PropertyRow[];
    /** Title of the section. */
    title?: string;
    /** Text shown instead of the rows, null to show the rows. */
    emptyText?: string | null;
    /** False to hide the connection status, the hint and the loading indicator (e.g. if a section above already shows them). */
    statusVisible?: boolean;
}>(), {
    title: 'Summary',
    emptyText: null,
    statusVisible: true
});

const titleId = useId();

const statusText: Record<HubStatus, string> = {
    connecting: 'Connecting…',
    connected: 'Live',
    reconnecting: 'Reconnecting…',
    disconnected: 'Disconnected'
};

const hint = computed(() => {
    if (props.statusVisible === false) {
        return null;
    }

    if (props.status === 'disconnected' || props.status === 'reconnecting') {
        return 'The Luna service cannot be reached, the values are not updated.';
    }

    if (props.received && props.found === false) {
        return 'The device is no longer in the DER configuration.';
    }

    return null;
});

const columns = computed(() => [props.leftRows, props.rightRows]);

const indicators: Record<ValueIndicator, { icon: string; label: string }> = {
    ok: { icon: 'bi-check-square-fill value-indicator-ok', label: 'OK' },
    fault: { icon: 'bi-x-square-fill value-indicator-fault', label: 'Not OK' }
};
</script>

<template>
    <section :aria-labelledby="titleId">
        <div class="dash-section-header">
            <h2 :id="titleId" class="dash-section-title">{{ title }}</h2>
            <span v-if="statusVisible" class="dash-status" :class="`dash-status-${status}`">
                <span class="dash-status-dot" aria-hidden="true"></span>{{ statusText[status] }}
            </span>
        </div>

        <p v-if="hint" class="dash-hint">
            <i class="bi bi-exclamation-triangle me-1" aria-hidden="true"></i>{{ hint }}
        </p>

        <div v-if="statusVisible && received === false && hint === null" class="d-flex align-items-center gap-2 small">
            <span class="spinner-border spinner-inline" role="status" aria-hidden="true"></span>
            Loading...
        </div>

        <p v-else-if="received && found && emptyText !== null" class="pm-empty">{{ emptyText }}</p>

        <div v-else-if="received && found" class="pm-columns">
            <dl v-for="(rows, column) in columns" :key="column" class="pm-values">
                <template v-for="(row, index) in rows" :key="row.name">
                    <dt :class="{ 'pm-values-first': index === 0 }">{{ row.name }}</dt>
                    <dd :class="{ 'pm-values-first': index === 0 }">
                        {{ row.value }}
                        <i v-if="row.indicator" class="bi value-indicator" :class="indicators[row.indicator].icon"
                           role="img" :aria-label="indicators[row.indicator].label" :title="indicators[row.indicator].label"></i>
                    </dd>
                </template>
            </dl>
        </div>
    </section>
</template>
