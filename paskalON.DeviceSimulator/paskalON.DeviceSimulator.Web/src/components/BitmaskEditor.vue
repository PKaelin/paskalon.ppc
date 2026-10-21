<script setup lang="ts">
import { computed, ref, watch } from 'vue';

/**
 * Edits a 16 bit register value as bitmask: one checkbox per bit (bit 15 left, bit 0 right)
 * plus a decimal input. Nothing is written until "Apply" (or "Clear") is pressed.
 */
const props = defineProps<{
    label: string;
    /** Current register value reported by the service. */
    modelValue: number;
    busy?: boolean;
}>();

const emit = defineEmits<{
    (event: 'apply', value: number): void;
}>();

const maxValue = 0xffff;
const bits = Array.from({ length: 16 }, (_, index) => 15 - index);

const draft = ref<number>(props.modelValue);
const dirty = ref(false);

// Follow the service value as long as the user has not started editing.
watch(
    () => props.modelValue,
    (value) => {
        if (!dirty.value) {
            draft.value = value;
        }
    }
);

const hex = computed(() => draft.value.toString(16).toUpperCase().padStart(4, '0'));
const currentHex = computed(() => props.modelValue.toString(16).toUpperCase().padStart(4, '0'));

function normalize(value: unknown): number {
    const asNumber = Math.trunc(Number(value));

    if (Number.isNaN(asNumber)) {
        return 0;
    }

    return Math.min(Math.max(asNumber, 0), maxValue);
}

function isSet(bit: number): boolean {
    return (draft.value & (1 << bit)) !== 0;
}

function toggle(bit: number): void {
    draft.value = draft.value ^ (1 << bit);
    dirty.value = true;
}

function onInput(event: Event): void {
    draft.value = normalize((event.target as HTMLInputElement).value);
    dirty.value = true;
}

function apply(): void {
    dirty.value = false;
    emit('apply', draft.value);
}

function clear(): void {
    draft.value = 0;
    dirty.value = false;
    emit('apply', 0);
}
</script>

<template>
    <div class="bitmask-editor">
        <div class="bitmask-heading">
            <span class="bitmask-label">{{ label }}</span>
            <span class="bitmask-current font-monospace" :title="`Current value ${modelValue}`">0x{{ currentHex }}</span>
            <span v-if="dirty" class="badge bg-warning text-dark">not applied</span>
        </div>

        <div class="bitmask-bits" role="group" :aria-label="`${label} bits`">
            <label v-for="bit in bits"
                   :key="bit"
                   class="bitmask-bit"
                   :class="{ 'bitmask-bit-set': isSet(bit) }"
                   :title="`Bit ${bit} = ${1 << bit}`">
                <input class="form-check-input m-0"
                       type="checkbox"
                       :checked="isSet(bit)"
                       :disabled="busy"
                       :aria-label="`${label} bit ${bit}`"
                       @change="toggle(bit)" />
                <span class="bitmask-bit-number">{{ bit }}</span>
            </label>
        </div>

        <div class="d-flex flex-wrap align-items-center gap-2">
            <div class="input-group input-group-sm bitmask-input">
                <span class="input-group-text">Value</span>
                <input class="form-control font-monospace"
                       type="number"
                       min="0"
                       :max="maxValue"
                       step="1"
                       :value="draft"
                       :disabled="busy"
                       :aria-label="`${label} value`"
                       @change="onInput"
                       @keyup.enter="apply" />
                <span class="input-group-text font-monospace">0x{{ hex }}</span>
            </div>
            <button class="btn btn-sm btn-primary bitmask-button" type="button" :disabled="busy" @click="apply">Apply</button>
            <button class="btn btn-sm btn-outline-light bitmask-button" type="button" :disabled="busy" @click="clear">Clear</button>
        </div>
    </div>
</template>

<style scoped>
/* One compact row: heading | 16 bits | value input. Wraps on narrow screens. */
.bitmask-editor {
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    gap: .25rem .75rem;
    padding: .25rem .5rem;
    border: 1px solid rgba(255, 255, 255, .15);
    background: rgba(0, 0, 0, .12);
    font-size: .75rem;
}

.bitmask-heading {
    display: flex;
    align-items: baseline;
    gap: .375rem;
    min-width: 8.5rem;
}

.bitmask-label {
    font-weight: 600;
}

.bitmask-current {
    opacity: .75;
}

.bitmask-bits {
    display: grid;
    grid-template-columns: repeat(16, 1.25rem);
    gap: 1px;
}

.bitmask-bit {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0;
    padding: 1px 0;
    cursor: pointer;
    border: 1px solid transparent;
}

.bitmask-bit .form-check-input {
    width: .8rem;
    height: .8rem;
}

.bitmask-bit-set {
    border-color: rgba(240, 173, 78, .6);
    background: rgba(240, 173, 78, .15);
}

/* Visual grouping per nibble (bits 15-12, 11-8, 7-4, 3-0). */
.bitmask-bit:nth-child(4n) {
    margin-right: 4px;
}

.bitmask-bit-number {
    font-family: var(--bs-font-monospace);
    font-size: .6rem;
    line-height: 1;
    opacity: .8;
}

.bitmask-input {
    width: auto;
}

.bitmask-input > .form-control {
    max-width: 5rem;
}

.bitmask-input > .form-control,
.bitmask-input > .input-group-text,
.bitmask-button {
    font-size: .75rem;
    padding: .125rem .375rem;
}

@media (max-width: 575.98px) {
    .bitmask-bits {
        grid-template-columns: repeat(8, 1.25rem);
    }
}
</style>
