<script setup lang="ts">
/** Name of the application shown right after the logo. */
const appTitle = 'Luna';
</script>

<template>
    <header>
        <nav class="navbar navbar-dark bg-primary border-bottom box-shadow mb-0">
            <div class="container-fluid">
                <a class="navbar-brand d-flex align-items-center gap-3" href="/">
                    <img src="/images/paskalOn-logo.svg" alt="paskalON logo" />
                    <span class="app-title">{{ appTitle }}</span>
                </a>
            </div>
        </nav>
    </header>
</template>
