<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import BitmaskEditor from './BitmaskEditor.vue';
import CollapseTransition from './CollapseTransition.vue';
import { SimulationDeviceType, type SimulationDeviceTypeValue, type SimulationDto } from '@/types/der';
import { setChargeSpeed, setCommunication, setEvents, setPowerTaper, setStateOfCharge, slowDownPower } from '@/api/derApi';
import { useNotifications } from '@/composables/useNotifications';

/**
 * Simulation controls of a simulated PCS, battery bank or power meter:
 *   - warning / fault / vendor event bitmask injection (PCS, battery bank)
 *   - communication loss (the Modbus server stops answering / the C37 server stops streaming)
 *   - PCS only: slow down the power response
 *   - battery bank only: set the preferred state of charge, the charge speed (1x real time .. 100x) and the power taper
 *
 * Successful changes are written into the simulation DTO right away, the next refresh /
 * live update confirms them.
 */
const props = defineProps<{
    simulation: SimulationDto;
    deviceId: number;
    deviceName: string;
    deviceType: SimulationDeviceTypeValue;
}>();

type EventKind = 'warning' | 'fault' | 'vendorEvent';

const { notify } = useNotifications();
const busy = ref(false);

const isPowerConversionSystem = computed(() => props.deviceType === SimulationDeviceType.PowerConversionSystem);
const isPowerMeter = computed(() => props.deviceType === SimulationDeviceType.PowerMeter);

/** Protocol whose server is stopped to simulate a communication loss. */
const protocol = computed(() => (isPowerMeter.value ? 'C37' : 'Modbus'));

