/**
 * Summary of the power conversion systems (PCSs) of DER units, shown for a DER circuit and a DER group in the system tab.
 */

import type { DerUnitDto, PcsCoreDto } from '../types/der';
import { communicationErrorRow, formatQuantity, Mega, type PropertyRow } from './deviceValues';

/** Rows of the left and the right column of the PCS summary. */
export interface PcsSummaryRows {
    leftRows: PropertyRow[];
    rightRows: PropertyRow[];
}

/** Sum of the values that are present, null if no value is present. */
function sum(values: (number | null | undefined)[]): number | null {
    const present = values.filter((value): value is number => value !== null && value !== undefined);

    return present.length === 0 ? null : present.reduce((total, value) => total + value, 0);
}

/**
 * Rows of the PCS summary of the DER units (battery storage and solar): the powers summed up over all PCSs that
 * have a value ("None" if no PCS has one), and a communication error if any PCS has one.
 * A PCS without core yet does not count, like in DerBatteryStorageUnitDto.
 */
export function pcsSummaryRows(units: DerUnitDto[]): PcsSummaryRows {
    const cores = units
        .map(unit => unit.powerConversionSystem?.core ?? null)
        .filter((core): core is PcsCoreDto => core !== null);

    return {
        leftRows: [
            // The first row of the left column stays empty, so the sums line up with the right column.
            { name: '', value: ' ' },
            { name: 'Active Power Target', value: formatQuantity(sum(cores.map(core => core.activePowerTarget?.watts)), Mega, 'MW') },
            { name: 'Active Power', value: formatQuantity(sum(cores.map(core => core.activePower?.watts)), Mega, 'MW') },
            { name: 'Available Active Power', value: formatQuantity(sum(cores.map(core => core.activeAvailablePower?.watts)), Mega, 'MW') }
        ],
        rightRows: [
            communicationErrorRow(cores.some(core => core.communicationError === true)),
            { name: 'Reactive Power Target', value: formatQuantity(sum(cores.map(core => core.reactivePowerTarget?.voltAmperesReactive)), Mega, 'MVAr') },
            { name: 'Reactive Power', value: formatQuantity(sum(cores.map(core => core.reactivePower?.voltAmperesReactive)), Mega, 'MVAr') },
            { name: 'Available Reactive Power', value: formatQuantity(sum(cores.map(core => core.reactiveAvailablePower?.voltAmperesReactive)), Mega, 'MVAr') }
        ]
    };
}
