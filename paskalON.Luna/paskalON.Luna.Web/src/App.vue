<script setup lang="ts">
import { computed, markRaw, ref } from 'vue';
import AppHeader from './components/AppHeader.vue';
import AppFooter from './components/AppFooter.vue';
import SideTabs from './components/SideTabs.vue';
import DashboardView from './views/DashboardView.vue';
import OperatingModesView from './views/OperatingModesView.vue';
import SystemView from './views/SystemView.vue';
import AlarmsView from './views/AlarmsView.vue';
import SettingsView from './views/SettingsView.vue';
import type { TabDefinition } from './types/tabs';
import { createNotifications } from './composables/useNotifications';

const { notices, dismiss } = createNotifications();

/** The tabs of the side navigation. The first one is active at start up. */
const tabs: TabDefinition[] = [
    { id: 'dashboard', label: 'Dashboard', icon: 'bi-speedometer2', component: markRaw(DashboardView) },
    { id: 'operating-modes', label: 'Operating Modes', icon: 'bi-sliders', component: markRaw(OperatingModesView) },
    { id: 'system', label: 'System', icon: 'bi-diagram-3', component: markRaw(SystemView) },
    { id: 'alarms', label: 'Alarms', icon: 'bi-bell', component: markRaw(AlarmsView) },
    { id: 'settings', label: 'Settings', icon: 'bi-gear', component: markRaw(SettingsView), pinnedBottom: true }
];

const activeTabId = ref(tabs[0].id);

const activeTab = computed(() => tabs.find((tab) => tab.id === activeTabId.value) ?? tabs[0]);
</script>

<template>
    <AppHeader />

    <div class="app-body">
        <SideTabs v-model="activeTabId" :tabs="tabs" />

        <main class="app-content" role="tabpanel" :aria-label="activeTab.label">
            <component :is="activeTab.component" />
        </main>
    </div>

    <AppFooter />

    <!-- Notifications, e.g. the outcome of the system commands -->
    <div class="app-notices" aria-live="polite">
        <div v-for="notice in notices"
             :key="notice.id"
             class="alert alert-dismissible py-2 mb-2"
             :class="`alert-${notice.kind}`"
             role="status">
            {{ notice.message }}
            <button type="button" class="btn-close" aria-label="Close" @click="dismiss(notice.id)"></button>
        </div>
    </div>
</template>
