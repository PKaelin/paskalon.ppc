<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from 'vue';
import DeviationBar from './DeviationBar.vue';
import MetricCard from './MetricCard.vue';
import { getNameplate } from '../api/systemApi';
import { useSystemMeterSummary, type HubStatus } from '../composables/useSystemMeterSummary';
import type { SystemNameplateDto } from '../types/system';

const WattsPerMegawatt = 1_000_000;

/** Bar colours of the state of charge, the first range whose lower limit the value reaches applies. */
const stateOfChargeColors: { from: number; color: string }[] = [
    { from: 81, color: '#3B82F6' }, // High
    { from: 20, color: '#22C55E' }, // Normal
    { from: 10, color: '#EAB308' }, // Moderate
    { from: 5, color: '#F97316' }, // Low
    { from: Number.NEGATIVE_INFINITY, color: '#EF4444' } // Critical
];

const { status, summary, received } = useSystemMeterSummary();

const nameplate = ref<SystemNameplateDto | null>(null);

const nameplateError = ref<string | null>(null);

const abortController = new AbortController();

const formatters = new Map<number, Intl.NumberFormat>();

/** Formats with the separators of the system locale and a fixed number of decimals, -0 is shown as 0. */
function format(value: number, decimals: number): string {
    let formatter = formatters.get(decimals);

    if (formatter === undefined) {
        formatter = new Intl.NumberFormat(undefined, { minimumFractionDigits: decimals, maximumFractionDigits: decimals });
        formatters.set(decimals, formatter);
    }

    const rounded = Number(value.toFixed(decimals));

    return formatter.format(rounded === 0 ? 0 : rounded);
}

/** Text below a power value: the sign is taken from the shown (rounded) value, so 0,00 never says producing. */
function caption(value: number, decimals: number, positive: string, negative: string): string {
    const rounded = Number(value.toFixed(decimals));

    if (rounded > 0) {
        return positive;
    }

    if (rounded < 0) {
        return negative;
    }

    return '';
}

const activePowerMegawatts = computed(() => (summary.value === null ? null : summary.value.activePower.watts / WattsPerMegawatt));

const reactivePowerMegavars = computed(() => (summary.value === null ? null : summary.value.reactivePower.voltAmperesReactive / WattsPerMegawatt));

const stateOfCharge = computed(() => summary.value?.stateOfCharge ?? null);

const stateOfChargeColor = computed(() => {
    const value = stateOfCharge.value ?? 0;

    return stateOfChargeColors.find((range) => value >= range.from)?.color ?? '#EF4444';
});

const statusText: Record<HubStatus, string> = {
    connecting: 'Connecting…',
    connected: 'Live',
    reconnecting: 'Reconnecting…',
    disconnected: 'Disconnected'
};

const hint = computed(() => {
    if (status.value === 'connected' && received.value && summary.value === null) {
        return 'No system power meter data available.';
    }

    if (status.value === 'disconnected' || status.value === 'reconnecting') {
        return 'The Luna service cannot be reached, the values are not updated.';
    }

    return null;
});

async function loadNameplate(): Promise<void> {
    nameplateError.value = null;

    try {
        nameplate.value = await getNameplate(abortController.signal);
    } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
            return;
        }

        nameplateError.value = error instanceof Error ? error.message : String(error);
    }
}

onMounted(() => {
    void loadNameplate();
});

onBeforeUnmount(() => abortController.abort());
</script>

<template>
    <section class="dash-section" aria-labelledby="dash-system-title">
        <div class="dash-section-header">
            <h2 id="dash-system-title" class="dash-section-title">System</h2>
            <span class="dash-status" :class="`dash-status-${status}`">
                <span class="dash-status-dot" aria-hidden="true"></span>{{ statusText[status] }}
            </span>
        </div>

        <p v-if="hint" class="dash-hint">
            <i class="bi bi-exclamation-triangle me-1" aria-hidden="true"></i>{{ hint }}
        </p>
        <p v-if="nameplateError" class="dash-hint">
            <i class="bi bi-exclamation-triangle me-1" aria-hidden="true"></i>{{ nameplateError }}
            <button class="btn btn-link btn-sm p-0 align-baseline" type="button" @click="loadNameplate">Retry</button>
        </p>

        <div class="dash-cards">
            <MetricCard label="Active Power"
                        :value="activePowerMegawatts === null ? '—' : format(activePowerMegawatts, 2)"
                        unit="MW"
                        :caption="activePowerMegawatts === null ? '' : caption(activePowerMegawatts, 2, 'Producing', 'Consuming')">
                <DeviationBar label="Active power"
                              :value="nameplate === null ? null : activePowerMegawatts"
                              :min="(nameplate?.minimumActivePowerWatts ?? -1) / WattsPerMegawatt"
                              :max="(nameplate?.maximumActivePowerWatts ?? 1) / WattsPerMegawatt"
                              color="#FFC857" />
            </MetricCard>

            <MetricCard label="Reactive Power"
                        :value="reactivePowerMegavars === null ? '—' : format(reactivePowerMegavars, 2)"
                        unit="MVAr"
                        :caption="reactivePowerMegavars === null ? '' : caption(reactivePowerMegavars, 2, 'Injecting', 'Absorbing')">
                <DeviationBar label="Reactive power"
                              :value="nameplate === null ? null : reactivePowerMegavars"
                              :min="(nameplate?.minimumReactivePowerVars ?? -1) / WattsPerMegawatt"
                              :max="(nameplate?.maximumReactivePowerVars ?? 1) / WattsPerMegawatt"
                              color="#B79CED" />
            </MetricCard>

            <MetricCard v-if="stateOfCharge !== null"
                        label="State Of Charge"
                        :value="format(stateOfCharge, 1)"
                        unit="%"
                        caption="">
                <DeviationBar label="State of charge" :value="stateOfCharge" :min="0" :max="100" :color="stateOfChargeColor" />
            </MetricCard>

            <MetricCard label="Power Factor" :value="summary === null ? '—' : format(summary.powerFactor, 3)" />

            <MetricCard label="Frequency" :value="summary === null ? '—' : format(summary.frequency, 2)" unit="Hz" />
        </div>
    </section>
</template>
