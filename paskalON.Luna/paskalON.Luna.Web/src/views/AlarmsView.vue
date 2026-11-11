<script lang="ts">
import { defaultAlarmFilter } from '../utils/alarmList';
import type { PpcAlarmFilter } from '../types/alarm';

/** Filter of the alarm list. Kept while the tab is switched. */
let savedFilter: PpcAlarmFilter = defaultAlarmFilter();

/** Delay after the last typed character of the device filter before the alarms are loaded, in milliseconds. */
const DeviceFilterDelay = 300;
</script>

<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue';
import AlarmDetails from '../components/AlarmDetails.vue';
import AlarmFilters from '../components/AlarmFilters.vue';
import AlarmGrid from '../components/AlarmGrid.vue';
import ActionDialog, { type DialogAction } from '../components/ActionDialog.vue';
import { acknowledgeAlarms, getPpcAlarms } from '../api/alarmApi';
import { useNotifications } from '../composables/useNotifications';
import type { PpcAlarmDto } from '../types/alarm';

const { notify } = useNotifications();

const filter = ref<PpcAlarmFilter>(savedFilter);

const alarms = ref<PpcAlarmDto[]>([]);

const loading = ref(false);

const error = ref<string | null>(null);

const loadedAt = ref<Date | null>(null);

const selectedId = ref<number | null>(null);

/** Cancels the running load, a newer load replaces it. */
let abortController: AbortController | null = null;

let deviceFilterTimer: ReturnType<typeof setTimeout> | undefined;

const selectedAlarm = computed(() => alarms.value.find((alarm) => alarm.id === selectedId.value) ?? null);

/** The alarms shown in the list that can be acknowledged (acknowledgeable and not acknowledged yet). */
const acknowledgeableAlarms = computed(() => alarms.value.filter((alarm) => alarm.canBeAcknowledged));

/** True while an acknowledgement is sent. */
const acknowledging = ref(false);

/** True while the confirmation of "Acknowledge Filtered" is shown. */
const isConfirmOpen = ref(false);

/** The alarm ids of the confirmation, taken when it opened so that a reload meanwhile does not change them. */
const pendingAlarmIds = ref<number[]>([]);

const confirmActions: DialogAction[] = [
    { id: 'acknowledge', label: 'Acknowledge', variant: 'success' },
    { id: 'cancel', label: 'Cancel', variant: 'outline-light' }
];

const confirmTitle = computed(() => `Acknowledge ${pendingAlarmIds.value.length} alarm${pendingAlarmIds.value.length === 1 ? '' : 's'}?`);

/** Asks to acknowledge the alarms shown in the list that can be acknowledged. */
function onAcknowledgeFiltered(): void {
    pendingAlarmIds.value = acknowledgeableAlarms.value.map((alarm) => alarm.id);

    if (pendingAlarmIds.value.length > 0) {
        isConfirmOpen.value = true;
    }
}

function onConfirm(action: string): void {
    if (action === 'acknowledge') {
        void acknowledge(pendingAlarmIds.value);
    }
}

/** Acknowledges the selected alarm without confirmation. */
function onAcknowledgeSelected(): void {
    if (selectedAlarm.value?.canBeAcknowledged === true) {
        void acknowledge([selectedAlarm.value.id]);
    }
}

/** Sends the acknowledgement through the Luna service and reloads the alarms to show the result. */
async function acknowledge(alarmIds: number[]): Promise<void> {
    acknowledging.value = true;

    try {
        const acknowledgedCount = await acknowledgeAlarms(alarmIds);
        notify(`${acknowledgedCount} alarm${acknowledgedCount === 1 ? '' : 's'} acknowledged.`, 'success');
    } catch (exception) {
        notify(exception instanceof Error ? exception.message : String(exception), 'danger');
    } finally {
        acknowledging.value = false;
    }

    await loadAlarms();
}

/** Loads the alarms of the current filter. The selection is kept if the alarm is still in the list. */
async function loadAlarms(): Promise<void> {
    clearTimeout(deviceFilterTimer);
    abortController?.abort();

    const controller = new AbortController();
    abortController = controller;
    loading.value = true;
    error.value = null;

    try {
        alarms.value = await getPpcAlarms(filter.value, controller.signal);
        loadedAt.value = new Date();
    } catch (exception) {
        if (exception instanceof DOMException && exception.name === 'AbortError') {
            return;
        }

        alarms.value = [];
        loadedAt.value = null;
        error.value = exception instanceof Error ? exception.message : String(exception);
    } finally {
        if (abortController === controller) {
            loading.value = false;
            abortController = null;
        }
    }

    if (alarms.value.some((alarm) => alarm.id === selectedId.value) === false) {
        selectedId.value = null;
    }
}

/** Every filter change loads the alarms, typing in the device filter only after a short pause. */
watch(filter, (current, previous) => {
    savedFilter = current;

    const isDeviceNameOnly = current.deviceName !== previous.deviceName
        && current.triggeredWithinHours === previous.triggeredWithinHours
        && current.severityLevel === previous.severityLevel
        && current.alarmSequenceState === previous.alarmSequenceState;

    if (isDeviceNameOnly) {
        clearTimeout(deviceFilterTimer);
        deviceFilterTimer = setTimeout(() => void loadAlarms(), DeviceFilterDelay);

        return;
    }

    void loadAlarms();
});

onMounted(() => {
    void loadAlarms();
});

onBeforeUnmount(() => {
    clearTimeout(deviceFilterTimer);
    abortController?.abort();
});
</script>

<template>
    <div class="alarm-layout">
        <AlarmFilters v-model="filter"
                      class="alarm-area-filters"
                      :loading="loading"
                      :alarm-count="loadedAt === null ? null : alarms.length"
                      :loaded-at="loadedAt"
                      @refresh="loadAlarms" />

        <AlarmGrid v-model:selected-id="selectedId"
                   class="alarm-area-grid"
                   :alarms="alarms"
                   :loading="loading"
                   :error="error"
                   @retry="loadAlarms" />

        <AlarmDetails class="alarm-area-details"
                      :alarm="selectedAlarm"
                      :acknowledgeable-count="acknowledgeableAlarms.length"
                      :acknowledging="acknowledging"
                      @acknowledge-filtered="onAcknowledgeFiltered"
                      @acknowledge-selected="onAcknowledgeSelected" />

        <ActionDialog v-model="isConfirmOpen" :title="confirmTitle" :actions="confirmActions" @choose="onConfirm" />
    </div>
</template>
