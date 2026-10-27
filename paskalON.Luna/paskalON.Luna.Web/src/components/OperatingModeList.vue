<script setup lang="ts">
import OperatingModeItem from './OperatingModeItem.vue';
import type { OperatingModeStackDto } from '../types/operatingModes';

defineProps<{
    title: string;
    modes: OperatingModeStackDto[];
    variant: 'available' | 'active';
    loading: boolean;
    error: string | null;
}>();

defineEmits<{
    retry: [];
}>();
</script>

<template>
    <section class="om-panel">
        <h2 class="om-heading">{{ title }}</h2>

        <div v-if="loading" class="d-flex align-items-center gap-2 small">
            <span class="spinner-border spinner-inline" role="status" aria-hidden="true"></span>
            Loading...
        </div>

        <div v-else-if="error" class="alert alert-danger py-2 small d-flex align-items-center justify-content-between gap-2" role="alert">
            <span>{{ error }}</span>
            <button class="btn btn-sm btn-outline-light" type="button" @click="$emit('retry')">Retry</button>
        </div>

        <p v-else-if="modes.length === 0" class="om-hint mb-0">No operating modes.</p>

        <ul v-else class="om-list" :class="`om-list-${variant}`">
            <OperatingModeItem v-for="mode in modes" :key="mode.operatingModeName" :mode="mode" :variant="variant" />
        </ul>
    </section>
</template>
