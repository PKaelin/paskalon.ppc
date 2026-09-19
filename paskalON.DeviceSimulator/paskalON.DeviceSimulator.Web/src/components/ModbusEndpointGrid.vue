<script setup lang="ts">
import EndpointValueInput from './EndpointValueInput.vue';
import type { ModbusEndpointDto } from '@/types/der';
import { writeEndpointValue } from '@/api/derApi';
import { config } from '@/api/config';
import { useNotifications } from '@/composables/useNotifications';

const props = defineProps<{
    endpoints: ModbusEndpointDto[];
    deviceId: number;
    deviceName: string;
}>();

const { notify } = useNotifications();

function formatDataType(value: unknown): string {
    return value === null || value === undefined ? '-' : String(value);
}

async function onCommitted(endpoint: ModbusEndpointDto, value: unknown): Promise<void> {
    if (!config().enableWriteBack) {
        notify(
            `${props.deviceName} / ${endpoint.name} = ${String(value)} (local only - write back is disabled in config.json).`,
            'info',
            3500
        );
        return;
    }

    try {
        await writeEndpointValue({
            deviceId: props.deviceId,
            name: endpoint.name,
            address: endpoint.address,
            value
        });

        notify(`${props.deviceName} / ${endpoint.name} written.`, 'success', 2500);
    } catch (error) {
        notify(error instanceof Error ? error.message : String(error), 'danger', 8000);
    }
}
</script>

<template>
    <div v-if="endpoints.length > 0">
        <div class="grid-caption">Modbus endpoints ({{ endpoints.length }})</div>
        <div class="table-responsive">
            <table class="table table-sm table-bordered table-hover table-endpoints mb-0"; style="max-width:1000px;">
                <thead>
                    <tr>
                        <th scope="col" class="col-address">Address</th>
                        <th scope="col">Name</th>
                        <th scope="col" class="col-datatype">Data type</th>
                        <th scope="col" class="col-value">Value</th>
                        <th scope="col" class="col-flags">Access</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(endpoint, index) in endpoints"
                        :key="`${endpoint.address}-${endpoint.name}-${index}`">
                        <td class="font-monospace">{{ endpoint.address }}</td>
                        <td>{{ endpoint.name }}</td>
                        <td class="font-monospace">{{ formatDataType(endpoint.dataType) }}</td>
                        <td>
                            <EndpointValueInput v-model="endpoint.value"
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
