<script setup lang="ts">
import { computed } from 'vue';

/**
 * Horizontal bar that fills from 0 to the value. With a negative lower limit 0 lies inside the track
 * (marked with a tick) and negative values fill to the left, positive values to the right.
 */
const props = defineProps<{
    /** The value, null shows an empty track. */
    value: number | null;
    /** Lower limit of the track (left end). */
    min: number;
    /** Upper limit of the track (right end). */
    max: number;
    /** Colour of the fill. */
    color: string;
    /** Accessible name of the bar. */
    label: string;
    /** Optional small text above the left end of the bar, e.g. the formatted lower limit. */
    minLabel?: string;
    /** Optional small text above the right end of the bar, e.g. the formatted upper limit. */
    maxLabel?: string;
}>();

const hasLimitLabels = computed(() => props.minLabel !== undefined || props.maxLabel !== undefined);

function toPercent(value: number): number {
    const range = props.max - props.min;

    if (range <= 0) {
        return 0;
    }

    return Math.min(100, Math.max(0, ((value - props.min) / range) * 100));
}

const zeroPercent = computed(() => toPercent(0));

const hasZeroTick = computed(() => props.min < 0 && props.max > 0);

const fill = computed(() => {
    if (props.value === null) {
        return null;
    }

    const valuePercent = toPercent(props.value);

    return {
        left: `${Math.min(zeroPercent.value, valuePercent)}%`,
        width: `${Math.abs(valuePercent - zeroPercent.value)}%`,
        backgroundColor: props.color
    };
});
</script>

<template>
    <div v-if="hasLimitLabels" class="deviation-bar-limits">
        <span>{{ minLabel }}</span>
        <span>{{ maxLabel }}</span>
    </div>
    <div class="deviation-bar"
         role="meter"
         :aria-label="label"
         :aria-valuemin="min"
         :aria-valuemax="max"
         :aria-valuenow="value ?? undefined">
        <div v-if="fill" class="deviation-bar-fill" :style="fill"></div>
        <div v-if="hasZeroTick" class="deviation-bar-zero" :style="{ left: `${zeroPercent}%` }"></div>
    </div>
</template>
