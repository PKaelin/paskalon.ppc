/// <reference types="vite/client" />

declare const __APP_VERSION__: string;
declare const __APP_DISPLAY_NAME__: string;
declare const __BUILD_DATE__: string;

declare module '*.vue' {
    import type { DefineComponent } from 'vue';
    const component: DefineComponent<{}, {}, any>;
    export default component;
}
