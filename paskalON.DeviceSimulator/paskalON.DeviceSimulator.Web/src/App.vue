<script setup lang="ts">
    import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue';
    import AppHeader from './components/AppHeader.vue';
    import AppFooter from './components/AppFooter.vue';
    import SearchBox from './components/SearchBox.vue';
    import TreeSection from './components/TreeSection.vue';
    import type { DerDto } from './types/der';
    import { getDer } from './api/derApi';
    import { config } from './api/config';
    import { buildTree, collectIds, countDevices } from './utils/treeBuilder';
    import { filterTree } from './utils/treeFilter';
    import { createExpansion } from './composables/useExpansion';
    import { provideSearchTerm, useDebounced } from './composables/useSearchTerm';
    import { createNotifications } from './composables/useNotifications';
    import { useExpansionSync } from './composables/useExpansionSync';
    import { useLiveUpdates } from './composables/useLiveUpdates';

    const expansion = createExpansion();
    const { notices, notify, dismiss } = createNotifications();

    const der = ref<DerDto | null>(null);
    const loading = ref(true);
    const loadError = ref<string | null>(null);
    const lastLoaded = ref<Date | null>(null);
    const lastUpdate = ref<Date | null>(null);
    const searchInput = ref('');
    const searchTerm = useDebounced(searchInput, 150);
    provideSearchTerm(searchTerm);

    let refreshHandle: ReturnType<typeof setInterval> | undefined;
    let abortController: AbortController | undefined;
    let firstLoad = true;

    /** Complete tree built from the DerDto. */
    const tree = computed(() => (der.value ? buildTree(der.value) : []));

    /** Tree after applying the search filter. */
    const filtered = computed(() => filterTree(tree.value, searchTerm.value));

    const allIds = computed(() => collectIds(tree.value));

    /**
     * Reports every expanded unit / device to POST /v1/der/setexpanded. It watches the
     * FILTERED tree, so sections hidden by the search do not count as expanded.
     */
    const expansionSync = useExpansionSync(
        computed(() => filtered.value.nodes),
        expansion,
        (message) => notify(`Expanded sections could not be sent: ${message}`, 'warning', 6000)
    );

    /** Live values pushed by the web service over SignalR. */
    const live = useLiveUpdates({
        der,
        onStructureChanged: () => {
            // Either the configuration changed or the hub reconnected: reload and tell the
            // service again which sections are open.
            expansionSync.resend();
            void load();
        },
        onApplied: () => {
            lastUpdate.value = new Date();
        }
    });

    const liveBadge = computed(() => {
        switch (live.status.value) {
            case 'connected':
                return { text: 'Live', css: 'bg-success' };
            case 'connecting':
                return { text: 'Connecting...', css: 'bg-secondary' };
            case 'reconnecting':
                return { text: 'Reconnecting...', css: 'bg-warning text-dark' };
            case 'disabled':
                return { text: 'Live off', css: 'bg-dark' };
            default:
                return { text: 'Offline', css: 'bg-danger' };
        }
    });
    const deviceCount = computed(() => countDevices(tree.value));
    const visibleDeviceCount = computed(() => countDevices(filtered.value.nodes));

    const title = computed(() => der.value?.name ?? '');

    const subtitle = computed(() => {
        if (!der.value) {
            return '';
        }

        const groups = der.value.derGroups?.length ?? 0;
        return `${groups} group${groups === 1 ? '' : 's'} · ${deviceCount.value} devices`;
    });

    const searchSummary = computed(() => {
        if (searchTerm.value.trim().length === 0) {
            return '';
        }

        const sections = filtered.value.nodes.length;
        const matches = filtered.value.matchedIds.size;

        if (matches === 0) {
            return `No section matches "${searchTerm.value}".`;
        }

        return `${matches} matching section${matches === 1 ? '' : 's'} · ${visibleDeviceCount.value} device${visibleDeviceCount.value === 1 ? '' : 's'} in ${sections} top level section${sections === 1 ? '' : 's'}.`;
    });

    // While a search is running every remaining section is expanded automatically.
    watch(
        () => searchTerm.value.trim().length > 0,
        (active) => {
            expansion.searchActive.value = active;
        },
        { immediate: true }
    );

    // Title of the browser tab follows the DER name.
    watch(title, (value) => {
        document.title = value ? `${value} - paskalON DeviceSimulator` : 'paskalON DeviceSimulator';
    });

    async function load(showNotice = false): Promise<void> {
        abortController?.abort();
        abortController = new AbortController();

        loading.value = true;
        loadError.value = null;

        try {
            const result = await getDer(abortController.signal);
            der.value = result;
            lastLoaded.value = new Date();

            // Open the top level sections on the first load so the structure is visible.
            if (firstLoad) {
                expansion.open(buildTree(result).map((node) => node.id));
                firstLoad = false;
            }

            if (showNotice) {
                notify('Configuration reloaded.', 'success', 2500);
            }
        } catch (error) {
            if (error instanceof DOMException && error.name === 'AbortError') {
                return;
            }

            loadError.value = error instanceof Error ? error.message : String(error);
        } finally {
            loading.value = false;
        }
    }

    function expandAll(): void {
        expansion.expandAll(allIds.value);
    }

    function collapseAll(): void {
        expansion.collapseAll();
    }

    onMounted(async () => {
        await load();
        void live.start();

        const interval = config().autoRefreshSeconds;

        if (interval > 0) {
            refreshHandle = setInterval(() => void load(), interval * 1000);
        }
    });

    onBeforeUnmount(() => {
        if (refreshHandle) {
            clearInterval(refreshHandle);
        }

        abortController?.abort();
    });
