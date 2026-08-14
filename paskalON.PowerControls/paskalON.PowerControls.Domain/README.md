# Power Control Domain


## Power distribution
The power control distributes the systems power using the following steps:
1. System gets the system target.
2. System constraints are applied to the system target.
3. The configured distribution algorithm is used to distribute the power between the units.
4. Unit constraints are applied to the unit target (Though the distribution algorithm already used the constraints the constraints get applied again to be sure the algorithm didn't exceed the limits).

![Power Distribution Overview](./Docs/Power%20Distribution%20Overview.drawio.svg)


### Proportional capacity distribution
Distribute power proportionally based on each unit's nameplate rating as multiple unit's can have different nameplate ratings.

#### Algorithm
1. Calculate the total nameplate capacity of the system
  - Total Nameplate = 100(Nameplate1) + 80(Nameplate2) = 180
2. Calculate percentage of unit's capacity and multiply with the target:
  - PCS1: 100(Nameplate1)/180(TotalNameplate) * 40(Target) = 22.22 (UnitTarget)
  - PCS2:  80(Nameplate2)/180(TotalNameplate) * 40(Target) = 17.78 (UnitTarget)


#### Use cases
Solar, Wind, Thermal, BESS (Identical Soc)



### Proportional State Of Charge (SoC) balancing distribution
Distribute power proportionally based on each unit's state of charge so that unit's with low SoC get a higher share than the unit's with a higher SoC when charging.  
Distribute power proportionally based on each unit's state of charge so that unit's with high SoC get a higher share than the unit's with a lower SoC when discharging.


#### Use cases
BESS (Mismatch Soc), Mixed age system


#### Algorithm discharging
1. Calculate the unit available energy: Nameplate / 100 * SoC (if system uses duration multiply by hours)
  - PCS1: 100(Nameplate), 80%(SoC) -> 100 / 100 * 80 = 80(Weight)
  - PCS2:  80(Nameplate), 40%(SoC) ->  80 / 100 * 40 = 32(Weight)
2. Sum the total available energy.
  -  PCS1 + PCS2 = TotalAvailableEnergy
3. Distribute: (UnitAvailableEnergy/TotalAvailableEnergy) * Target
  - PCS1: 80(UnitAvailableEnergy)/112(TotalAvailableEnergy) * 40(Target) = 28.57 (UnitTarget)
  - PCS1: 32(UnitAvailableEnergy)/112(TotalAvailableEnergy) * 40(Target) = 11.43 (UnitTarget)


#### Algorithm charging
1. Calculate headroom for each unit: Nameplate * (100 - SoC)
  - PCS1: 100(Nameplate) * (100 - 80%(SoC)) = 2000
  - PCS2:  80(Nameplate) * (100 - 40%(SoC)) = 4800
2. Sum the total headroom
  - PCS1 + PCS2 = 6800
3. Distribute: (UnitHeadroom/TotalHeadroom) * Target
  - PCS1: 2000(UnitHeadroom)/6800(TotalHeadroom) * 40(Target) = 11.76
  - PCS2: 4800(UnitHeadroom)/6800(TotalHeadroom) * 40(Target) = 28.24


### State of Health (SoH) & degradation distribution
Distribute power proportionally based on each unit's state of health and reduce the depth of the cycling on lower SoH than the unit's with higher SoH.


### Thermal limits distribution
Distribute and cap power based on each unit's real time thermals and limits.



