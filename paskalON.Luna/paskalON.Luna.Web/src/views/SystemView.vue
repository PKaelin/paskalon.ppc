<script lang="ts">
/** Width of the object tree in pixels set with the splitter, null for the default width. Kept while the tab is switched. */
let savedTreeWidth: number | null = null;
</script>

<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from 'vue';
import DerTree from '../components/DerTree.vue';
import PmSystemContent from '../components/PmSystemContent.vue';
import { getDer } from '../api/deviceApi';
import { buildDerTree, findDerNode, type DerTreeNode } from '../utils/derTree';

/** Narrowest object tree in rem. */
const MinimumTreeWidth = 12;

/** Narrowest content area in rem, the object tree cannot be widened beyond it. */
const MinimumContentWidth = 16;

/** Step of the arrow keys on the splitter in rem. */
const KeyboardStep = 1;

const nodes = ref<DerTreeNode[]>([]);

const loading = ref(true);

const error = ref<string | null>(null);

const selectedId = ref<string | null>(null);

const treeWidth = ref<number | null>(savedTreeWidth);

const abortController = new AbortController();

/** Pointer position and tree width when the splitter drag started, null while not dragging. */
let drag: { startX: number; startWidth: number } | null = null;

/** The selected node, null if no node is selected or the node is no longer in the tree. */
const selectedNode = computed(() => (selectedId.value === null ? null : findDerNode(nodes.value, selectedId.value) ?? null));

const layoutStyle = computed(() => (treeWidth.value === null ? {} : { '--sys-tree-width': `${treeWidth.value}px` }));

async function loadDer(): Promise<void> {
    loading.value = true;
    error.value = null;

    try {
        nodes.value = buildDerTree(await getDer(abortController.signal));
    } catch (exception) {
        if (exception instanceof DOMException && exception.name === 'AbortError') {
            return;
        }

        error.value = exception instanceof Error ? exception.message : String(exception);
    } finally {
        loading.value = false;
    }
}

function remToPixels(rem: number): number {
    return rem * parseFloat(getComputedStyle(document.documentElement).fontSize);
}

/** Current width of the object tree, it sits right before the splitter. */
function currentTreeWidth(splitter: HTMLElement): number {
    return splitter.previousElementSibling?.getBoundingClientRect().width ?? remToPixels(MinimumTreeWidth);
}

/** Sets the tree width, kept between the narrowest tree and the width that leaves the narrowest content area. */
function resizeTree(splitter: HTMLElement, width: number): void {
    const layoutWidth = splitter.parentElement?.clientWidth ?? 0;
    const minimum = remToPixels(MinimumTreeWidth);
    const maximum = Math.max(minimum, layoutWidth - splitter.offsetWidth - remToPixels(MinimumContentWidth));

    treeWidth.value = Math.round(Math.min(Math.max(width, minimum), maximum));
    savedTreeWidth = treeWidth.value;
}

function onSplitterPointerDown(event: PointerEvent): void {
    if (event.button !== 0) {
        return;
    }

    const splitter = event.currentTarget as HTMLElement;

    // No text selection while dragging, the captured pointer keeps the drag alive outside the splitter.
    event.preventDefault();
    splitter.setPointerCapture(event.pointerId);
    splitter.focus();
    drag = { startX: event.clientX, startWidth: currentTreeWidth(splitter) };
}

function onSplitterPointerMove(event: PointerEvent): void {
    if (drag === null) {
        return;
    }

    resizeTree(event.currentTarget as HTMLElement, drag.startWidth + event.clientX - drag.startX);
}

function onSplitterPointerUp(): void {
    drag = null;
}

function onSplitterKeyDown(event: KeyboardEvent): void {
    const splitter = event.currentTarget as HTMLElement;
    const width = currentTreeWidth(splitter);
    const step = remToPixels(KeyboardStep);

    switch (event.key) {
        case 'ArrowLeft':
            resizeTree(splitter, width - step);
            break;
        case 'ArrowRight':
            resizeTree(splitter, width + step);
            break;
        case 'Home':
            resizeTree(splitter, 0);
            break;
        case 'End':
            resizeTree(splitter, Number.POSITIVE_INFINITY);
            break;
        default:
            return;
    }

    event.preventDefault();
}

function resetTreeWidth(): void {
    treeWidth.value = null;
    savedTreeWidth = null;
}

onMounted(() => {
    void loadDer();
});

onBeforeUnmount(() => abortController.abort());
</script>

<template>
    <div class="sys-layout" :style="layoutStyle">
        <DerTree v-model="selectedId" :nodes="nodes" :loading="loading" :error="error" @retry="loadDer" />

        <div class="sys-splitter"
             role="separator"
             tabindex="0"
             aria-orientation="vertical"
             aria-label="Resize the object tree"
             title="Drag to resize, double-click to reset"
             @pointerdown="onSplitterPointerDown"
             @pointermove="onSplitterPointerMove"
             @pointerup="onSplitterPointerUp"
             @pointercancel="onSplitterPointerUp"
             @keydown="onSplitterKeyDown"
             @dblclick="resetTreeWidth"></div>

        <section class="sys-content" aria-live="polite">
            <PmSystemContent v-if="selectedNode?.type === 'PmSystemDto' && selectedNode.deviceId !== undefined" :device-id="selectedNode.deviceId" />
            <template v-else-if="selectedNode">
                <h2 class="om-heading">{{ selectedNode.name }}</h2>
                <p class="mb-0">{{ selectedNode.type }}</p>
            </template>
            <p v-else class="om-hint">Select an object in the tree.</p>
        </section>
    </div>
</template>
