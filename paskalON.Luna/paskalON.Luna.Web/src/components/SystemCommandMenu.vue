<script setup lang="ts">
import { ref } from 'vue';
import { sendSystemCommand, type SystemCommand } from '../api/systemApi';
import { useNotifications } from '../composables/useNotifications';

/** One entry of the system command menu. */
interface MenuItem {
    command: SystemCommand;
    label: string;
    icon: string;
}

const items: MenuItem[] = [
    { command: 'SystemStartCommand', label: 'Send Start Command', icon: 'bi-play-circle' },
    { command: 'SystemStopCommand', label: 'Send Stop Command', icon: 'bi-stop-circle' },
    { command: 'ClearShutdownCommand', label: 'Send Clear Shutdown', icon: 'bi-arrow-counterclockwise' }
];

const { notify } = useNotifications();

/** True while a command is being sent - prevents sending a second one at the same time. */
const busy = ref(false);

async function send(item: MenuItem): Promise<void> {
    busy.value = true;

    try {
        const outcome = await sendSystemCommand(item.command);

        if (outcome.ok) {
            notify(outcome.message, 'success', 4000);
        } else {
            notify(outcome.message, 'danger', 0);
        }
    } finally {
        busy.value = false;
    }
}
</script>

<template>
    <div class="dropdown">
        <button class="btn btn-link text-light dropdown-toggle system-command-toggle"
                type="button"
                data-bs-toggle="dropdown"
                aria-expanded="false"
                title="System commands"
                :disabled="busy">
            <span v-if="busy" class="spinner-border spinner-inline" role="status" aria-hidden="true"></span>
            <i v-else class="bi bi-power" aria-hidden="true"></i>
            <span class="visually-hidden">System commands</span>
        </button>

        <ul class="dropdown-menu dropdown-menu-end">
            <li v-for="item in items" :key="item.command">
                <button class="dropdown-item" type="button" @click="send(item)">
                    <i class="bi me-2" :class="item.icon" aria-hidden="true"></i>{{ item.label }}
                </button>
            </li>
        </ul>
    </div>
</template>
