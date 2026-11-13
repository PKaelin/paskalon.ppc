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
    /** Only set for limiting operating modes, the maximum limit. */
    maximumLimit?: number;
    /** Only set for limiting operating modes with a minimum limit. */
    minimumLimit?: number | null;
}

/** SystemSetpointDto - the supervisory setpoints in watts and vars. */
export interface SystemSetpointDto {
    activePowerWatts: number;
    reactivePowerVars: number;
}

/**
 * PowerLimitReasonDto - why the applied setpoint of a power axis differs from its supervisory setpoint.
 * Ordered by priority: if several reasons apply, the highest is reported.
 */
export const PowerLimitReason = {
    None: 0,
    OperatingMode: 1,
    Ramping: 2,
    ConfigurationLimit: 3,
    AvailablePower: 4,
    LimitingMode: 5,
    NoOperatingMode: 6,
    Override: 7
} as const;

export type PowerLimitReason = (typeof PowerLimitReason)[keyof typeof PowerLimitReason];

/** CurrentSetpointsDto - the applied setpoints of the operating mode service, pushed by the system hub. */
export interface CurrentSetpointsDto {
    /** Applied (commanded) active power in watts. */
    setpointActivePowerWatts: number;
    /** Applied (commanded) reactive power in vars. */
    setpointReactivePowerVars: number;
    /** Supervisory (operator) active power setpoint in watts, null if unknown. */
    supervisorySetpointActivePowerWatts: number | null;
    /** Supervisory (operator) reactive power setpoint in vars, null if unknown. */
    supervisorySetpointReactivePowerVars: number | null;
    activePowerLimitReason: PowerLimitReason;
    reactivePowerLimitReason: PowerLimitReason;
    timestamp: string;
}
