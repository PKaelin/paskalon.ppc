/**
 * Language dependent texts of the alarm configurations.
 *
 * The alarm service sends string identifiers (e.g. {{PCS.OverTemp.Name}}) instead of texts. Until a text table
 * exists the identifier is shown as it is - this is the single place where a translation lookup plugs in.
 */
export function resolveAlarmText(identifier: string | null | undefined): string {
    return identifier ?? '';
}
