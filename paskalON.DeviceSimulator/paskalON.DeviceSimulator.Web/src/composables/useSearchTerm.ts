import { inject, provide, ref, watch, type InjectionKey, type Ref } from 'vue';

export const searchTermKey: InjectionKey<Ref<string>> = Symbol('searchTerm');

/** Provides the (debounced) search term to every node of the tree for highlighting. */
export function provideSearchTerm(term: Ref<string>): void {
    provide(searchTermKey, term);
}

export function useSearchTerm(): Ref<string> {
    return inject(searchTermKey, ref(''));
}

/**
 * Search as you type: returns a ref that follows `source` with a small delay so that
 * fast typing does not re-filter the tree on every single keystroke.
 */
export function useDebounced(source: Ref<string>, delayMs = 150): Ref<string> {
    const debounced = ref(source.value);
    let handle: ReturnType<typeof setTimeout> | undefined;

    watch(source, (value) => {
        if (handle) {
            clearTimeout(handle);
        }

        // An empty box resets immediately - that feels snappier when clearing.
        if (value.length === 0) {
            debounced.value = '';
            return;
        }

        handle = setTimeout(() => {
            debounced.value = value;
        }, delayMs);
    });

    return debounced;
}