</script>

<template>
    <AppHeader :title="title" :subtitle="subtitle" />

    <main>
        <!-- Toolbar with the search as you type box -->
        <div class="app-toolbar">
            <div class="container-fluid">
                <div class="row g-2 align-items-start">
                    <div class="col-12 col-lg-7">
                        <SearchBox v-model="searchInput" :result-text="searchSummary" />
                    </div>
                    <div class="col-12 col-lg-5 d-flex flex-wrap align-items-center gap-2 justify-content-lg-end">
                        <span class="badge" :class="liveBadge.css" :title="`SignalR hub: ${live.messageCount.value} message(s) received`">
                            <i class="bi bi-broadcast me-1" aria-hidden="true"></i>{{ liveBadge.text }}
                        </span>
                        <button class="btn btn-sm btn-outline-secondary" type="button" @click="expandAll">
                            <i class="bi bi-arrows-expand me-1" aria-hidden="true"></i>Expand all
                        </button>
                        <button class="btn btn-sm btn-outline-secondary" type="button" @click="collapseAll">
                            <i class="bi bi-arrows-collapse me-1" aria-hidden="true"></i>Collapse all
                        </button>
                        <button class="btn btn-sm btn-secondary"
                                type="button"
                                :disabled="loading"
                                @click="load(true)">
                            <span v-if="loading"
                                  class="spinner-border spinner-inline me-1"
                                  role="status"
                                  aria-hidden="true"></span>
                            <i v-else class="bi bi-arrow-clockwise me-1" aria-hidden="true"></i>
                            Refresh
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <div class="container-fluid">
            <!-- Notifications -->
            <div v-if="notices.length > 0" class="mb-3">
                <div v-for="notice in notices"
                     :key="notice.id"
                     class="alert alert-dismissible app-status-bar py-2"
                     :class="`alert-${notice.kind}`"
                     role="status">
                    {{ notice.message }}
                    <button type="button" class="btn-close" aria-label="Close" @click="dismiss(notice.id)"></button>
                </div>
            </div>

            <!-- Loading -->
            <div v-if="loading && !der" class="d-flex align-items-center gap-2 py-4">
                <span class="spinner-border" role="status" aria-hidden="true"></span>
                <span>Loading the DER configuration from the web service...</span>
            </div>

            <!-- Error -->
            <div v-else-if="loadError" class="alert alert-danger" role="alert">
                <h5 class="alert-heading">The DER configuration could not be loaded</h5>
                <p class="mb-2">{{ loadError }}</p>
                <p class="mb-2 small">
                    The application requests
                    <code>{{ config().apiBaseUrl }}{{ config().derEndpoint }}</code>.
                    In the development server and in the container this address is reverse
                    proxied to the DeviceSimulator web service (default
                    <code>http://localhost:45500</code>). Check that the service is running,
                    then press Refresh.
                </p>
                <button class="btn btn-sm btn-outline-light" type="button" @click="load(true)">
                    Try again
                </button>
            </div>

            <!-- Tree -->
            <template v-else-if="der">
                <div v-if="filtered.nodes.length === 0" class="alert alert-warning" role="alert">
                    Nothing matches "{{ searchTerm }}".
                </div>

                <TreeSection v-for="node in filtered.nodes"
                             :key="node.id"
                             :node="node"
                             :level="0"
                             :matched-ids="filtered.matchedIds" />

                <p v-if="lastLoaded" class="search-summary mt-3 mb-0">
                    Last loaded {{ lastLoaded.toLocaleTimeString() }}
                    <template v-if="lastUpdate">
                        &middot; Last live update {{ lastUpdate.toLocaleTimeString() }}
                    </template>
                    <template v-if="expansionSync.expandedUnits.value.length > 0 || expansionSync.expandedDevices.value.length > 0">
                        &middot; expanded {{ expansionSync.expandedUnits.value.length }} DER unit(s)
                        and {{ expansionSync.expandedDevices.value.length }} DER device(s)
                    </template>
                </p>
            </template>
        </div>
    </main>

    <AppFooter />
</template>
