<script setup lang="ts">
const year = new Date().getFullYear();
const version = __APP_VERSION__;
const appName = __APP_DISPLAY_NAME__;
</script>

<template>
    <footer class="border-top footer text-muted mt-4">
        <div class="container-fluid">
            Copyright &copy; {{ year }} paskalON &ndash; {{ appName }} {{ version }} 233333
        </div>
    </footer>
</template>
