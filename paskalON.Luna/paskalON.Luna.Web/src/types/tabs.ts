import type { Component } from 'vue';

/** One entry of the side tab navigation. */
export interface TabDefinition {
    /** Unique identifier of the tab. */
    id: string;
    /** Text shown on the tab. */
    label: string;
    /** Bootstrap icon class (e.g. 'bi-speedometer2') - the only thing visible when collapsed. */
    icon: string;
    /** Content rendered in the main content area while the tab is active. */
    component: Component;
    /** When true the tab is not stacked with the others but pinned to the bottom. */
    pinnedBottom?: boolean;
}
