import { inject, provide, ref, type InjectionKey, type Ref } from 'vue';

export type NoticeKind = 'info' | 'success' | 'warning' | 'danger';

export interface Notice {
    id: number;
    kind: NoticeKind;
    message: string;
}

export interface NotificationApi {
    notices: Ref<Notice[]>;
    notify: (message: string, kind?: NoticeKind, autoCloseMs?: number) => void;
    dismiss: (id: number) => void;
}

export const notificationKey: InjectionKey<NotificationApi> = Symbol('notifications');

export function createNotifications(): NotificationApi {
    const notices = ref<Notice[]>([]);
    let nextId = 1;

    function dismiss(id: number): void {
        notices.value = notices.value.filter((notice) => notice.id !== id);
    }

    function notify(message: string, kind: NoticeKind = 'info', autoCloseMs = 6000): void {
        const notice: Notice = { id: nextId++, kind, message };
        notices.value = [...notices.value, notice];

        if (autoCloseMs > 0) {
            setTimeout(() => dismiss(notice.id), autoCloseMs);
        }
    }

    const api: NotificationApi = { notices, notify, dismiss };
    provide(notificationKey, api);

    return api;
}

export function useNotifications(): NotificationApi {
    const api = inject(notificationKey);

    if (!api) {
        throw new Error('useNotifications() was called outside of the notification provider.');
    }

    return api;
}
