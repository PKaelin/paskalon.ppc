/**
 * Rules of the operating mode stack, a mirror of OperatingModeStackValidator of the operating mode service.
 * They let the UI refuse invalid drops and put dropped items at a valid position before anything is sent.
 * The operating mode service validates the stack again when it is applied.
 */
import { StackType } from '../types/operatingModes';
import type { OperatingModeStackDto } from '../types/operatingModes';
import { toFlags } from './stackType';

type Axis = 'active' | 'reactive';
type Category = 'Base' | 'Exclusive' | 'Additive' | 'Limiting';

const axes: Axis[] = ['active', 'reactive'];

const flagsByAxis: Record<Axis, Record<Category, number>> = {
    active: {
        Base: StackType.BaseActive,
        Exclusive: StackType.ExclusiveActive,
        Additive: StackType.AdditiveActive,
        Limiting: StackType.LimitingActive
    },
    reactive: {
        Base: StackType.BaseReactive,
        Exclusive: StackType.ExclusiveReactive,
        Additive: StackType.AdditiveReactive,
        Limiting: StackType.LimitingReactive
    }
};

/** Result of a proposed change of the stack. */
export type StackProposal =
    | { ok: true; stack: OperatingModeStackDto[]; replaced: OperatingModeStackDto[] }
    | { ok: false; reason: string };

function has(mode: OperatingModeStackDto, axis: Axis, category: Category): boolean {
    return (toFlags(mode.type) & flagsByAxis[axis][category]) !== 0;
}

/** True for a base or exclusive mode of the axis - the foundation the other modes of the axis build on. */
function isFoundation(mode: OperatingModeStackDto, axis: Axis): boolean {
    return has(mode, axis, 'Base') || has(mode, axis, 'Exclusive');
}

function isAnyFoundation(mode: OperatingModeStackDto): boolean {
    return axes.some((axis) => isFoundation(mode, axis));
}

/** Group of a mode in the stack: base/exclusive modes first, then additive modes, then limiting modes. */
function rank(mode: OperatingModeStackDto): number {
    if (isAnyFoundation(mode)) {
        return 0;
    }

    if (axes.some((axis) => has(mode, axis, 'Additive'))) {
        return 1;
    }

    return axes.some((axis) => has(mode, axis, 'Limiting')) ? 2 : 1;
}

/** Puts the modes in group order (base/exclusive, additive, limiting), keeping the order within a group. */
function toGroupOrder(modes: OperatingModeStackDto[]): OperatingModeStackDto[] {
    return modes
        .map((mode, index) => ({ mode, index }))
        .sort((left, right) => rank(left.mode) - rank(right.mode) || left.index - right.index)
        .map((entry) => entry.mode);
}

/**
 * Validates a stack. Returns the reason if it is invalid, otherwise null.
 * An empty stack is valid (the system ramps down to 0). Besides the rules of the operating mode service a stack
 * that is not empty needs at least one base or exclusive mode.
 */
export function validateStack(modes: OperatingModeStackDto[]): string | null {
    if (modes.length === 0) {
        return null;
    }

    if (modes.some(isAnyFoundation) === false) {
        return 'The stack needs at least one base or exclusive operating mode.';
    }

    if (new Set(modes.map((mode) => mode.operatingModeName)).size !== modes.length) {
        return 'The stack contains duplicate operating mode names.';
    }

    for (const axis of axes) {
        const count = (category: Category): number => modes.filter((mode) => has(mode, axis, category)).length;

        const multipleClassifications = modes.find((mode) =>
            (['Base', 'Exclusive', 'Additive', 'Limiting'] as Category[]).filter((category) => has(mode, axis, category)).length > 1);

        if (multipleClassifications) {
            return `Operating mode '${multipleClassifications.operatingModeName}' has multiple ${axis}-power classifications.`;
        }

        if (count('Exclusive') > 1) {
            return `Only one exclusive ${axis} mode is allowed.`;
        }

        if (count('Exclusive') > 0 && count('Base') + count('Additive') + count('Limiting') > 0) {
            return `An exclusive ${axis} mode cannot be combined with base, additive or limiting ${axis} modes.`;
        }

        if (count('Base') > 1) {
            return `Only one ${axis}-power base mode is allowed.`;
        }

        if (count('Additive') + count('Limiting') > 0 && count('Base') !== 1) {
            return `${axis === 'active' ? 'Active' : 'Reactive'}-power additive and limiting modes require a base ${axis} mode.`;
        }

        // Ordering of the axis: the base mode first, then all additive modes, then all limiting modes
        const axisModes = modes.filter((mode) => has(mode, axis, 'Base') || has(mode, axis, 'Additive') || has(mode, axis, 'Limiting'));
        const firstLimiting = axisModes.findIndex((mode) => has(mode, axis, 'Limiting'));

        if (axisModes.length > 0 && has(axisModes[0], axis, 'Base') === false) {
            return `The first ${axis}-power mode in the stack must be a base mode.`;
        }

        if (firstLimiting >= 0 && axisModes.slice(firstLimiting).some((mode) => has(mode, axis, 'Additive'))) {
            return `An additive ${axis} mode cannot follow a limiting ${axis} mode in the stack.`;
        }
    }

    return null;
}

/**
 * Proposes dropping a mode into the stack at an index.
 * A base or exclusive mode coming from the available modes replaces the base or exclusive modes of the same axes, they go
 * back to the available modes. A mode dropped at a wrong position is moved into its group (base/exclusive, additive, limiting).
 * Everything else that breaks a rule is refused.
 */
export function proposeDrop(stack: OperatingModeStackDto[], mode: OperatingModeStackDto, index: number, isFromStack: boolean): StackProposal {
    const others = stack.filter((entry) => entry.operatingModeName !== mode.operatingModeName);
    const replaced = isFromStack
        ? []
        : others.filter((entry) => axes.some((axis) => isFoundation(entry, axis) && isFoundation(mode, axis)));
    let modes: OperatingModeStackDto[];

    if (replaced.length > 0) {
        // Like a replacement in the operating mode service, the mode takes the position of the (first) replaced mode
        modes = others.flatMap((entry) => {
            if (entry === replaced[0]) {
                return [mode];
            }

            return replaced.includes(entry) ? [] : [entry];
        });
    } else {
        modes = [...others];
        modes.splice(Math.min(Math.max(index, 0), modes.length), 0, mode);
    }

    const ordered = toGroupOrder(modes);
    const reason = validateStack(ordered);

    return reason === null ? { ok: true, stack: ordered, replaced } : { ok: false, reason };
}

/** Proposes removing a mode from the stack (dropping it on the available modes). */
export function proposeRemove(stack: OperatingModeStackDto[], mode: OperatingModeStackDto): StackProposal {
    const modes = stack.filter((entry) => entry.operatingModeName !== mode.operatingModeName);
    const reason = validateStack(modes);

    return reason === null ? { ok: true, stack: modes, replaced: [] } : { ok: false, reason };
}
