<script setup lang="ts">
import { computed } from 'vue';
import type { PpcAlarmFilter } from '../types/alarm';
import {
    conditionOptions,
    defaultAlarmFilter,
    severityOptions,
    stateOptions,
    triggeredOptions
} from '../utils/alarmList';

const props = defineProps<{
    /** True while the alarms are loaded. */
    loading: boolean;
    /** Number of loaded alarms, null before the first load or after a failed load. */
    alarmCount: number | null;
    /** Time the alarms were loaded, null before the first load. */
    loadedAt: Date | null;
}>();

defineEmits<{
    refresh: [];
}>();

const filter = defineModel<PpcAlarmFilter>({ required: true });

const timeFormat = new Intl.DateTimeFormat(undefined, { hour: '2-digit', minute: '2-digit', second: '2-digit' });

/** Two-way binding of one criterion of the filter, every change replaces the filter object. */
function criterion<K extends keyof PpcAlarmFilter>(key: K) {
    return computed<PpcAlarmFilter[K]>({
        get: () => filter.value[key],
        set: (value) => {
            filter.value = { ...filter.value, [key]: value };
        }
    });
}

const severityLevel = criterion('severityLevel');
const triggeredWithinHours = criterion('triggeredWithinHours');
const alarmProcessCondition = criterion('alarmProcessCondition');
const alarmSequenceState = criterion('alarmSequenceState');
const deviceName = criterion('deviceName');

const isDefault = computed(() => JSON.stringify(filter.value) === JSON.stringify(defaultAlarmFilter()));

const status = computed(() => {
    if (props.alarmCount === null || props.loadedAt === null) {
        return '';
    }

    const count = props.alarmCount === 1 ? '1 alarm' : `${props.alarmCount} alarms`;

    return `${count} · loaded ${timeFormat.format(props.loadedAt)}`;
});

function clearAll(): void {
    filter.value = defaultAlarmFilter();
}
</script>

<template>
    <section class="alarm-filters" aria-labelledby="alarm-filters-title">
        <h2 id="alarm-filters-title" class="alarm-filters-title">Filters</h2>

        <div class="alarm-filters-actions">
            <button class="btn btn-sm btn-outline-light" type="button" :disabled="isDefault" @click="clearAll">
                Clear All
            </button>
            <button class="btn btn-sm btn-secondary" type="button" :disabled="loading" title="Load the alarms again" @click="$emit('refresh')">
                <span v-if="loading" class="spinner-border spinner-inline me-1" role="status" aria-hidden="true"></span>
                <i v-else class="bi bi-arrow-clockwise me-1" aria-hidden="true"></i>Refresh
            </button>
        </div>

        <div class="alarm-filter">
            <label class="om-label" for="alarm-filter-severity">Severity</label>
            <select id="alarm-filter-severity" v-model="severityLevel" class="form-select form-select-sm">
                <option v-for="option in severityOptions" :key="option.label" :value="option.value">{{ option.label }}</option>
            </select>
        </div>

        <div class="alarm-filter">
            <label class="om-label" for="alarm-filter-triggered">Triggered</label>
            <select id="alarm-filter-triggered" v-model="triggeredWithinHours" class="form-select form-select-sm">
                <option v-for="option in triggeredOptions" :key="option.label" :value="option.value">{{ option.label }}</option>
            </select>
        </div>

        <div class="alarm-filter">
            <label class="om-label" for="alarm-filter-condition">Condition</label>
            <select id="alarm-filter-condition" v-model="alarmProcessCondition" class="form-select form-select-sm">
                <option v-for="option in conditionOptions" :key="option.label" :value="option.value">{{ option.label }}</option>
            </select>
        </div>

        <div class="alarm-filter">
            <label class="om-label" for="alarm-filter-state">State</label>
            <select id="alarm-filter-state" v-model="alarmSequenceState" class="form-select form-select-sm">
                <option v-for="option in stateOptions" :key="option.label" :value="option.value">{{ option.label }}</option>
            </select>
        </div>

        <div class="alarm-filter">
            <label class="om-label" for="alarm-filter-device">Device</label>
            <input id="alarm-filter-device"
                   v-model="deviceName"
                   class="form-control form-control-sm"
                   type="search"
                   placeholder="Device name"
                   autocomplete="off" />
        </div>

        <p v-if="status" class="om-hint alarm-filters-status mb-0" aria-live="polite">{{ status }}</p>
    </section>
</template>
