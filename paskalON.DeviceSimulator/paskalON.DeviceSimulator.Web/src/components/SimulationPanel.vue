<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import BitmaskEditor from './BitmaskEditor.vue';
import CollapseTransition from './CollapseTransition.vue';
import { SimulationDeviceType, type SimulationDeviceTypeValue, type SimulationDto } from '@/types/der';
import { setCommunication, setEvents, slowDownPower } from '@/api/derApi';
import { useNotifications } from '@/composables/useNotifications';

/**
 * Simulation controls of a simulated PCS or battery bank:
 *   - warning / fault / vendor event bitmask injection
 *   - communication loss (the Modbus server stops answering)
 *   - PCS only: slow down the power response
 *
 * Successful changes are written into the simulation DTO right away, the next refresh /
 * live update confirms them.
 */
const props = defineProps<{
    simulation: SimulationDto;
    deviceId: number;
    deviceName: string;
    isPowerConversionSystem: boolean;
}>();

type EventKind = 'warning' | 'fault' | 'vendorEvent';

const { notify } = useNotifications();
const busy = ref(false);

const deviceType = computed<SimulationDeviceTypeValue>(() =>
    props.isPowerConversionSystem ? SimulationDeviceType.PowerConversionSystem : SimulationDeviceType.BatteryBank
);

const stateBadge = computed(() => {
    switch (props.simulation.state) {
        case 'Fault':
            return 'bg-danger';
        case 'On':
        case 'Connected':
            return 'bg-success';
        case 'Standby':
        case 'PreCharging':
            return 'bg-warning text-dark';
        default:
            return 'bg-secondary';
    }
});

/** The panel starts collapsed, the header always shows state, active events and communication loss. */
const isOpen = ref(false);
const panelId = computed(() => `simulation-${deviceType.value}-${props.deviceId}`);

const hasStateOfCharge = computed(
    () => props.simulation.usableStateOfCharge !== null && props.simulation.usableStateOfCharge !== undefined
);

function toHex(value: number): string {
    return value.toString(16).toUpperCase().padStart(4, '0');
}

/** Events that are currently set (value other than 0), fault first. */
const activeEvents = computed(() => {
    const events: { label: string; hex: string; css: string }[] = [];

    if (props.simulation.fault) {
        events.push({ label: 'Fault', hex: toHex(props.simulation.fault), css: 'bg-danger' });
    }

    if (props.simulation.warning) {
        events.push({ label: 'Warning', hex: toHex(props.simulation.warning), css: 'bg-warning text-dark' });
    }

    if (props.simulation.vendorEvent) {
        events.push({ label: 'Vendor', hex: toHex(props.simulation.vendorEvent), css: 'bg-info text-dark' });
    }

    return events;
});

const slowActive = ref<number>(props.simulation.slowActivePowerByPercent ?? 0);
const slowReactive = ref<number>(props.simulation.slowReactivePowerByPercent ?? 0);

watch(
    () => [props.simulation.slowActivePowerByPercent, props.simulation.slowReactivePowerByPercent],
    ([active, reactive]) => {
        if (!busy.value) {
            slowActive.value = active ?? 0;
            slowReactive.value = reactive ?? 0;
        }
    }
);

async function run(action: () => Promise<void>, success: string): Promise<boolean> {
    busy.value = true;

    try {
        await action();
        notify(success, 'success', 2500);
        return true;
    } catch (error) {
        notify(error instanceof Error ? error.message : String(error), 'danger', 8000);
        return false;
    } finally {
        busy.value = false;
    }
}

const eventLabels: Record<EventKind, string> = {
    warning: 'Warning',
    fault: 'Fault',
    vendorEvent: 'Vendor event'
};

async function applyEvent(kind: EventKind, value: number): Promise<void> {
    const applied = await run(
        () => setEvents({ deviceType: deviceType.value, deviceId: props.deviceId, [kind]: value }),
        `${props.deviceName} / ${eventLabels[kind]} = ${value} (0x${value.toString(16).toUpperCase()}) written.`
    );

    if (!applied) {
        return;
    }

    props.simulation[kind] = value;

    // An injected PCS fault stops the PCS until the controller sends the next command.
    if (kind === 'fault' && value !== 0 && props.isPowerConversionSystem) {
        props.simulation.state = 'Fault';
    }
}

async function onCommunicationChange(event: Event): Promise<void> {
    const checkbox = event.target as HTMLInputElement;
    const isResponding = checkbox.checked;

    const applied = await run(
        () => setCommunication({ deviceType: deviceType.value, deviceId: props.deviceId, isResponding }),
        isResponding
            ? `${props.deviceName} answers Modbus requests again.`
            : `${props.deviceName} stopped answering Modbus requests (communication loss).`
    );

    if (applied) {
        props.simulation.isResponding = isResponding;
    } else {
        checkbox.checked = props.simulation.isResponding;
    }
}

async function applySlowDown(): Promise<void> {
    const active = Math.max(0, Number(slowActive.value) || 0);
    const reactive = Math.max(0, Number(slowReactive.value) || 0);

    const applied = await run(
        () => slowDownPower({ deviceId: props.deviceId, slowActivePowerByPercent: active, slowReactivePowerByPercent: reactive }),
        active === 0 && reactive === 0
            ? `${props.deviceName} responds instantly again.`
            : `${props.deviceName} ramps with ${active} %/s active and ${reactive} %/s reactive power.`
    );

    if (applied) {
        props.simulation.slowActivePowerByPercent = active;
        props.simulation.slowReactivePowerByPercent = reactive;
    }
}
</script>

