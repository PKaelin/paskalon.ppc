<script setup lang="ts">
/** Kind of event an indicator stands for; the shapes and colors follow COLORS.md. */
type EventKind = 'info' | 'warning' | 'error';

/** One event indicator shown as a shape. */
interface EventShape {
    /** Kind of event the shape stands for. */
    kind: EventKind;
    /** Name shown in the tooltip. */
    title: string;
}

const shapes: EventShape[] = [
    { kind: 'info', title: 'Info' },
    { kind: 'warning', title: 'Warning' },
    { kind: 'error', title: 'Error' }
];
</script>

<template>
    <div class="event-shapes">
        <div v-for="shape in shapes"
             :key="shape.kind"
             class="event-shape"
             :class="`event-shape-${shape.kind}`"
             role="img"
             :title="shape.title"
             :aria-label="shape.title">
            <svg viewBox="0 0 24 24" aria-hidden="true">
                <rect v-if="shape.kind === 'info'" x="2" y="2" width="20" height="20" rx="2" />
                <polygon v-else-if="shape.kind === 'warning'" points="12,1.5 23,22.5 1,22.5" stroke-linejoin="round" />
                <circle v-else cx="12" cy="12" r="11" />
            </svg>
        </div>
    </div>
</template>
