import { createApp } from 'vue';

// The supplied custom stylesheet (it pulls in the Bootstrap framework itself),
// followed by the application specific styles.
import './styles/bootstrap.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import './styles/site.css';

// Bootstrap JavaScript is only needed for the navbar toggler and the dropdown -
// the accordions are driven by Vue.
import 'bootstrap/js/dist/collapse';
import 'bootstrap/js/dist/dropdown';

import App from './App.vue';
import { loadConfig } from './api/config';

// The runtime configuration must be available before the first API call.
void loadConfig().then(() => {
    createApp(App).mount('#app');
});
