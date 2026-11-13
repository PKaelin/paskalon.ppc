import { ref, type Ref } from 'vue';

/** The active tab of the side navigation. Shared by the whole app, so that any component can switch the tab. */
const activeTabId = ref('dashboard');

/** The id of the active tab of the side navigation. */
export function useActiveTab(): Ref<string> {
    return activeTabId;
}

/** Shows the tab with the id like a click on its side tab. */
export function showTab(tabId: string): void {
    activeTabId.value = tabId;
}
