<script lang="ts">
import type { AlarmSort, AlarmSortKey } from '../utils/alarmList';

/** Keys of the columns of the alarm list. */
type ColumnKey = 'severity' | 'triggered' | 'acknowledged' | 'state' | 'device' | 'name' | 'lastChange' | 'acknowledgedBy';

/** Column widths in pixels set by the user, null while the columns fit the content area. Kept while the tab is switched. */
let savedWidths: Record<ColumnKey, number> | null = null;

/** Order of the list chosen by the user. Kept while the tab is switched. */
let savedSort: AlarmSort | null = null;

/** One column of the alarm list. */
interface AlarmColumn {
    key: ColumnKey;
    title: string;
    /** Share of the content area when the columns fit it. */
    weight: number;
    /** Narrowest width in pixels. */
    minWidth: number;
    /** Order of the column, null if the list cannot be sorted by it. */
    sortKey: AlarmSortKey | null;
    /** Direction of the first click on the column header. */
    firstDirection: 'asc' | 'desc';
}

const columns: AlarmColumn[] = [
    { key: 'severity', title: 'Severity', weight: 0.9, minWidth: 72, sortKey: 'severity', firstDirection: 'desc' },
    { key: 'triggered', title: 'Triggered', weight: 1.4, minWidth: 96, sortKey: 'triggered', firstDirection: 'desc' },
    { key: 'acknowledged', title: 'Acknowledged', weight: 1.4, minWidth: 96, sortKey: 'acknowledged', firstDirection: 'desc' },
    { key: 'state', title: 'State', weight: 1, minWidth: 72, sortKey: 'state', firstDirection: 'asc' },
    { key: 'device', title: 'Device', weight: 1.3, minWidth: 72, sortKey: 'device', firstDirection: 'asc' },
    { key: 'name', title: 'Name', weight: 2.4, minWidth: 96, sortKey: null, firstDirection: 'asc' },
    { key: 'lastChange', title: 'Last Change', weight: 1.4, minWidth: 96, sortKey: 'lastChange', firstDirection: 'desc' },
    { key: 'acknowledgedBy', title: 'Acknowledged By', weight: 1.1, minWidth: 72, sortKey: null, firstDirection: 'asc' }
];

/** Step of the arrow keys on a column border in pixels. */
const KeyboardStep = 16;
</script>

<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref, useTemplateRef } from 'vue';
import type { PpcAlarmDto } from '../types/alarm';
import {
    defaultAlarmSort,
    formatAcknowledgement,
    formatAlarmTime,
    severityClasses,
    severityLabels,
    sortAlarms,
    stateLabels
} from '../utils/alarmList';
import { resolveAlarmText } from '../utils/alarmTexts';

const props = defineProps<{
    alarms: PpcAlarmDto[];
    loading: boolean;
    error: string | null;
}>();

defineEmits<{
    retry: [];
}>();

/** Id of the selected alarm, null if no alarm is selected. Only one alarm can be selected. */
const selectedId = defineModel<number | null>('selectedId', { required: true });

const scrollArea = useTemplateRef<HTMLDivElement>('scrollArea');

const tableBody = useTemplateRef<HTMLTableSectionElement>('tableBody');

const sort = ref<AlarmSort>(savedSort ?? defaultAlarmSort);

const customWidths = ref<Record<ColumnKey, number> | null>(savedWidths);

/** Inner width of the scroll area, the columns fit it until the user resizes one. */
const availableWidth = ref(0);

/** Column, pointer position and column width when a border drag started, null while not dragging. */
let drag: { key: ColumnKey; startX: number; startWidth: number } | null = null;

let resizeObserver: ResizeObserver | null = null;

const sortedAlarms = computed(() => sortAlarms(props.alarms, sort.value));

/** Widths of the columns: set by the user, or shared out over the content area by the weights. */
const widths = computed<Record<ColumnKey, number>>(() => {
    if (customWidths.value !== null) {
        return customWidths.value;
    }

    const totalWeight = columns.reduce((sum, column) => sum + column.weight, 0);
    const fitted = {} as Record<ColumnKey, number>;

    for (const column of columns) {
        fitted[column.key] = Math.max(column.minWidth, Math.floor(availableWidth.value * column.weight / totalWeight));
    }

    return fitted;
});

