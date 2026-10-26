<script setup lang="ts">
import { computed, ref } from 'vue';
import type { TabDefinition } from '../types/tabs';

const props = defineProps<{
    /** All tabs, in display order. */
    tabs: TabDefinition[];
}>();

/** Id of the active tab. */
const activeId = defineModel<string>({ required: true });

/** True while only the icons are shown. */
const collapsed = ref(false);

/** Stacked tabs first, followed by the tabs pinned to the bottom. */
const orderedTabs = computed(() => [
    ...props.tabs.filter((tab) => tab.pinnedBottom !== true),
    ...props.tabs.filter((tab) => tab.pinnedBottom === true)
]);

/** The first pinned tab is pushed to the bottom (margin-top: auto), the rest follow it. */
const firstBottomId = computed(() => orderedTabs.value.find((tab) => tab.pinnedBottom === true)?.id);

function toggle(): void {
    collapsed.value = !collapsed.value;
}
</script>

<template>
    <nav class="side-tabs" :class="{ collapsed }" aria-label="Main navigation">
        <button class="side-tab side-tabs-toggle"
                type="button"
                :title="collapsed ? 'Show tabs' : 'Hide tabs'"
                :aria-expanded="!collapsed"
                @click="toggle">
            <i class="bi" :class="collapsed ? 'bi-chevron-double-right' : 'bi-chevron-double-left'" aria-hidden="true"></i>
        </button>

        <div class="side-tabs-list" role="tablist" aria-orientation="vertical">
            <button v-for="tab in orderedTabs"
                    :key="tab.id"
                    class="side-tab"
                    :class="{ active: tab.id === activeId, 'side-tab-bottom': tab.id === firstBottomId }"
                    type="button"
                    role="tab"
                    :aria-selected="tab.id === activeId"
                    :title="tab.label"
                    @click="activeId = tab.id">
                <i class="bi" :class="tab.icon" aria-hidden="true"></i>
                <span class="side-tab-label">{{ tab.label }}</span>
            </button>
        </div>
    </nav>
</template>
