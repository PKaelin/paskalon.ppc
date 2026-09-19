<script setup lang="ts">
import { computed } from 'vue';
import { highlightParts } from '@/utils/treeFilter';
import { useSearchTerm } from '@/composables/useSearchTerm';

const props = defineProps<{ text: string }>();

const term = useSearchTerm();
const parts = computed(() => highlightParts(props.text, term.value));
</script>

<template>
    <span>
        <template v-for="(part, index) in parts" :key="index">
            <mark v-if="part.hit" class="search-hit">{{ part.text }}</mark>
            <template v-else>{{ part.text }}</template>
        </template>
    </span>
</template>
