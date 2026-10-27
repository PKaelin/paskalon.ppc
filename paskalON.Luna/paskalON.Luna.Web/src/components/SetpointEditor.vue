<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref, useTemplateRef } from 'vue';
import { getSetpoint, setSetpoint } from '../api/operatingModeApi';
import { useNotifications } from '../composables/useNotifications';
import { formatLocaleNumber, isLocaleNumberKey, parseLocaleNumber } from '../utils/localeNumber';

/** The service works in watts and vars, the operator in megawatts and megavars. */
const megaFactor = 1_000_000;

const { notify } = useNotifications();

/** Entered texts in MW / Mvar, with the digit group and decimal separators of the system locale. */
const activePowerText = ref('');
const reactivePowerText = ref('');

/** Parsed values in MW / Mvar, NaN while the text is not a valid number. */
const activePowerMegawatts = computed(() => parseLocaleNumber(activePowerText.value));
const reactivePowerMegavars = computed(() => parseLocaleNumber(reactivePowerText.value));

const loading = ref(true);
const loadError = ref<string | null>(null);
const applying = ref(false);

/** The active power input gets the focus once the setpoints are loaded. */
const activePowerInput = useTemplateRef<HTMLInputElement>('activePowerInput');

let abortController: AbortController | undefined;

const isValid = computed(() => Number.isFinite(activePowerMegawatts.value) && Number.isFinite(reactivePowerMegavars.value));

/** Watts to megawatts without floating point noise (e.g. 1100000 W -> 1.1 MW). */
function toMega(value: number): number {
    return Number((value / megaFactor).toFixed(6));
}

/** Megawatts to whole watts (e.g. 1.1 MW -> 1100000 W). */
function fromMega(value: number): number {
    return Math.round(value * megaFactor);
}

/** Only digits, the locale's decimal and digit group separators, a minus and the editing keys may be typed. */
function onKeydown(event: KeyboardEvent): void {
    if (event.key.length === 1 && isLocaleNumberKey(event.key) === false && event.ctrlKey === false && event.metaKey === false) {
        event.preventDefault();
    }
}

/** Re-formats a valid entry with the digit group separators (e.g. 1234.5 -> 1’234.5), an invalid entry is kept as typed. */
function reformat(text: string): string {
    const value = parseLocaleNumber(text);

    return Number.isFinite(value) ? formatLocaleNumber(value) : text;
}

async function load(): Promise<void> {
    abortController?.abort();
    abortController = new AbortController();
    loading.value = true;
    loadError.value = null;

    try {
        const setpoint = await getSetpoint(abortController.signal);
        activePowerText.value = formatLocaleNumber(toMega(setpoint.activePowerWatts));
        reactivePowerText.value = formatLocaleNumber(toMega(setpoint.reactivePowerVars));
    } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
            return;
        }

        loadError.value = error instanceof Error ? error.message : String(error);
    } finally {
        loading.value = false;
    }

    // The input is disabled while loading - wait until it is enabled before focusing it.
    await nextTick();
    focusActivePower();
}

/** Focuses the active power input with the cursor at the end of the value. */
function focusActivePower(): void {
    const input = activePowerInput.value;

    if (input === null) {
        return;
    }

    input.focus();
    input.setSelectionRange(input.value.length, input.value.length);
}

/** Applies the entered setpoints. Returns true if the service accepted them. */
async function apply(): Promise<boolean> {
    if (isValid.value === false) {
        return false;
    }

    const activePower = activePowerMegawatts.value;
    const reactivePower = reactivePowerMegavars.value;
    applying.value = true;

    // Applied with Enter the inputs keep the focus, so they are not re-formatted by the blur.
    activePowerText.value = formatLocaleNumber(activePower);
    reactivePowerText.value = formatLocaleNumber(reactivePower);

    try {
        await setSetpoint({
            activePowerWatts: fromMega(activePower),
            reactivePowerVars: fromMega(reactivePower)
        });
        notify(`Setpoints applied: ${formatLocaleNumber(activePower)} MW, ${formatLocaleNumber(reactivePower)} Mvar.`, 'success', 4000);

        return true;
    } catch (error) {
        const message = error instanceof Error ? error.message : String(error);
        notify(`The setpoints could not be applied. ${message} Check that they are within the nameplate limits.`, 'danger', 0);

        return false;
    } finally {
        applying.value = false;
    }
}

/** Sets both setpoints to 0 and applies them, e.g. after all operating modes were removed from the stack. */
async function applyZero(): Promise<boolean> {
    activePowerText.value = formatLocaleNumber(0);
    reactivePowerText.value = formatLocaleNumber(0);

    return apply();
}

// The operating mode stack applies the setpoints as its last step.
defineExpose({ apply, applyZero, isValid });

onMounted(load);

onBeforeUnmount(() => abortController?.abort());
</script>

<template>
    <form class="om-panel" novalidate @submit.prevent="apply">
        <h2 class="om-heading">Active and Reactive Setpoints</h2>

        <div v-if="loadError" class="alert alert-danger py-2 small d-flex align-items-center justify-content-between gap-2" role="alert">
            <span>The current setpoints could not be loaded. {{ loadError }}</span>
            <button class="btn btn-sm btn-outline-light" type="button" @click="load">Retry</button>
        </div>

        <div class="row g-2 align-items-end">
            <div class="col-12 col-sm-5">
                <label class="form-label om-label" for="setpoint-active">Active power</label>
                <div class="input-group input-group-sm">
                    <input id="setpoint-active"
                           ref="activePowerInput"
                           v-model.trim="activePowerText"
                           class="form-control text-end"
                           type="text"
                           inputmode="decimal"
                           autocomplete="off"
                           :disabled="loading"
                           @keydown="onKeydown"
                           @blur="activePowerText = reformat(activePowerText)" />
                    <span class="input-group-text">MW</span>
                </div>
            </div>
            <div class="col-12 col-sm-5">
                <label class="form-label om-label" for="setpoint-reactive">Reactive power</label>
                <div class="input-group input-group-sm">
                    <input id="setpoint-reactive"
                           v-model.trim="reactivePowerText"
                           class="form-control text-end"
                           type="text"
                           inputmode="decimal"
                           autocomplete="off"
                           :disabled="loading"
                           @keydown="onKeydown"
                           @blur="reactivePowerText = reformat(reactivePowerText)" />
                    <span class="input-group-text">Mvar</span>
                </div>
            </div>
            <div class="col-12 col-sm-2 d-grid">
                <button class="btn btn-sm btn-secondary" type="submit" :disabled="loading || applying || isValid === false">
                    <span v-if="applying" class="spinner-border spinner-inline me-1" role="status" aria-hidden="true"></span>
                    Apply
                </button>
            </div>
        </div>
    </form>
</template>
