<script setup lang="ts">
import { computed } from 'vue';

const props = withDefaults(
    defineProps<{
        /** Title of the SPA - the Name of the DerDto. */
        title: string;
        subtitle?: string;
    }>(),
    { subtitle: '' }
);

const version = __APP_VERSION__;
const appName = __APP_DISPLAY_NAME__;

const displayTitle = computed(() => props.title || 'paskalON DeviceSimulator');
</script>

<template>
    <header>
        <nav class="navbar navbar-expand-lg navbar-dark bg-primary border-bottom box-shadow mb-0">
            <div class="container-fluid">
                <a class="navbar-brand d-flex align-items-center gap-3" href="/">
                    <img src="/images/paskalOn-logo.svg" alt="paskalON logo" />
                </a>

                <button class="navbar-toggler"
                        type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#mainNavbar"
                        aria-controls="mainNavbar"
                        aria-expanded="false"
                        aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>

                <div class="navbar-collapse collapse" id="mainNavbar">
                    <ul class="navbar-nav flex-grow-1 align-items-lg-center">
                        <li class="nav-item">
                            <a class="nav-link text-light fw-bold active" aria-current="page" href="/">Home</a>
                        </li>
                        <!--<li class="nav-item dropdown">
                            <a class="nav-link text-light fw-bold dropdown-toggle"
                               href="#"
                               id="navbarDropdown"
                               role="button"
                               data-bs-toggle="dropdown"
                               aria-haspopup="true"
                               aria-expanded="false">Admin</a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdown">
                                <a class="dropdown-item" href="#">Do something</a>
                                <a class="dropdown-item" href="#">Do something else</a>
                            </div>
                        </li>-->

                        <li class="nav-item ms-lg-4 d-flex flex-column justify-content-center">
                            <span class="app-title">{{ displayTitle }}</span>
                            <span v-if="subtitle" class="app-subtitle">{{ subtitle }}</span>
                        </li>
                    </ul>

                    <!-- Copyright and application version, right aligned in the header. -->
                    <div class="navbar-text text-light text-lg-end small">
                        <span class="d-block d-lg-inline">{{ appName }} {{ version }}</span>
                    </div>
                </div>
            </div>
        </nav>
    </header>
</template>
