/**
 * Formatting of the property values of a device in the system tab.
 */

/** A name and the formatted value of a property. */
export interface PropertyRow {
    name: string;
    value: string;
}

export const Mega = 1_000_000;

export const Kilo = 1_000;

/** Text of a property without value. */
export const NoValue = 'None';

const formatter = new Intl.NumberFormat(undefined, { minimumFractionDigits: 4, maximumFractionDigits: 4 });

/** Formats a value in the unit (the value divided by the divisor) with 4 decimals, null is shown as "None". */
export function formatQuantity(value: number | null | undefined, divisor: number, unit: string): string {
    if (value === null || value === undefined) {
        return NoValue;
    }

    const rounded = Number((value / divisor).toFixed(4));
    const text = formatter.format(rounded === 0 ? 0 : rounded);

    return unit === '' ? text : `${text} ${unit}`;
}

/** Formats a boolean as "True" or "False", null is shown as "None". */
export function formatBoolean(value: boolean | null | undefined): string {
    if (value === null || value === undefined) {
        return NoValue;
    }

    return value ? 'True' : 'False';
}

/** Formats an enum serialized as number (looked up in the names) or as name, null is shown as "None". */
export function formatEnum(value: number | string | null | undefined, names: Record<number, string>): string {
    if (value === null || value === undefined) {
        return NoValue;
    }

    return typeof value === 'string' ? value : names[value] ?? String(value);
}
