<script setup lang="ts">
import EndpointValueInput from './EndpointValueInput.vue';
import type { C37EndpointDto } from '@/types/der';
import { writeEndpointValue } from '@/api/derApi';
import { config } from '@/api/config';
import { useNotifications } from '@/composables/useNotifications';

const props = defineProps<{
    endpoints: C37EndpointDto[];
    deviceId: number;
    deviceName: string;
}>();

const { notify } = useNotifications();

function formatSignalType(value: unknown): string {
    return value === null || value === undefined ? '-' : String(value);
}

/** C37 values are nullable doubles - keep null when the box is cleared. */
function toNullableNumber(value: unknown): number | null {
    if (value === null || value === undefined || value === '') {
        return null;
    }

    const asNumber = Number(value);
    return Number.isNaN(asNumber) ? null : asNumber;
}

async function onCommitted(endpoint: C37EndpointDto, value: unknown): Promise<void> {
    const normalized = toNullableNumber(value);
    endpoint.value = normalized;

    if (!config().enableWriteBack) {
        notify(
            `${props.deviceName} / ${endpoint.name} = ${normalized ?? 'null'} (local only - write back is disabled in config.json).`,
            'info',
            3500
        );
        return;
    }

    try {
        await writeEndpointValue({
            deviceId: props.deviceId,
            name: endpoint.name,
            value: normalized
        });

        notify(`${props.deviceName} / ${endpoint.name} written.`, 'success', 2500);
    } catch (error) {
        notify(error instanceof Error ? error.message : String(error), 'danger', 8000);
    }
}
</script>

<template>
    <div v-if="endpoints.length > 0">
        <div class="grid-caption">C37 endpoints ({{ endpoints.length }})</div>
        <div class="table-responsive">
            <table class="table table-sm table-bordered table-hover table-endpoints mb-0" style="max-width: 1000px">
                <thead>
                    <tr>
                        <th scope="col">Name</th>
                        <th scope="col">Endpoint</th>
                        <th scope="col" class="col-datatype">Signal type</th>
                        <th scope="col" class="col-value">Value</th>
                        <th scope="col" class="col-flags">Access</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(endpoint, index) in endpoints" :key="`${endpoint.name}-${index}`">
                        <td>{{ endpoint.name }}</td>
                        <td>{{ endpoint.endpoint }}</td>
                        <td class="font-monospace">{{ formatSignalType(endpoint.signalType) }}</td>
                        <td>
                            <EndpointValueInput :model-value="endpoint.value ?? ''"
                                                :read-only="endpoint.isReadOnly"
                                                :label="`${deviceName} ${endpoint.name} value`"
                                                @committed="(value) => onCommitted(endpoint, value)" />
                        </td>
                        <td>
                            <span :class="[endpoint.isReadOnly ? 'bg-dark' : 'bg-secondary']">
                                {{ endpoint.isReadOnly ? 'read only' : 'writeable' }}
                            </span>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
