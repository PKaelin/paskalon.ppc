import { createApp } from 'vue';

// The custom stylesheet (it pulls in the Bootstrap framework itself),
// followed by the application specific styles.
import './styles/bootstrap.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import './styles/site.css';

// Bootstrap JavaScript is only needed for the dropdown of the system command menu.
import 'bootstrap/js/dist/dropdown';

import App from './App.vue';
import { loadConfig } from './api/config';

// The runtime configuration must be available before the first API call.
void loadConfig().then(() => {
    createApp(App).mount('#app');
});
