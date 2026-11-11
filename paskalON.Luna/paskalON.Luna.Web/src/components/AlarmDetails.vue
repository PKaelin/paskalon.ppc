<script setup lang="ts">
import type { PpcAlarmDto } from '../types/alarm';
import { resolveAlarmText } from '../utils/alarmTexts';

defineProps<{
    /** The selected alarm, null if no alarm is selected. */
    alarm: PpcAlarmDto | null;
    /** Number of alarms shown in the alarm list that can be acknowledged. */
    acknowledgeableCount: number;
    /** True while an acknowledgement is sent, the buttons are disabled. */
    acknowledging: boolean;
}>();

const emit = defineEmits<{
    /** Acknowledge the alarms shown in the alarm list that can be acknowledged. */
    acknowledgeFiltered: [];
    /** Acknowledge the selected alarm. */
    acknowledgeSelected: [];
}>();
</script>

<template>
    <section class="alarm-details" aria-label="Alarm details" aria-live="polite">
        <p v-if="alarm === null" class="om-hint mb-0">Select an alarm to see its details.</p>
        <dl v-else class="alarm-details-list">
            <dt>Device</dt>
            <dd>{{ alarm.deviceName ?? '—' }}</dd>
            <dt>Description</dt>
            <dd>{{ resolveAlarmText(alarm.ppcAlarmConfig.description) || '—' }}</dd>
            <dt>Possible Causes</dt>
            <dd>{{ resolveAlarmText(alarm.ppcAlarmConfig.possibleCauses) || '—' }}</dd>
        </dl>

        <div class="alarm-details-actions">
            <button type="button"
                    class="btn btn-sm btn-secondary"
                    :disabled="acknowledging || acknowledgeableCount === 0"
                    :title="acknowledgeableCount === 0
                        ? 'No alarm in the list can be acknowledged'
                        : `Acknowledge the ${acknowledgeableCount} alarms in the list that are not acknowledged`"
                    @click="emit('acknowledgeFiltered')">
                Acknowledge Filtered
            </button>
            <button type="button"
                    class="btn btn-sm"
                    :class="alarm === null ? 'btn-outline-light' : 'btn-secondary'"
                    :disabled="acknowledging || alarm === null || alarm.canBeAcknowledged === false"
                    :title="alarm === null
                        ? 'Select an alarm'
                        : alarm.canBeAcknowledged ? 'Acknowledge the selected alarm' : 'The selected alarm cannot be acknowledged'"
                    @click="emit('acknowledgeSelected')">
                Acknowledge Selected
            </button>
        </div>
    </section>
</template>
