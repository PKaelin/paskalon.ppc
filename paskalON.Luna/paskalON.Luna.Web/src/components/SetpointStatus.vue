<script setup lang="ts">
import { computed } from 'vue';
import { useSetpointStatus } from '../composables/useSetpointStatus';
import { filterActiveAlarms } from '../composables/useAlarmFilter';
import { showTab } from '../composables/useNavigation';
import { PowerLimitReason } from '../types/operatingModes';
import { formatQuantity, Mega, NoValue } from '../utils/deviceValues';

/** How a reason is shown: neutral information, a warning or a forced state. */
type ReasonLevel = 'info' | 'warning' | 'danger';

/** One power axis: the operator setpoint, what is commanded to the plant and why they differ. */
interface AxisStatus {
    label: string;
    unit: string;
    setpoint: string;
    commanded: string;
    reason: PowerLimitReason;
    text: string | null;
    level: ReasonLevel;
}

const { status, setpoints } = useSetpointStatus();

/** Explains the reason in the words of the operator, the direction tells whether the plant cannot produce or consume. */
function describe(reason: PowerLimitReason, setpoint: number | null): { text: string | null; level: ReasonLevel } {
    const isConsuming = setpoint !== null && setpoint < 0;

    switch (reason) {
        case PowerLimitReason.OperatingMode:
            return { text: 'Set by the operating modes (e.g. feedback, droop, MPPT)', level: 'info' };
        case PowerLimitReason.Ramping:
            return { text: 'Ramping to the setpoint', level: 'info' };
        case PowerLimitReason.ConfigurationLimit:
            return { text: 'Limited by the configured limits of the operating mode or nameplate', level: 'warning' };
        case PowerLimitReason.AvailablePower:
            return {
                text: isConsuming
                    ? 'Not enough power available to charge/consume (e.g. battery full)'
                    : 'Not enough power available to discharge/produce (e.g. battery empty, low production)',
                level: 'warning'
            };
        case PowerLimitReason.LimitingMode:
            return { text: 'Limited by a limiting operating mode', level: 'warning' };
        case PowerLimitReason.NoOperatingMode:
            return { text: 'No operating mode controls this power', level: 'warning' };
        case PowerLimitReason.Override:
            return { text: 'Forced by an override operating mode (e.g. shutdown)', level: 'danger' };
        default:
            return { text: null, level: 'info' };
    }
}

function axisStatus(label: string, setpoint: number | null, commanded: number, reason: PowerLimitReason, unit: string): AxisStatus {
    const { text, level } = describe(reason, setpoint);

    return {
        label,
        unit,
        setpoint: formatQuantity(setpoint, Mega, ''),
        commanded: formatQuantity(commanded, Mega, ''),
        reason,
        text,
        level
    };
}

const axes = computed<AxisStatus[]>(() => {
    const value = setpoints.value;

    if (value === null) {
        return [];
    }

    return [
        axisStatus('Active power', value.supervisorySetpointActivePowerWatts, value.setpointActivePowerWatts, value.activePowerLimitReason, 'MW'),
        axisStatus('Reactive power', value.supervisorySetpointReactivePowerVars, value.setpointReactivePowerVars, value.reactivePowerLimitReason, 'Mvar')
    ];
});

/** Shows the active alarms, e.g. the battery state of charge alarm that explains the missing available power. */
function showActiveAlarms(): void {
    filterActiveAlarms();
    showTab('alarms');
}
</script>

<template>
    <section class="om-status" aria-label="Commanded setpoints">
        <p v-if="setpoints === null" class="om-hint mb-0">
            <template v-if="status === 'connected'">Waiting for the setpoints of the operating mode service...</template>
            <template v-else>The setpoints cannot be received ({{ status }}).</template>
        </p>

        <table v-else class="om-status-table">
            <thead>
                <tr>
                    <th scope="col"></th>
                    <th scope="col" class="text-center" colspan="2">Setpoint</th>
                    <th scope="col" class="text-center" colspan="2">Commanded</th>
                </tr>
            </thead>
            <tbody>
                <template v-for="axis in axes" :key="axis.label">
                    <tr>
                        <th scope="row">{{ axis.label }}</th>
                        <!-- Number and unit in separate cells: the numbers are right-aligned with a fixed number of decimals, so the decimal points line up across the rows. -->
                        <td class="om-status-number">{{ axis.setpoint }}</td>
                        <td class="om-status-unit">{{ axis.setpoint === NoValue ? '' : axis.unit }}</td>
                        <td class="om-status-number" :class="axis.text !== null ? `om-status-differs om-status-reason-${axis.level}` : ''">{{ axis.commanded }}</td>
                        <td class="om-status-unit">{{ axis.commanded === NoValue ? '' : axis.unit }}</td>
                    </tr>
                    <tr v-if="axis.text !== null" class="om-status-reason-row">
                        <td colspan="5">
                            <span class="om-status-reason" :class="`om-status-reason-${axis.level}`">
                                <i class="bi me-1" aria-hidden="true"
                                   :class="axis.level === 'info' ? 'bi-info-circle' : 'bi-exclamation-triangle'"></i>{{ axis.text }}
                            </span>
                            <button v-if="axis.reason === PowerLimitReason.AvailablePower"
                                    class="btn btn-link btn-sm p-0 ms-2 align-baseline"
                                    type="button"
                                    @click="showActiveAlarms">
                                Show active alarms
                            </button>
                        </td>
                    </tr>
                </template>
            </tbody>
        </table>

        <p v-if="setpoints !== null && status !== 'connected'" class="om-hint mt-1 mb-0">
            <i class="bi bi-wifi-off me-1" aria-hidden="true"></i>Connection lost ({{ status }}), the values may be outdated.
        </p>
    </section>
</template>
