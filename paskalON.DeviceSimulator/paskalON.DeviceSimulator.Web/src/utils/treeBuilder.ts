import type {
    BbDto,
    DerCircuitDto,
    DerDto,
    DerGroupDto,
    DerUnitDto,
    PcsDto,
    PmBaseDto,
    PvDto
} from '@/types/der';
import { isBatteryUnit, isSolarUnit } from '@/types/der';
import type { NodeKind, PropertyRow, TreeNode } from '@/types/tree';

const icons: Record<NodeKind, string> = {
    group: 'bi-diagram-3',
    circuit: 'bi-diagram-2',
    batteryUnit: 'bi-battery-charging',
    solarUnit: 'bi-sun',
    pcs: 'bi-cpu',
    batteryBank: 'bi-battery-full',
    solarPanel: 'bi-grid-3x3',
    meterSystem: 'bi-speedometer2',
    meterAuxiliary: 'bi-speedometer',
    meterExternal: 'bi-speedometer',
    meterCircuit: 'bi-speedometer',
    folder: 'bi-collection'
};

function yesNo(value: boolean | undefined): string {
    return value ? 'Yes' : 'No';
}

function deviceProperties(device: { deviceId: number }, extra: PropertyRow[] = []): PropertyRow[] {
    return [{ name: 'Device ID', value: String(device.deviceId) }, ...extra];
}

function modbusDeviceNode(
    id: string,
    kind: NodeKind,
    typeLabel: string,
    device: PcsDto | BbDto | PvDto
): TreeNode {
    return {
        id,
        kind,
        typeLabel,
        icon: icons[kind],
        name: device.name,
        deviceId: device.deviceId,
        properties: deviceProperties(device),
        modbusEndpoints: device.endpoints ?? [],
        children: []
    };
}

function meterNode(id: string, kind: NodeKind, typeLabel: string, meter: PmBaseDto): TreeNode {
    return {
        id,
        kind,
        typeLabel,
        icon: icons[kind],
        name: meter.name,
        deviceId: meter.deviceId,
        properties: deviceProperties(meter),
        c37Endpoints: meter.endpoints ?? [],
        children: []
    };
}

function unitNode(id: string, unit: DerUnitDto): TreeNode {
    const kind: NodeKind = isBatteryUnit(unit) ? 'batteryUnit' : 'solarUnit';

    // Scalar (non list) properties of the unit.
    const properties: PropertyRow[] = [
        { name: 'Unit type', value: isBatteryUnit(unit) ? 'Battery storage' : 'Solar' },
        { name: 'Is in maintenance mode', value: yesNo(unit.isInMaintenanceMode) }
    ];

    const children: TreeNode[] = [];

    if (isBatteryUnit(unit)) {
        if (unit.powerConversionSystem) {
            children.push(
                modbusDeviceNode(
                    `${id}/pcs`,
                    'pcs',
                    'PCS',
                    unit.powerConversionSystem
                )
            );
        }

        (unit.batteryBanks ?? []).forEach((bank, index) => {
            children.push(
                modbusDeviceNode(`${id}/bb-${index}`, 'batteryBank', 'Battery bank', bank)
            );
        });
    } else if (isSolarUnit(unit)) {
        properties.push({ name: 'Number of panels', value: String(unit.numberOfPanels ?? 0) });

        if (unit.powerConversionSystem) {
            children.push(
                modbusDeviceNode(
                    `${id}/pcs`,
                    'pcs',
                    'PCS',
                    unit.powerConversionSystem
                )
            );
        }

        (unit.solarPanels ?? []).forEach((panel, index) => {
            children.push(
                modbusDeviceNode(`${id}/pv-${index}`, 'solarPanel', 'Solar panel', panel)
            );
        });
    }

    return {
        id,
        kind,
        typeLabel: isBatteryUnit(unit) ? 'Battery unit' : 'Solar unit',
        icon: icons[kind],
        name: unit.name,
        properties,
        children
    };
}

function circuitNode(id: string, circuit: DerCircuitDto): TreeNode {
    const children: TreeNode[] = (circuit.derUnits ?? []).map((unit, index) =>
        unitNode(`${id}/unit-${index}`, unit)
    );

    if (circuit.circuitPowerMeter) {
        children.push(
            meterNode(`${id}/meter`, 'meterCircuit', 'Circuit meter', circuit.circuitPowerMeter)
        );
    }

    return {
        id,
        kind: 'circuit',
        typeLabel: 'Circuit',
        icon: icons.circuit,
        name: circuit.name,
        properties: [
            { name: 'DER units', value: String((circuit.derUnits ?? []).length) },
            {
                name: 'Circuit power meter',
                value: circuit.circuitPowerMeter ? circuit.circuitPowerMeter.name : '-'
            }
        ],
        children
    };
}

function groupNode(id: string, group: DerGroupDto): TreeNode {
    return {
        id,
        kind: 'group',
        typeLabel: 'Group',
        icon: icons.group,
        name: group.name,
        properties: [{ name: 'Circuits', value: String((group.derCircuits ?? []).length) }],
        children: (group.derCircuits ?? []).map((circuit, index) =>
            circuitNode(`${id}/circuit-${index}`, circuit)
        )
    };
}

function meterFolder(
    id: string,
    name: string,
    kind: NodeKind,
    typeLabel: string,
    meters: PmBaseDto[]
): TreeNode | null {
    if (!meters || meters.length === 0) {
        return null;
    }

    return {
        id,
        kind: 'folder',
        typeLabel: 'Meters',
        icon: icons.folder,
        name,
        properties: [],
        children: meters.map((meter, index) =>
            meterNode(`${id}/meter-${index}`, kind, typeLabel, meter)
        )
    };
}

/**
 * Converts the DerDto into the recursive accordion view model.
 * Endpoint arrays are passed by reference so that edits update the loaded model.
 */
export function buildTree(der: DerDto): TreeNode[] {
    const nodes: TreeNode[] = (der.derGroups ?? []).map((group, index) =>
        groupNode(`group-${index}`, group)
    );

    const folders = [
        meterFolder(
            'system-meters',
            'System power meters',
            'meterSystem',
            'System meter',
            der.systemPowerMeters ?? []
        ),
        meterFolder(
            'auxiliary-meters',
            'Auxiliary power meters',
            'meterAuxiliary',
            'Auxiliary meter',
            der.auxiliaryPowerMeters ?? []
        ),
        meterFolder(
            'external-meters',
            'External power meters',
            'meterExternal',
            'External meter',
            der.externalPowerMeters ?? []
        )
    ];

    for (const folder of folders) {
        if (folder) {
            nodes.push(folder);
        }
    }

    return nodes;
}

/** Collects the ids of a node and all of its descendants. */
export function collectIds(nodes: TreeNode[], target: string[] = []): string[] {
    for (const node of nodes) {
        target.push(node.id);
        collectIds(node.children, target);
    }

    return target;
}

/** Number of devices (nodes with endpoints) below and including the given nodes. */
export function countDevices(nodes: TreeNode[]): number {
    return nodes.reduce((total, node) => {
        const own = node.modbusEndpoints || node.c37Endpoints ? 1 : 0;
        return total + own + countDevices(node.children);
    }, 0);
}
