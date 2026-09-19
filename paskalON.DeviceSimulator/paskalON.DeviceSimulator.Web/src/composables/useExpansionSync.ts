import { computed, onBeforeUnmount, watch, type ComputedRef, type Ref } from 'vue';
import { isDeviceNode, isUnitNode, type TreeNode } from '@/types/tree';
import { setExpanded, type SetExpandedRequest } from '@/api/derApi';
import { config } from '@/api/config';
import type { ExpansionApi } from './useExpansion';

export interface ExpansionSync {
    /** Names of the DerUnitDto sections that are currently expanded AND visible. */
    expandedUnits: ComputedRef<string[]>;
    /** Names of the device sections (PCS, BB, PV, Pm*) that are currently expanded AND visible. */
    expandedDevices: ComputedRef<string[]>;
    /** Last request that was successfully sent (for the UI / diagnostics). */
    lastRequest: ComputedRef<SetExpandedRequest>;
    /** Posts the current state again, e.g. after the hub reconnected. */
    resend: () => void;
}

/**
 * Watches the expand/collapse state and posts the complete set of expanded units and
 * devices to POST /v1/der/setexpanded whenever it changes.
 *
 * "Expanded" means what the user can actually see: a section only counts when it is open
 * AND every one of its ancestors is open AND it survived the search filter.
 */
export function useExpansionSync(
    tree: Ref<TreeNode[]> | ComputedRef<TreeNode[]>,
    expansion: ExpansionApi,
    onError?: (message: string) => void
): ExpansionSync {
    let abortController: AbortController | undefined;
    let debounceHandle: ReturnType<typeof setTimeout> | undefined;

    /** Walks the (filtered) tree and collects the visibly expanded unit/device names. */
    const visibleExpanded = computed(() => {
        const units: string[] = [];
        const devices: string[] = [];

        const walk = (nodes: TreeNode[]): void => {
            for (const node of nodes) {
                if (!expansion.isOpen(node.id)) {
                    // Children of a collapsed section are not visible.
                    continue;
                }

                if (isUnitNode(node)) {
                    units.push(node.name);
                } else if (isDeviceNode(node)) {
                    devices.push(node.name);
                }

                walk(node.children);
            }
        };

        walk(tree.value);

        // Distinct + stable order, so identical states produce identical payloads.
        return {
            expandedUnits: [...new Set(units)].sort(),
            expandedDevices: [...new Set(devices)].sort()
        } satisfies SetExpandedRequest;
    });

    const expandedUnits = computed(() => visibleExpanded.value.expandedUnits);
    const expandedDevices = computed(() => visibleExpanded.value.expandedDevices);
    const lastRequest = computed(() => visibleExpanded.value);

    function send(): void {
        if (!config().enableExpansionSync) {
            return;
        }

        abortController?.abort();
        abortController = new AbortController();

        void setExpanded(visibleExpanded.value, abortController.signal).catch((error) => {
            if (error instanceof DOMException && error.name === 'AbortError') {
                return;
            }

            onError?.(error instanceof Error ? error.message : String(error));
        });
    }

    watch(
        // Serialize so the watcher only fires when the SET really changed.
        () => JSON.stringify(visibleExpanded.value),
        () => {
            if (!config().enableExpansionSync) {
                return;
            }

            if (debounceHandle) {
                clearTimeout(debounceHandle);
            }

            debounceHandle = setTimeout(send, config().expansionSyncDebounceMs);
        },
        { immediate: true }
    );

    onBeforeUnmount(() => {
        if (debounceHandle) {
            clearTimeout(debounceHandle);
        }

        abortController?.abort();
    });

    return { expandedUnits, expandedDevices, lastRequest, resend: send };
}
