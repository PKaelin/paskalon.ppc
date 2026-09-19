<script setup lang="ts">
import { computed, ref, watch } from 'vue';

/**
 * Renders the value cell of an endpoint grid.
 *
 * IsReadOnly === true  -> read only input (the value is still shown)
 * IsReadOnly === false -> editable input, typed according to the current value:
 *                         checkbox for booleans, number input for numbers, text otherwise.
 */
const props = defineProps<{
    modelValue: unknown;
    readOnly: boolean;
    label: string;
}>();

const emit = defineEmits<{
    (event: 'update:modelValue', value: unknown): void;
    (event: 'committed', value: unknown): void;
}>();

type ValueKind = 'boolean' | 'number' | 'text';

const kind = computed<ValueKind>(() => {
    if (typeof props.modelValue === 'boolean') {
        return 'boolean';
    }

    if (typeof props.modelValue === 'number') {
        return 'number';
    }

    return 'text';
});

/**
 * Local draft so the model is only updated on change/blur, not on every keystroke.
 * Note: on <input type="number"> Vue casts the bound value to a number automatically,
 * so the draft can hold a string OR a number - it is normalized in commitText().
 */
const draft = ref<string | number>(formatValue(props.modelValue));
const dirty = ref(false);

watch(
    () => props.modelValue,
    (value) => {
        if (!dirty.value) {
            draft.value = formatValue(value);
        }
    }
);

function formatValue(value: unknown): string {
    if (value === null || value === undefined) {
        return '';
    }

    if (typeof value === 'object') {
        return JSON.stringify(value);
    }

    return String(value);
}

function commitText(): void {
    const raw = draft.value === null || draft.value === undefined ? '' : String(draft.value);
    let parsed: unknown = raw;

    if (kind.value === 'number') {
        const asNumber = Number(raw);
        parsed = raw.trim() === '' || Number.isNaN(asNumber) ? props.modelValue : asNumber;
    }

    dirty.value = false;
    draft.value = formatValue(parsed);

    if (parsed !== props.modelValue) {
        emit('update:modelValue', parsed);
        emit('committed', parsed);
    }
}

function onBooleanChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    emit('update:modelValue', checked);
    emit('committed', checked);
}
</script>

<template>
    <div v-if="kind === 'boolean'" class="form-check mb-0">
        <input class="form-check-input"
               type="checkbox"
               :checked="modelValue === true"
               :disabled="readOnly"
               :aria-label="label"
               @change="onBooleanChange" />
        <span class="ms-1 small">{{ modelValue === true ? 'true' : 'false' }}</span>
    </div>

    <input v-else
           :class="['form-control', 'form-control-sm', 'endpoint-value-input', { 'endpoint-dirty': dirty }]"
           :type="kind === 'number' ? 'number' : 'text'"
           step="any"
           :readonly="readOnly"
           :tabIndex="readOnly ? -1 : undefined"
           :aria-label="label"
           v-model="draft"
           @input="dirty = true"
           @change="commitText"
           @blur="commitText"
           @keyup.enter="commitText" />
</template>