const tableWidth = computed(() => columns.reduce((sum, column) => sum + widths.value[column.key], 0));

/** Row that takes the focus when tabbing into the list: the selected one, otherwise the first. */
const focusableId = computed(() => (
    sortedAlarms.value.some((alarm) => alarm.id === selectedId.value) ? selectedId.value : sortedAlarms.value[0]?.id ?? null
));

function ariaSort(column: AlarmColumn): 'ascending' | 'descending' | 'none' | undefined {
    if (column.sortKey === null) {
        return undefined;
    }

    if (column.sortKey !== sort.value.key) {
        return 'none';
    }

    return sort.value.direction === 'asc' ? 'ascending' : 'descending';
}

/** Sorts by the column, a second click on the same column reverses the order. */
function toggleSort(column: AlarmColumn): void {
    if (column.sortKey === null) {
        return;
    }

    const direction = column.sortKey === sort.value.key
        ? (sort.value.direction === 'asc' ? 'desc' : 'asc')
        : column.firstDirection;

    sort.value = { key: column.sortKey, direction };
    savedSort = sort.value;
}

function resizeColumn(column: AlarmColumn, width: number): void {
    const current = customWidths.value ?? { ...widths.value };

    customWidths.value = { ...current, [column.key]: Math.round(Math.max(column.minWidth, width)) };
    savedWidths = customWidths.value;
}

/** The columns fit the content area again. */
function fitColumns(): void {
    customWidths.value = null;
    savedWidths = null;
}

function onBorderPointerDown(event: PointerEvent, column: AlarmColumn): void {
    if (event.button !== 0) {
        return;
    }

    const border = event.currentTarget as HTMLElement;

    // No text selection and no sorting while dragging, the captured pointer keeps the drag alive outside the border.
    event.preventDefault();
    event.stopPropagation();
    border.setPointerCapture(event.pointerId);
    border.focus();
    drag = { key: column.key, startX: event.clientX, startWidth: widths.value[column.key] };
}

function onBorderPointerMove(event: PointerEvent, column: AlarmColumn): void {
    if (drag === null || drag.key !== column.key) {
        return;
    }

    resizeColumn(column, drag.startWidth + event.clientX - drag.startX);
}

function onBorderPointerUp(): void {
    drag = null;
}

function onBorderKeyDown(event: KeyboardEvent, column: AlarmColumn): void {
    switch (event.key) {
        case 'ArrowLeft':
            resizeColumn(column, widths.value[column.key] - KeyboardStep);
            break;
        case 'ArrowRight':
            resizeColumn(column, widths.value[column.key] + KeyboardStep);
            break;
        case 'Home':
            fitColumns();
            break;
        default:
            return;
    }

    event.preventDefault();
}

function select(alarm: PpcAlarmDto): void {
    selectedId.value = alarm.id;
}

/** Moves the selection with the arrow keys, Home and End, the selected row takes the focus. */
async function onRowKeyDown(event: KeyboardEvent, index: number): Promise<void> {
    const lastIndex = sortedAlarms.value.length - 1;
    let target: number;

    switch (event.key) {
        case 'ArrowDown':
            target = Math.min(index + 1, lastIndex);
            break;
        case 'ArrowUp':
            target = Math.max(index - 1, 0);
            break;
        case 'Home':
            target = 0;
            break;
        case 'End':
            target = lastIndex;
            break;
        case 'Enter':
        case ' ':
            target = index;
            break;
        default:
            return;
    }

    event.preventDefault();
    select(sortedAlarms.value[target]);
    await nextTick();

    const row = tableBody.value?.rows.item(target);
    row?.focus({ preventScroll: true });
    row?.scrollIntoView({ block: 'nearest' });
}

onMounted(() => {
    const area = scrollArea.value;

    if (area === null) {
        return;
    }

    availableWidth.value = area.clientWidth;
    resizeObserver = new ResizeObserver(() => {
        availableWidth.value = area.clientWidth;
    });
    resizeObserver.observe(area);
});

onBeforeUnmount(() => resizeObserver?.disconnect());
</script>

