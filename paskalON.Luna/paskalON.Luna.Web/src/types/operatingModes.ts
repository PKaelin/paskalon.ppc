/**
 * TypeScript mirror of the operating mode DTOs (paskalON.OperatingModes.Dto).
 */

/** Flags of OperatingModeStackTypeDto. */
export const StackType = {
    Unknown: 0,
    AdditiveActive: 1 << 0,
    AdditiveReactive: 1 << 1,
    BaseActive: 1 << 2,
    BaseReactive: 1 << 3,
    ExclusiveActive: 1 << 4,
    ExclusiveReactive: 1 << 5,
    LimitingActive: 1 << 6,
    LimitingReactive: 1 << 7
} as const;

export type StackTypeName = keyof typeof StackType;

/** OperatingModeStackDto (and the derived OperatingModeLimitingStackDto). */
export interface OperatingModeStackDto {
    operatingModeName: string;
    /** Flags - a number, or "BaseActive, BaseReactive" if the service serializes enums as strings. */
    type: number | string;
    /** Only set for active operating modes. */
    priority?: number | null;
    /** Only set for limiting operating modes. */
    limit?: number;
}

/** SystemSetpointDto - the supervisory setpoints in watts and vars. */
export interface SystemSetpointDto {
    activePowerWatts: number;
    reactivePowerVars: number;
}
