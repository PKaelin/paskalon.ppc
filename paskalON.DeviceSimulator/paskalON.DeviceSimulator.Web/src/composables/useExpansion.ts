import { inject, provide, ref, type InjectionKey, type Ref } from 'vue';

/**
 * Expand/collapse state of the accordion tree.
 *
 * The accordions are driven by Vue (not by the Bootstrap collapse plugin) so that the
 * search can force sections open without fighting the plugin for the DOM.
 */
export interface ExpansionApi {
    isOpen: (id: string) => boolean;
    toggle: (id: string) => void;
    open: (ids: string[]) => void;
    close: (ids: string[]) => void;
    expandAll: (ids: string[]) => void;
    collapseAll: () => void;
    /** While a search is running every visible node is expanded automatically. */
    searchActive: Ref<boolean>;
}

export const expansionKey: InjectionKey<ExpansionApi> = Symbol('expansion');

export function createExpansion(): ExpansionApi {
    const openIds = ref(new Set<string>());
    const searchActive = ref(false);
    /** Nodes the user collapsed manually while a search is running. */
    const closedDuringSearch = ref(new Set<string>());

    function isOpen(id: string): boolean {
        if (searchActive.value) {
            return !closedDuringSearch.value.has(id);
        }

        return openIds.value.has(id);
    }

    function toggle(id: string): void {
        if (searchActive.value) {
            const next = new Set(closedDuringSearch.value);

            if (next.has(id)) {
                next.delete(id);
            } else {
                next.add(id);
            }

            closedDuringSearch.value = next;
            return;
        }

        const next = new Set(openIds.value);

        if (next.has(id)) {
            next.delete(id);
        } else {
            next.add(id);
        }

        openIds.value = next;
    }

    function open(ids: string[]): void {
        const next = new Set(openIds.value);
        ids.forEach((id) => next.add(id));
        openIds.value = next;
        closedDuringSearch.value = new Set();
    }

    function close(ids: string[]): void {
        const next = new Set(openIds.value);
        ids.forEach((id) => next.delete(id));
        openIds.value = next;
    }

    function expandAll(ids: string[]): void {
        openIds.value = new Set(ids);
        closedDuringSearch.value = new Set();
    }

    function collapseAll(): void {
        openIds.value = new Set();
        closedDuringSearch.value = new Set();
    }

    const api: ExpansionApi = {
        isOpen,
        toggle,
        open,
        close,
        expandAll,
        collapseAll,
        searchActive
    };

    provide(expansionKey, api);

    return api;
}

export function useExpansion(): ExpansionApi {
    const api = inject(expansionKey);

    if (!api) {
        throw new Error('useExpansion() was called outside of the expansion provider.');
    }

    return api;
}
