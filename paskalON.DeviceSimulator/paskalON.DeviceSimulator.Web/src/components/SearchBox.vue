<script setup lang="ts">
import { ref, watch } from 'vue';

const props = defineProps<{ modelValue: string; resultText?: string }>();
const emit = defineEmits<{ (event: 'update:modelValue', value: string): void }>();

const inputRef = ref<HTMLInputElement | null>(null);
const local = ref(props.modelValue);

watch(
    () => props.modelValue,
    (value) => {
        local.value = value;
    }
);

function onInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    local.value = value;
    emit('update:modelValue', value);
}

function clear(): void {
    local.value = '';
    emit('update:modelValue', '');
    inputRef.value?.focus();
}
</script>

<template>
    <div class="input-group">
        <span class="input-group-text" id="search-addon">
            <i class="bi bi-search" aria-hidden="true"></i>
        </span>
        <input ref="inputRef"
               type="search"
               class="form-control"
               placeholder="Search groups, circuits, units, devices and meters..."
               aria-label="Search the device tree"
               aria-describedby="search-addon"
               autocomplete="off"
               :value="local"
               @input="onInput"
               @keyup.escape="clear" />
        <button v-if="local.length > 0"
                class="btn btn-outline-secondary"
                type="button"
                title="Clear search (Esc)"
                @click="clear">
            <i class="bi bi-x-lg" aria-hidden="true"></i>
        </button>
    </div>
    <div v-if="resultText" class="search-summary mt-1">{{ resultText }}</div>
</template>
