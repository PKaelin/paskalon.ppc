import type { C37EndpointDto, ModbusEndpointDto } from './der';

/** What a node represents. Used for the badge and the icon in the accordion header. */
export type NodeKind =
    | 'group'
    | 'circuit'
    | 'batteryUnit'
    | 'solarUnit'
    | 'pcs'
    | 'batteryBank'
    | 'solarPanel'
    | 'meterSystem'
    | 'meterAuxiliary'
    | 'meterExternal'
    | 'meterCircuit'
    | 'folder';

/** A single row of the scalar property grid (everything that is not a list). */
export interface PropertyRow {
    name: string;
    value: string;
}

/**
 * One accordion section. The whole UI is rendered recursively from this structure,
 * which keeps the search/filter logic in a single place.
 *
 * `modbusEndpoints` / `c37Endpoints` keep the REFERENCE to the arrays of the loaded
 * DerDto, so edits in the grids write straight back into the reactive model.
 */
export interface TreeNode {
    /** Stable id (path based) - used as the key for the expand/collapse state. */
    id: string;
    kind: NodeKind;
    name: string;
    /** Badge text, e.g. "Group", "Battery unit", "PCS". */
    typeLabel: string;
    /** Bootstrap icon class for the header. */
    icon: string;
    /** Scalar properties shown in the property grid. */
    properties: PropertyRow[];
    /** Device id - only set for devices (PCS, battery bank, PV panel, power meter). */
    deviceId?: number;
    modbusEndpoints?: ModbusEndpointDto[];
    c37Endpoints?: C37EndpointDto[];
    children: TreeNode[];
}

/** Node kinds that represent a DerUnitDto. */
export const unitKinds: readonly NodeKind[] = ['batteryUnit', 'solarUnit'];

/** Node kinds that represent a device (PcsDto, BbDto, PvDto, Pm*Dto). */
export const deviceKinds: readonly NodeKind[] = [
    'pcs',
    'batteryBank',
    'solarPanel',
    'meterSystem',
    'meterAuxiliary',
    'meterExternal',
    'meterCircuit'
];

export function isUnitNode(node: TreeNode): boolean {
    return unitKinds.includes(node.kind);
}

export function isDeviceNode(node: TreeNode): boolean {
    return deviceKinds.includes(node.kind);
}
