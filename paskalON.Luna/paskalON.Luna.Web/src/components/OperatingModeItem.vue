<script setup lang="ts">
import { computed } from 'vue';
import type { OperatingModeStackDto } from '../types/operatingModes';
import { getAxis, getCategories, getTypeNames } from '../utils/stackType';

const props = defineProps<{
    mode: OperatingModeStackDto;
    /** Available and active operating modes use different colours. */
    variant: 'available' | 'active';
}>();

/** One badge per category, with the stack type names as hover information (e.g. "BaseReactive"). */
const badges = computed(() => getCategories(props.mode.type).map((category) => ({
    glyph: category.glyph,
    title: getTypeNames(props.mode.type, category).join(', ')
})));
const axis = computed(() => getAxis(props.mode.type));
</script>

<template>
    <li class="om-item" :class="[`om-item-${variant}`, `om-axis-${axis}`]">
        <span v-if="badges.length > 0" class="om-item-badges">
            <span v-for="badge in badges" :key="badge.glyph" class="om-item-badge" :title="badge.title">{{ badge.glyph }}</span>
        </span>
        <span class="om-item-name">{{ mode.operatingModeName }}</span>
    </li>
</template>
