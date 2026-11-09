<script setup lang="ts">
import { computed } from 'vue';
import { useAlarmSummary } from '../composables/useAlarmSummary';
import type { AlarmSummaryDto } from '../types/alarm';

/** Kind of event an indicator stands for; the shapes and colors follow COLORS.md. */
type EventKind = 'info' | 'warning' | 'error' | 'critical';

/** One event indicator shown as a shape. */
interface EventShape {
    /** Kind of event the shape stands for. */
    kind: EventKind;
    /** Name shown in the tooltip. */
    title: string;
    /** Unacknowledged alarms (sequence state Alarm) of the summary. */
    unacknowledged: (summary: AlarmSummaryDto) => number;
    /** Acknowledged alarms (sequence state Acknowledged) of the summary. */
    acknowledged: (summary: AlarmSummaryDto) => number;
    /** Vertical position of the count, lower in the triangle where it is wider. */
    countY: number;
}

/** State of a shape shown to the operator. */
interface EventShapeState extends EventShape {
    /** Active alarms: unacknowledged and acknowledged, null while unknown. */
    count: number | null;
    /** True while at least one alarm is not acknowledged yet. */
    isBlinking: boolean;
    /** Text of the tooltip. */
    tooltip: string;
}

/** Highest count shown as number, higher counts are shown as '99+' so that they fit into the shape. */
const maximumShownCount = 99;

const shapes: EventShape[] = [
    { kind: 'info', title: 'Info', unacknowledged: s => s.infoCount, acknowledged: s => s.acknowledgedInfoCount, countY: 12.5 },
    { kind: 'warning', title: 'Warning', unacknowledged: s => s.warningCount, acknowledged: s => s.acknowledgedWarningCount, countY: 16 },
    { kind: 'error', title: 'Error', unacknowledged: s => s.errorCount, acknowledged: s => s.acknowledgedErrorCount, countY: 12.5 },
    { kind: 'critical', title: 'Critical', unacknowledged: s => s.criticalCount, acknowledged: s => s.acknowledgedCriticalCount, countY: 12.5 }
];

const summary = useAlarmSummary();

/**
 * The state of every shape:
 * - Unknown (no summary): gray outline.
 * - No active alarms: outline in the event color.
 * - Active alarms: filled with the number of active alarms, blinking while at least one is not acknowledged.
 */
const states = computed<EventShapeState[]>(() => shapes.map(shape => {
    if (summary.value === null) {
        return { ...shape, count: null, isBlinking: false, tooltip: `${shape.title}: alarm state unknown` };
    }

    const unacknowledged = shape.unacknowledged(summary.value);
    const acknowledged = shape.acknowledged(summary.value);
    const count = unacknowledged + acknowledged;
    const tooltip = count === 0
        ? `${shape.title}: no active alarms`
        : `${shape.title}: ${count} active (${unacknowledged} unacknowledged, ${acknowledged} acknowledged)`;

    return { ...shape, count, isBlinking: unacknowledged > 0, tooltip };
}));

/** Text of the count, '99+' when it does not fit into the shape. */
function countText(count: number): string {
    return count > maximumShownCount ? `${maximumShownCount}+` : String(count);
}
</script>

<template>
    <div class="event-shapes">
        <div v-for="state in states"
             :key="state.kind"
             class="event-shape"
             :class="[
                 `event-shape-${state.kind}`,
                 {
                     'event-shape-unknown': state.count === null,
                     'event-shape-active': state.count !== null && state.count > 0,
                     'event-shape-blinking': state.isBlinking
                 }
             ]"
             role="img"
             :title="state.tooltip"
             :aria-label="state.tooltip">
            <svg viewBox="0 0 24 24" aria-hidden="true">
                <rect v-if="state.kind === 'info'" class="event-shape-body" x="2" y="2" width="20" height="20" rx="2" />
                <polygon v-else-if="state.kind === 'warning'" class="event-shape-body" points="12,1.5 23,22.5 1,22.5" stroke-linejoin="round" />
                <circle v-else-if="state.kind === 'error'" class="event-shape-body" cx="12" cy="12" r="11" />
                <!-- Octagon like a stop sign -->
                <polygon v-else class="event-shape-body"
                         points="7.79,1.84 16.21,1.84 22.16,7.79 22.16,16.21 16.21,22.16 7.79,22.16 1.84,16.21 1.84,7.79"
                         stroke-linejoin="round" />
                <text v-if="state.count !== null && state.count > 0"
                      class="event-shape-count"
                      :class="{ 'event-shape-count-small': state.count > maximumShownCount }"
                      x="12"
                      :y="state.countY">{{ countText(state.count) }}</text>
            </svg>
        </div>
    </div>
</template>
