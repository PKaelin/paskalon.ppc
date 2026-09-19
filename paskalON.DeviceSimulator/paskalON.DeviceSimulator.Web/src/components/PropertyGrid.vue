<script setup lang="ts">
import type { PropertyRow } from '@/types/tree';

defineProps<{ rows: PropertyRow[]; caption?: string }>();
</script>

<template>
    <div v-if="rows.length > 0" class="mb-3">
        <div class="grid-caption">{{ caption ?? 'Properties' }}</div>
        <table class="table table-sm table-bordered table-properties mb-0">
            <tbody>
                <tr v-for="row in rows" :key="row.name">
                    <th scope="row">{{ row.name }}</th>
                    <td>{{ row.value }}</td>
                </tr>
            </tbody>
        </table>
    </div>
</template>