<template>
    <div class="mb-3 simulation-panel">
        <button class="simulation-header"
                type="button"
                :aria-expanded="isOpen"
                :aria-controls="panelId"
                @click="isOpen = !isOpen">
            <i class="bi" :class="isOpen ? 'bi-chevron-down' : 'bi-chevron-right'" aria-hidden="true"></i>
            <span class="simulation-title">Simulation</span>
            <span class="badge" :class="stateBadge">{{ simulation.state }}</span>
            <span v-if="!simulation.isResponding" class="badge bg-danger">Communication loss</span>
            <span v-for="active in activeEvents" :key="active.label" class="badge" :class="active.css">
                {{ active.label }} 0x{{ active.hex }}
            </span>
            <span v-if="hasStateOfCharge" class="simulation-soc font-monospace">
                SOC {{ simulation.usableStateOfCharge!.toFixed(2) }} %
            </span>
        </button>

        <CollapseTransition>
            <div v-show="isOpen" :id="panelId" class="simulation-body">
                <div class="d-flex flex-wrap align-items-center gap-3 mb-2">
                    <span v-if="hasStateOfCharge">
                        Usable SOC <span class="font-monospace">{{ simulation.usableStateOfCharge!.toFixed(2) }} %</span>
                    </span>

                    <div class="form-check form-switch mb-0">
                        <input :id="`responding-${deviceType}-${deviceId}`"
                               class="form-check-input"
                               type="checkbox"
                               role="switch"
                               :checked="simulation.isResponding"
                               :disabled="busy"
                               @change="onCommunicationChange" />
                        <label class="form-check-label" :for="`responding-${deviceType}-${deviceId}`">
                            Modbus responding
                        </label>
                    </div>
                    <span v-if="!simulation.isResponding" class="badge bg-danger">Communication loss</span>
                </div>

                <div class="d-flex flex-column gap-1">
                    <BitmaskEditor v-if="simulation.hasFaultRegister"
                                   label="Fault"
                                   :model-value="simulation.fault ?? 0"
                                   :busy="busy"
                                   @apply="(value) => applyEvent('fault', value)" />
                    <BitmaskEditor v-if="simulation.hasWarningRegister"
                                   label="Warning"
                                   :model-value="simulation.warning ?? 0"
                                   :busy="busy"
                                   @apply="(value) => applyEvent('warning', value)" />
                    <BitmaskEditor v-if="simulation.hasVendorEventRegister"
                                   label="Vendor event"
                                   :model-value="simulation.vendorEvent ?? 0"
                                   :busy="busy"
                                   @apply="(value) => applyEvent('vendorEvent', value)" />
                </div>

                <p v-if="isPowerConversionSystem && simulation.hasFaultRegister" class="simulation-hint mt-1 mb-2">
                    A fault other than 0 puts the PCS into the fault state (no power, AC breaker open). While the fault is set,
                    start, stop and standby commands are rejected. After clearing the fault the PCS stays faulted until the
                    controller sends the next start, stop or standby command.
                </p>

                <div v-if="isPowerConversionSystem" class="ramp-settings">
                    <div class="ramp-caption">Power ramp limit</div>
                    <div class="d-flex flex-wrap align-items-center gap-2">
                        <div class="input-group input-group-sm ramp-input">
                            <span class="input-group-text">Active power (P)</span>
                            <input v-model.number="slowActive"
                                   class="form-control font-monospace"
                                   type="number"
                                   min="0"
                                   step="any"
                                   :disabled="busy"
                                   aria-label="Active power ramp limit in percent of the nameplate active power per second" />
                            <span class="input-group-text">% / s</span>
                        </div>
                        <div class="input-group input-group-sm ramp-input">
                            <span class="input-group-text">Reactive power (Q)</span>
                            <input v-model.number="slowReactive"
                                   class="form-control font-monospace"
                                   type="number"
                                   min="0"
                                   step="any"
                                   :disabled="busy"
                                   aria-label="Reactive power ramp limit in percent of the nameplate reactive power per second" />
                            <span class="input-group-text">% / s</span>
                        </div>
                        <button class="btn btn-sm btn-primary" type="button" :disabled="busy" @click="applySlowDown">Apply</button>
                    </div>
                    <div class="simulation-hint mt-1">
                        Two separate limits: P limits how fast the active power changes, Q how fast the reactive power changes,
                        each in percent of its nameplate per second (e.g. 10 = 10% of the nameplate per second). 0 = instant (ideal PCS).
                    </div>
                </div>
            </div>
        </CollapseTransition>
    </div>
</template>

<style scoped>
.simulation-panel {
    font-size: .8125rem;
}

.simulation-header {
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    gap: .375rem;
    width: 100%;
    max-width: 1000px;
    padding: .25rem .5rem;
    border: 1px solid rgba(255, 255, 255, .15);
    background: rgba(0, 0, 0, .15);
    color: inherit;
    text-align: left;
}

.simulation-header:hover {
    background: rgba(255, 255, 255, .06);
}

.simulation-title {
    font-weight: 600;
    margin-right: .25rem;
}

.simulation-soc {
    margin-left: auto;
    opacity: .85;
}

.simulation-body {
    max-width: 1000px;
    padding: .5rem;
    border: 1px solid rgba(255, 255, 255, .15);
    border-top: 0;
    overflow: hidden;
}

.simulation-hint {
    font-size: .75rem;
    opacity: .75;
}

.ramp-settings {
    margin-top: .5rem;
}

.ramp-caption {
    font-weight: 600;
    margin-bottom: .25rem;
}

.ramp-input {
    width: auto;
    max-width: 16.5rem;
}

.ramp-input input {
    max-width: 5rem;
}

.simulation-panel :deep(.input-group-sm > .form-control),
.simulation-panel :deep(.input-group-sm > .input-group-text),
.simulation-panel :deep(.input-group-sm > .btn),
.simulation-panel .btn-sm {
    font-size: .75rem;
    padding: .125rem .375rem;
}
</style>
