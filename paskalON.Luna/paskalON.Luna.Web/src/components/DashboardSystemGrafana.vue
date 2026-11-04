<script setup lang="ts">
import { ref } from 'vue';
import GrafanaDashboard from './GrafanaDashboard.vue';
import { config } from '../api/config';

const section = config().dashboardSection;

/** The configured dashboards, the default one is always selectable even if it is not listed. */
const dashboardNames = section.dashboards.includes(section.defaultDashboard)
    ? section.dashboards
    : [section.defaultDashboard, ...section.dashboards];

const selected = ref(section.defaultDashboard);
</script>

<template>
    <section class="dash-section dash-section-grafana" aria-labelledby="dash-dashboard-title">
        <div class="dash-section-header">
            <h2 id="dash-dashboard-title" class="dash-section-title">Dashboard</h2>
        </div>

        <select v-model="selected" class="form-select form-select-sm dash-grafana-select" aria-label="Grafana dashboard">
            <option v-for="name in dashboardNames" :key="name" :value="name">{{ name }}</option>
        </select>

        <GrafanaDashboard :name="selected" />
    </section>
</template>
