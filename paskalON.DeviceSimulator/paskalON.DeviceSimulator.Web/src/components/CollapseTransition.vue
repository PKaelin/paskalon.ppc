<script setup lang="ts">
/**
 * Height based collapse animation (the Bootstrap collapse plugin is not used because
 * the open state is owned by Vue).
 */
function beforeEnter(element: Element): void {
    (element as HTMLElement).style.height = '0';
}

function enter(element: Element): void {
    const el = element as HTMLElement;
    el.style.height = `${el.scrollHeight}px`;
}

function afterEnter(element: Element): void {
    (element as HTMLElement).style.height = '';
}

function beforeLeave(element: Element): void {
    const el = element as HTMLElement;
    el.style.height = `${el.scrollHeight}px`;
    // Force a reflow so the transition to 0 actually runs.
    void el.offsetHeight;
}

function leave(element: Element): void {
    (element as HTMLElement).style.height = '0';
}
</script>

<template>
    <Transition name="collapse"
                @before-enter="beforeEnter"
                @enter="enter"
                @after-enter="afterEnter"
                @before-leave="beforeLeave"
                @leave="leave">
        <slot />
    </Transition>
</template>