const stateBadge = computed(() => {
    switch (props.simulation.state) {
        case 'Fault':
            return 'bg-danger';
        case 'On':
        case 'Connected':
        case 'Streaming':
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
const panelId = computed(() => `simulation-${props.deviceType}-${props.deviceId}`);

const isBatteryBank = computed(() => props.deviceType === SimulationDeviceType.BatteryBank);

const hasStateOfCharge = computed(
    () => props.simulation.usableStateOfCharge !== null && props.simulation.usableStateOfCharge !== undefined
);

/** State of charge with 2 decimals, missing values as '–'. */
function formatStateOfCharge(value: number | null | undefined): string {
    return value === null || value === undefined ? '–' : value.toFixed(2);
}

const MIN_CHARGE_SPEED = 1;
const MAX_CHARGE_SPEED = 100;

/** Preferred SOC entered by the user, prefilled with the current value (not overwritten by live updates while typing). */
const preferredSoc = ref<number>(Math.round(props.simulation.preferredStateOfCharge ?? 100));
const chargeSpeed = ref<number>(props.simulation.chargeSpeedFactor ?? MIN_CHARGE_SPEED);

const isAccelerated = computed(() => (props.simulation.chargeSpeedFactor ?? MIN_CHARGE_SPEED) > MIN_CHARGE_SPEED);

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

watch(
    () => props.simulation.chargeSpeedFactor,
    (factor) => {
        if (!busy.value) {
            chargeSpeed.value = factor ?? MIN_CHARGE_SPEED;
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
        () => setEvents({ deviceType: props.deviceType, deviceId: props.deviceId, [kind]: value }),
        `${props.deviceName} / ${eventLabels[kind]} = ${value} (0x${value.toString(16).toUpperCase()}) written.`
    );

    if (!applied) {
        return;
    }

    props.simulation[kind] = value;

    // An injected PCS fault stops the PCS until the controller sends the next command.
    if (kind === 'fault' && value !== 0 && isPowerConversionSystem.value) {
        props.simulation.state = 'Fault';
    }
}

function communicationMessage(isResponding: boolean): string {
    if (isPowerMeter.value) {
        return isResponding
            ? `${props.deviceName} streams C37 data again. The device service reconnects within its reconnect interval.`
            : `${props.deviceName} stopped streaming C37 data (communication loss).`;
    }

    return isResponding
        ? `${props.deviceName} answers Modbus requests again. The device service reconnects within its reconnect interval.`
        : `${props.deviceName} stopped answering Modbus requests (communication loss).`;
}

async function onCommunicationChange(event: Event): Promise<void> {
    const checkbox = event.target as HTMLInputElement;
    const isResponding = checkbox.checked;

    const applied = await run(
        () => setCommunication({ deviceType: props.deviceType, deviceId: props.deviceId, isResponding }),
        communicationMessage(isResponding)
    );

    if (applied) {
        props.simulation.isResponding = isResponding;

        if (isPowerMeter.value) {
            props.simulation.state = isResponding ? 'Streaming' : 'Stopped';
        }
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

async function applyStateOfCharge(): Promise<void> {
    const value = Number(preferredSoc.value);

    if (!Number.isFinite(value) || value < 0 || value > 100) {
        notify('Preferred SOC must be between 0 and 100 %.', 'danger', 8000);
        return;
    }

    const applied = await run(
        () => setStateOfCharge({ deviceId: props.deviceId, preferredStateOfCharge: value }),
        `${props.deviceName} / Preferred SOC set to ${value} %.`
    );

    if (applied) {
        props.simulation.preferredStateOfCharge = value;
    }
}

async function applyChargeSpeed(): Promise<void> {
    const factor = Math.min(MAX_CHARGE_SPEED, Math.max(MIN_CHARGE_SPEED, Math.round(Number(chargeSpeed.value) || MIN_CHARGE_SPEED)));

    const applied = await run(
        () => setChargeSpeed({ deviceId: props.deviceId, chargeSpeedFactor: factor }),
        factor === MIN_CHARGE_SPEED
            ? `${props.deviceName} charges and discharges in real time again.`
            : `${props.deviceName} charges and discharges ${factor}x faster (1 s = ${factor} s).`
    );

    if (applied) {
        props.simulation.chargeSpeedFactor = factor;
    } else {
        chargeSpeed.value = props.simulation.chargeSpeedFactor ?? MIN_CHARGE_SPEED;
    }
}

async function onPowerTaperChange(event: Event): Promise<void> {
    const checkbox = event.target as HTMLInputElement;
    const isEnabled = checkbox.checked;

    const applied = await run(
        () => setPowerTaper({ deviceId: props.deviceId, isEnabled }),
        isEnabled
            ? `${props.deviceName} tapers the power below 10 % and above 90 % usable SOC.`
            : `${props.deviceName} uses full power until 0 % and 100 % usable SOC.`
    );

    if (applied) {
        props.simulation.isPowerTaperEnabled = isEnabled;
    } else {
        checkbox.checked = props.simulation.isPowerTaperEnabled ?? false;
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
            <span v-if="isAccelerated" class="badge bg-warning text-dark">Charge speed ×{{ simulation.chargeSpeedFactor }}</span>
            <span v-if="simulation.isPowerTaperEnabled" class="badge bg-info text-dark">Power taper</span>
            <span v-if="hasStateOfCharge" class="simulation-soc font-monospace">
                Preferred SOC {{ formatStateOfCharge(simulation.preferredStateOfCharge) }} %
            </span>
        </button>

        <CollapseTransition>
            <div v-show="isOpen" :id="panelId" class="simulation-body">
                <div class="d-flex flex-wrap align-items-center gap-3 mb-2">
                    <span v-if="hasStateOfCharge">
                        Preferred SOC <span class="font-monospace">{{ formatStateOfCharge(simulation.preferredStateOfCharge) }} %</span>
                    </span>
                    <span v-if="hasStateOfCharge">
                        Usable SOC <span class="font-monospace">{{ formatStateOfCharge(simulation.usableStateOfCharge) }} %</span>
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
                            {{ protocol }} responding
                        </label>
                    </div>
                    <span v-if="!simulation.isResponding" class="badge bg-danger">Communication loss</span>
                </div>

                <p v-if="isPowerMeter" class="simulation-hint mt-1 mb-0">
                    Switching C37 off stops the C37 server of the meter's port: the connection is closed and new connections
                    are refused (C37 communication error). All power meters streaming on the same port lose the communication too.
                </p>

                <p class="simulation-hint mt-1 mb-2">
                    Restoring the communication takes some time: the device service reconnects lost {{ protocol }} connections
                    only every {{ protocol }}ReconnectIntervalMilliseconds (system configuration of the device service, default 30 s).
                    Until then the device keeps its communication error, a lost system power meter keeps the operating modes holding.
                </p>

                <div v-if="!isPowerMeter" class="d-flex flex-column gap-1">
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

                <div v-if="isBatteryBank && hasStateOfCharge" class="battery-settings ramp-settings">
                    <div>
                        <div class="ramp-caption">State of charge</div>
                        <div class="d-flex flex-wrap align-items-center gap-2">
                            <div class="input-group input-group-sm ramp-input">
                                <span class="input-group-text">Preferred SOC</span>
                                <input v-model.number="preferredSoc"
                                       class="form-control font-monospace"
                                       type="number"
                                       min="0"
                                       max="100"
                                       step="any"
                                       :disabled="busy"
                                       aria-label="Preferred state of charge in percent"
                                       @keyup.enter="applyStateOfCharge" />
                                <span class="input-group-text">%</span>
                            </div>
                            <button class="btn btn-sm btn-primary" type="button" :disabled="busy" @click="applyStateOfCharge">Set</button>
                        </div>
                        <div class="simulation-hint mt-1">
                            Sets the state of charge in the preferred (operator) window, 0–100 %. The value is limited to the usable window.
                        </div>
                    </div>

                    <div>
                        <div class="ramp-caption">Charge speed</div>
                        <div class="d-flex flex-wrap align-items-center gap-2">
                            <input v-model.number="chargeSpeed"
                                   class="form-range charge-speed-range"
                                   type="range"
                                   :min="MIN_CHARGE_SPEED"
                                   :max="MAX_CHARGE_SPEED"
                                   step="1"
                                   :disabled="busy"
                                   aria-label="Charge speed factor, 1 is real time, 100 means one second charges or discharges like 100 seconds"
                                   @change="applyChargeSpeed" />
                            <span class="font-monospace">×{{ chargeSpeed }}</span>
                        </div>
                        <div class="simulation-hint mt-1">
                            1 = real time. 100 = one second charges or discharges like 100 seconds. Overrides the physical definition
                            for the state of charge only; the active power stays as allocated by the PCS.
                        </div>
                    </div>

                    <div class="battery-settings-full">
                        <div class="ramp-caption">Power taper</div>
                        <div class="form-check form-switch mb-0">
                            <input :id="`power-taper-${deviceType}-${deviceId}`"
                                   class="form-check-input"
                                   type="checkbox"
                                   role="switch"
                                   :checked="simulation.isPowerTaperEnabled ?? false"
                                   :disabled="busy"
                                   @change="onPowerTaperChange" />
                            <label class="form-check-label" :for="`power-taper-${deviceType}-${deviceId}`">
                                Taper power near empty and full
                            </label>
                        </div>
                        <div class="simulation-hint mt-1">
                            On: the available discharge power reduces linearly from the battery bank's max. discharge rate at 10 % to 0 at 0 % usable SOC,
                            the available charge power from the max. charge rate at 90 % to 0 at 100 % usable SOC.
                            Off: full power until the hard cut-off at 0 % and 100 % usable SOC.
                        </div>
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

/* State of charge and charge speed side by side, stacked when the panel is narrow. */
.battery-settings {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(18rem, 1fr));
    gap: .5rem 1.5rem;
}

.battery-settings-full {
    grid-column: 1 / -1;
}

.charge-speed-range {
    width: 16.5rem;
    max-width: 100%;
}

.simulation-panel :deep(.input-group-sm > .form-control),
.simulation-panel :deep(.input-group-sm > .input-group-text),
.simulation-panel :deep(.input-group-sm > .btn),
.simulation-panel .btn-sm {
    font-size: .75rem;
    padding: .125rem .375rem;
}
</style>
