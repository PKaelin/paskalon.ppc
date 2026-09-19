<script setup lang="ts">
import { computed } from 'vue';
import CollapseTransition from './CollapseTransition.vue';
import HighlightedText from './HighlightedText.vue';
import { useExpansion } from '@/composables/useExpansion';

const props = withDefaults(
    defineProps<{
        /** Stable id - the expand/collapse state is stored per id. */
        nodeId: string;
        title: string;
        /** Badge on the left, e.g. "Group" or "Battery unit". */
        typeLabel?: string;
        /** Bootstrap icon class. */
        icon?: string;
        /** Nesting level (0 = top) - controls the header shade. */
        level?: number;
        /** Small counter badge on the right, e.g. "3 circuits". */
        counter?: string;
        /** True when the title itself matched the search term. */
        matched?: boolean;
    }>(),
    { typeLabel: '', icon: '', level: 0, counter: '', matched: false }
);

const expansion = useExpansion();

const isOpen = computed(() => expansion.isOpen(props.nodeId));
const headerId = computed(() => `hdr-${props.nodeId.replace(/[^\w-]/g, '-')}`);
const panelId = computed(() => `pnl-${props.nodeId.replace(/[^\w-]/g, '-')}`);
const levelClass = computed(() => `accordion-level-${Math.min(props.level, 5)}`);
</script>

<template>
    <div class="accordion" :class="levelClass">
        <div class="accordion-item">
            <h2 class="accordion-header" :id="headerId">
                <button class="accordion-button"
                        :class="{ collapsed: !isOpen }"
                        type="button"
                        :aria-expanded="isOpen"
                        :aria-controls="panelId"
                        @click="expansion.toggle(nodeId)">
                    <span class="d-flex align-items-center flex-wrap gap-2 w-100 pe-2">
                        <i v-if="icon" class="bi" :class="icon" aria-hidden="true"></i>

                        <span class="text-truncate">
                            <HighlightedText :text="title" />
                        </span>

                        <span v-if="typeLabel"
                              class="badge bg-dark node-type-badge">{{ typeLabel }}</span>

                        <span v-if="matched"
                              class="badge bg-warning text-dark node-type-badge">match</span>

                        <span v-if="counter" class="ms-auto node-count-badge">{{ counter }}</span>
                    </span>
                </button>
            </h2>

            <CollapseTransition>
                <div v-show="isOpen" :id="panelId" role="region" :aria-labelledby="headerId">
                    <div class="accordion-body">
                        <slot />
                    </div>
                </div>
            </CollapseTransition>
        </div>
    </div>
</template>
