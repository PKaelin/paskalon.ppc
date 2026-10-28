/** Active power as serialized by the web service (paskalON.PhysicalUnits ActivePower). */
export interface ActivePowerDto {
    watts: number;
}

/** Reactive power as serialized by the web service (paskalON.PhysicalUnits ReactivePower). */
export interface ReactivePowerDto {
    voltAmperesReactive: number;
}

/** Apparent power as serialized by the web service (paskalON.PhysicalUnits ApparentPower). */
export interface ApparentPowerDto {
    voltAmperes: number;
}

/** Energy as serialized by the web service (paskalON.PhysicalUnits Energy). */
export interface EnergyDto {
    wattHours: number;
}

/** Reactive energy as serialized by the web service (paskalON.PhysicalUnits ReactiveEnergy). */
export interface ReactiveEnergyDto {
    voltAmperesReactiveHours: number;
}

/** Summary of the system power meter pushed by the system hub (SystemMeterSummaryDto). */
export interface SystemMeterSummaryDto {
    /** Positive: producing (exporting), negative: consuming (importing). */
    activePower: ActivePowerDto;
    /** Positive: injecting (capacitive), negative: absorbing (inductive). */
    reactivePower: ReactivePowerDto;
    /** Average state of charge of the battery banks in percent, null without energy storage. */
    stateOfCharge: number | null;
    /** Power factor, signed according to the power factor standard (IEEE or IEC) of the system power meter. */
    powerFactor: number;
    /** Frequency in Hz. */
    frequency: number;
}

/** System nameplate ratings (SystemNameplateDto). */
export interface SystemNameplateDto {
    maximumActivePowerWatts: number;
    minimumActivePowerWatts: number;
    maximumReactivePowerVars: number;
    minimumReactivePowerVars: number;
}