<template>
    <section class="alarm-grid" aria-label="Alarm data">
        <div v-if="error" class="alert alert-danger py-2 small d-flex align-items-center justify-content-between gap-2 mb-2" role="alert">
            <span>{{ error }}</span>
            <button class="btn btn-sm btn-outline-light" type="button" @click="$emit('retry')">Retry</button>
        </div>

        <div ref="scrollArea" class="alarm-grid-scroll" :aria-busy="loading">
            <table class="alarm-table" role="grid" aria-label="Alarms" :style="{ width: `${tableWidth}px` }">
                <colgroup>
                    <col v-for="column in columns" :key="column.key" :style="{ width: `${widths[column.key]}px` }" />
                </colgroup>
                <thead>
                    <tr>
                        <th v-for="column in columns" :key="column.key" scope="col" :aria-sort="ariaSort(column)">
                            <button v-if="column.sortKey !== null"
                                    class="alarm-sort"
                                    type="button"
                                    :title="`Sort by ${column.title}`"
                                    @click="toggleSort(column)">
                                <span class="alarm-cell-text">{{ column.title }}</span>
                                <i class="bi alarm-sort-icon"
                                   :class="column.sortKey === sort.key
                                       ? (sort.direction === 'asc' ? 'bi-caret-up-fill' : 'bi-caret-down-fill')
                                       : 'bi-caret-down alarm-sort-icon-inactive'"
                                   aria-hidden="true"></i>
                            </button>
                            <span v-else class="alarm-header-text alarm-cell-text">{{ column.title }}</span>
                            <!-- Drag to resize, double-click to fit all columns into the content area again. -->
                            <span class="alarm-column-border"
                                  role="separator"
                                  aria-orientation="vertical"
                                  tabindex="0"
                                  :aria-label="`Resize column ${column.title}`"
                                  :aria-valuenow="widths[column.key]"
                                  :aria-valuemin="column.minWidth"
                                  title="Drag to resize, double-click to fit all columns"
                                  @pointerdown="onBorderPointerDown($event, column)"
                                  @pointermove="onBorderPointerMove($event, column)"
                                  @pointerup="onBorderPointerUp"
                                  @pointercancel="onBorderPointerUp"
                                  @dblclick="fitColumns"
                                  @keydown="onBorderKeyDown($event, column)"></span>
                        </th>
                    </tr>
                </thead>
                <tbody ref="tableBody">
                    <tr v-for="(alarm, index) in sortedAlarms"
                        :key="alarm.id"
                        class="alarm-row"
                        :class="{ active: alarm.id === selectedId }"
                        :aria-selected="alarm.id === selectedId"
                        :tabindex="alarm.id === focusableId ? 0 : -1"
                        @click="select(alarm)"
                        @keydown="onRowKeyDown($event, index)">
                        <td>
                            <span class="alarm-severity" :class="severityClasses[alarm.ppcAlarmConfig.severityLevel]">
                                {{ severityLabels[alarm.ppcAlarmConfig.severityLevel] }}
                            </span>
                        </td>
                        <td :title="formatAlarmTime(alarm.createdOn)">{{ formatAlarmTime(alarm.createdOn) }}</td>
                        <td :title="formatAcknowledgement(alarm)">{{ formatAcknowledgement(alarm) }}</td>
                        <td>{{ stateLabels[alarm.alarmSequenceState] }}</td>
                        <td :title="alarm.deviceName ?? 'No device'">{{ alarm.deviceName ?? '—' }}</td>
                        <td :title="resolveAlarmText(alarm.ppcAlarmConfig.name)">{{ resolveAlarmText(alarm.ppcAlarmConfig.name) }}</td>
                        <td :title="formatAlarmTime(alarm.changedDate)">{{ formatAlarmTime(alarm.changedDate) }}</td>
                        <td :title="alarm.acknowledgedBy ?? 'Not acknowledged'">{{ alarm.acknowledgedBy ?? '—' }}</td>
                    </tr>
                </tbody>
            </table>

            <p v-if="sortedAlarms.length === 0 && loading" class="alarm-grid-message">
                <span class="spinner-border spinner-inline me-2" role="status" aria-hidden="true"></span>Loading alarms...
            </p>
            <p v-else-if="sortedAlarms.length === 0 && error === null" class="alarm-grid-message">No alarms match the filters.</p>
        </div>

        <div v-if="loading && sortedAlarms.length > 0" class="alarm-grid-overlay" aria-hidden="true">
            <span class="spinner-border"></span>
        </div>
    </section>
</template>
