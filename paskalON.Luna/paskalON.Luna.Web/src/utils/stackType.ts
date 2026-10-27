import { StackType, type StackTypeName } from '../types/operatingModes';

/** Power axis an operating mode controls - decides the colour of its list item. */
export type PowerAxis = 'active' | 'reactive' | 'both' | 'none';

/** Category of an operating mode with the glyph shown in the corner of its list item. */
export interface StackCategory {
    name: 'Base' | 'Exclusive' | 'Additive' | 'Limiting';
    glyph: string;
    active: number;
    reactive: number;
}

const categories: StackCategory[] = [
    { name: 'Base', glyph: 'B', active: StackType.BaseActive, reactive: StackType.BaseReactive },
    { name: 'Exclusive', glyph: 'E', active: StackType.ExclusiveActive, reactive: StackType.ExclusiveReactive },
    { name: 'Additive', glyph: '+', active: StackType.AdditiveActive, reactive: StackType.AdditiveReactive },
    { name: 'Limiting', glyph: '-|', active: StackType.LimitingActive, reactive: StackType.LimitingReactive }
];

const allActive = categories.reduce((flags, category) => flags | category.active, 0);
const allReactive = categories.reduce((flags, category) => flags | category.reactive, 0);

/** Converts the stack type as serialized by the service (number or comma separated names) into flags. */
export function toFlags(type: number | string): number {
    if (typeof type === 'number') {
        return type;
    }

    return type
        .split(',')
        .map((name) => StackType[name.trim() as StackTypeName] ?? 0)
        .reduce((flags, flag) => flags | flag, 0);
}

/** The categories (Base, Exclusive, Additive, Limiting) of an operating mode. */
export function getCategories(type: number | string): StackCategory[] {
    const flags = toFlags(type);

    return categories.filter((category) => (flags & (category.active | category.reactive)) !== 0);
}

/** The stack type names of a category an operating mode has, e.g. ['ExclusiveActive', 'ExclusiveReactive']. */
export function getTypeNames(type: number | string, category: StackCategory): StackTypeName[] {
    const flags = toFlags(type);
    const names: StackTypeName[] = [];

    if ((flags & category.active) !== 0) {
        names.push(`${category.name}Active`);
    }

    if ((flags & category.reactive) !== 0) {
        names.push(`${category.name}Reactive`);
    }

    return names;
}


/** The power axis (active, reactive or both) of an operating mode. */
export function getAxis(type: number | string): PowerAxis {
    const flags = toFlags(type);
    const isActive = (flags & allActive) !== 0;
    const isReactive = (flags & allReactive) !== 0;

    if (isActive && isReactive) {
        return 'both';
    }

    if (isActive) {
        return 'active';
    }

    return isReactive ? 'reactive' : 'none';
}
