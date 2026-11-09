/**
 * Language dependent texts of the alarm configurations.
 *
 * The alarm service sends localization keys (e.g. {{PCS.CommunicationError.Name}}) instead of texts and owns the texts
 * of the keys. They are loaded once at the start (retried until the alarm service answers) and resolved here:
 * - A value written as {{Key}} is shown with the text of the key, or as it is while the texts are not loaded or the key has no text.
 * - Any other value is plain text and shown as it is.
 * The texts are reactive, a component that shows a resolved text is updated when the texts are loaded.
 */
import { shallowRef } from 'vue';
import { getAlarmTexts } from '../api/alarmApi';

/** Language of the alarm texts. English only for now, later the language of the user. */
export const alarmTextLanguage = 'en';

/** Time before a failed load of the texts is retried. */
const retryDelayMilliseconds = 30_000;

/** A value that is a localization key: {{Key}} without braces in the key. */
const keyPattern = /^\{\{([^{}]+)\}\}$/;

/** Texts by their localization key, null until they are loaded. */
const texts = shallowRef<Readonly<Record<string, string>> | null>(null);

/** Keys without a text that were already reported, so that each missing key is reported once. */
const reportedMissingKeys = new Set<string>();

/** The running or finished load of the texts. */
let loading: Promise<void> | null = null;

/** Waits for the time. */
function delay(milliseconds: number): Promise<void> {
    return new Promise((resolve) => setTimeout(resolve, milliseconds));
}

/** Loads the texts and retries until the alarm service answers. */
async function loadUntilLoaded(): Promise<void> {
    for (;;) {
        try {
            const dto = await getAlarmTexts(alarmTextLanguage);
            texts.value = dto.texts;

            return;
        } catch (error) {
            const reason = error instanceof Error ? error.message : String(error);
            console.warn(`The alarm texts could not be loaded, retry in ${retryDelayMilliseconds / 1000} s: ${reason}`);
            await delay(retryDelayMilliseconds);
        }
    }
}

/**
 * Starts loading the alarm texts once, further calls return the same load.
 * The returned promise resolves when the texts are loaded.
 */
export function loadAlarmTexts(): Promise<void> {
    if (loading === null) {
        loading = loadUntilLoaded();
    }

    return loading;
}

/** Shows a value of an alarm configuration: the text of a localization key or the plain text. */
export function resolveAlarmText(value: string | null | undefined): string {
    if (value === null || value === undefined) {
        return '';
    }

    const match = keyPattern.exec(value);

    if (match === null) {
        return value;
    }

    const loadedTexts = texts.value;

    if (loadedTexts === null) {
        return value;
    }

    const text = loadedTexts[match[1]];

    if (text !== undefined) {
        return text;
    }

    if (reportedMissingKeys.has(match[1]) === false) {
        reportedMissingKeys.add(match[1]);
        console.warn(`The alarm text key ${value} has no text in the language ${alarmTextLanguage}.`);
    }

    return value;
}
