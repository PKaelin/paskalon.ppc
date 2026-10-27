/**
 * Number formatting and parsing with the digit group and decimal separators of the
 * current system (browser) locale, e.g. 1’234.5 (de-CH), 1,234.5 (en-US), 1.234,5 (de-DE).
 */

const formatter = new Intl.NumberFormat(undefined, { maximumFractionDigits: 6 });

function separator(type: 'group' | 'decimal', fallback: string): string {
    return formatter.formatToParts(12345.6).find((part) => part.type === type)?.value ?? fallback;
}

/** Decimal separator of the current locale. */
const decimalSeparator = separator('decimal', '.');

/**
 * Digit group separators accepted when typing. Besides the locale's own separator a space is accepted
 * for locales grouping with a (narrow) no-break space, and an apostrophe for locales grouping with ’ (de-CH).
 */
const groupSeparators = (() => {
    const group = separator('group', ',');
    const accepted = new Set([group]);

    if (/[\s  ]/.test(group)) {
        accepted.add(' ');
        accepted.add(' ');
        accepted.add(' ');
    }

    if (group === '’') {
        accepted.add('\'');
    }

    return accepted;
})();

/** Formats a number with the separators of the current locale. */
export function formatLocaleNumber(value: number): string {
    return formatter.format(value);
}

/**
 * Parses a number typed with the separators of the current locale.
 * Returns NaN if the text is empty or not a valid number.
 */
export function parseLocaleNumber(text: string): number {
    let normalized = text.trim();

    for (const group of groupSeparators) {
        normalized = normalized.split(group).join('');
    }

    normalized = normalized.replace(decimalSeparator, '.');

    if (/^-?(\d+\.?\d*|\.\d+)$/.test(normalized) === false) {
        return Number.NaN;
    }

    return Number(normalized);
}

/** True if the key may be typed into a locale number input. */
export function isLocaleNumberKey(key: string): boolean {
    return /^[0-9]$/.test(key) || key === '-' || key === decimalSeparator || groupSeparators.has(key);
}
