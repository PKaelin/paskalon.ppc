import type { DerCircuitDto, DerDto, DerGroupDto, DerUnitDto, DeviceDto } from '../types/der';

/** The DTO class a node of the object tree stands for. */
export type DerNodeType =
    | 'PmSystemDto'
    | 'PmAuxiliaryDto'
    | 'PmExternalDto'
    | 'DerGroupDto'
    | 'DerCircuitDto'
    | 'PmCircuitDto'
    | 'DerBatteryStorageUnitDto'
    | 'DerSolarUnitDto'
    | 'PcsDto'
    | 'BbDto'
    | 'PvDto';

/** A node of the object tree. */
export interface DerTreeNode {
    /** Path of the node, unique within the tree (e.g. 'group:0/circuit:1/unit:0/pcs'). */
    id: string;
    name: string;
    type: DerNodeType;
    children: DerTreeNode[];
}

/** Part of a name that matches the filter text. */
export interface NameMatch {
    start: number;
    length: number;
}

/** Bootstrap icon of each node type. */
export const derNodeIcons: Record<DerNodeType, string> = {
    PmSystemDto: 'bi-speedometer',
    PmAuxiliaryDto: 'bi-speedometer',
    PmExternalDto: 'bi-speedometer',
    DerGroupDto: 'bi-collection',
    DerCircuitDto: 'bi-diagram-2',
    PmCircuitDto: 'bi-speedometer',
    DerBatteryStorageUnitDto: 'bi-battery-charging',
    DerSolarUnitDto: 'bi-sun',
    PcsDto: 'bi-cpu',
    BbDto: 'bi-battery-full',
    PvDto: 'bi-grid-3x3'
};

function deviceNodes(prefix: string, devices: DeviceDto[] | null | undefined, type: DerNodeType): DerTreeNode[] {
    return (devices ?? []).map((device, index) => ({ id: `${prefix}:${index}`, name: device.name, type, children: [] }));
}

function unitNode(prefix: string, unit: DerUnitDto): DerTreeNode {
    const pcs: DerTreeNode = { id: `${prefix}/pcs`, name: unit.powerConversionSystem.name, type: 'PcsDto', children: [] };

    if (unit.unitType === 'battery') {
        return { id: prefix, name: unit.name, type: 'DerBatteryStorageUnitDto', children: [pcs, ...deviceNodes(`${prefix}/bb`, unit.batteryBanks, 'BbDto')] };
    }

    return { id: prefix, name: unit.name, type: 'DerSolarUnitDto', children: [pcs, ...deviceNodes(`${prefix}/pv`, unit.solarPanels, 'PvDto')] };
}

function circuitNode(prefix: string, circuit: DerCircuitDto): DerTreeNode {
    const meter = circuit.circuitPowerMeter;
    const children: DerTreeNode[] = meter === null || meter === undefined
        ? []
        : [{ id: `${prefix}/pm-circuit`, name: meter.name, type: 'PmCircuitDto', children: [] }];

    (circuit.derUnits ?? []).forEach((unit, index) => children.push(unitNode(`${prefix}/unit:${index}`, unit)));

    return { id: prefix, name: circuit.name, type: 'DerCircuitDto', children };
}

function groupNode(prefix: string, group: DerGroupDto): DerTreeNode {
    const children = (group.derCircuits ?? []).map((circuit, index) => circuitNode(`${prefix}/circuit:${index}`, circuit));

    return { id: prefix, name: group.name, type: 'DerGroupDto', children };
}

/** Builds the object tree of a DER: the power meters of the system followed by the DER groups. */
export function buildDerTree(der: DerDto): DerTreeNode[] {
    return [
        ...deviceNodes('pm-system', der.systemPowerMeters, 'PmSystemDto'),
        ...deviceNodes('pm-auxiliary', der.auxiliaryPowerMeters, 'PmAuxiliaryDto'),
        ...deviceNodes('pm-external', der.externalPowerMeters, 'PmExternalDto'),
        ...(der.derGroups ?? []).map((group, index) => groupNode(`group:${index}`, group))
    ];
}

/** The part of the name that matches the filter text (case insensitive), or null if it does not match. */
export function findMatch(name: string, filter: string): NameMatch | null {
    const text = filter.trim().toLowerCase();

    if (text === '') {
        return null;
    }

    const start = name.toLowerCase().indexOf(text);

    return start < 0 ? null : { start, length: text.length };
}

/**
 * The nodes whose name matches the filter text, together with their parents so the place in the tree stays visible.
 * Returns the tree unchanged if the filter text is empty.
 */
export function filterDerTree(nodes: DerTreeNode[], filter: string): DerTreeNode[] {
    if (filter.trim() === '') {
        return nodes;
    }

    const result: DerTreeNode[] = [];

    for (const node of nodes) {
        const children = filterDerTree(node.children, filter);

        if (children.length > 0 || findMatch(node.name, filter) !== null) {
            result.push({ ...node, children });
        }
    }

    return result;
}

/** Finds a node by its id, or undefined if the tree has no such node. */
export function findDerNode(nodes: DerTreeNode[], id: string): DerTreeNode | undefined {
    for (const node of nodes) {
        if (node.id === id) {
            return node;
        }

        const found = findDerNode(node.children, id);

        if (found !== undefined) {
            return found;
        }
    }

    return undefined;
}
