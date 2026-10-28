<script setup lang="ts">
/** Card of the dashboard showing one value with its unit, an optional caption below and an optional bar (default slot). */
defineProps<{
    label: string;
    /** The formatted value, '—' if not available. */
    value: string;
    unit?: string;
    /** Small text below the value, e.g. 'Producing'. The line is reserved so the bars of all cards align. */
    caption?: string;
}>();
</script>

<template>
    <div class="metric-card">
        <div class="metric-card-label">{{ label }}</div>
        <div class="metric-card-value">
            <span class="metric-card-number">{{ value }}</span>
            <span v-if="unit" class="metric-card-unit">{{ unit }}</span>
        </div>
        <div v-if="caption !== undefined" class="metric-card-caption">{{ caption }}</div>
        <slot></slot>
    </div>
</template>
