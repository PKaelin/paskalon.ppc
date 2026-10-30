<script setup lang="ts">
import { computed } from 'vue';
import { useDependencyStatus } from '../composables/useDependencyStatus';

/** State of an LED: unknown while the Luna service does not deliver the availability. */
type LedState = 'available' | 'unavailable' | 'unknown';

/** One monitored dependency shown as LED. */
interface DependencyLed {
    /** Name of the dependency as pushed by the dependency hub. */
    name: string;
    /** Abbreviation shown above the LED. */
    label: string;
    /** Name shown in the tooltip. */
    title: string;
}

const dependencies: DependencyLed[] = [
    { name: 'OperatingMode', label: 'OM', title: 'Operating Mode' },
    { name: 'PowerControl', label: 'PC', title: 'Power Control' },
    { name: 'DeviceService', label: 'DS', title: 'Device Service' },
    { name: 'Redis', label: 'RE', title: 'Redis' }
];

const stateText: Record<LedState, string> = {
    available: 'available',
    unavailable: 'not available',
    unknown: 'unknown'
};

const statuses = useDependencyStatus();

const leds = computed(() => dependencies.map((dependency) => {
    const status = statuses.value.find((item) => item.name === dependency.name);
    let state: LedState = 'unknown';

    if (status !== undefined) {
        state = status.isAvailable ? 'available' : 'unavailable';
    }

    return { ...dependency, state, description: `${dependency.title}: ${stateText[state]}` };
}));
</script>

<template>
    <div class="dependency-leds">
        <div v-for="led in leds"
             :key="led.name"
             class="dependency-led"
             role="img"
             :title="led.description"
             :aria-label="led.description">
            <span class="dependency-led-label">{{ led.label }}</span>
            <span class="dependency-led-light" :class="`dependency-led-${led.state}`"></span>
        </div>
    </div>
</template>
