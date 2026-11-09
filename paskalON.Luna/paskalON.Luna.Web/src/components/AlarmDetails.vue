<script setup lang="ts">
import type { PpcAlarmDto } from '../types/alarm';
import { resolveAlarmText } from '../utils/alarmTexts';

defineProps<{
    /** The selected alarm, null if no alarm is selected. */
    alarm: PpcAlarmDto | null;
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
    </section>
</template>
