# Device Domain Configs
This project contains all persistent configuration related code.


## General concept overview
We define a specific "Devices" just once (PCS, Battery, etc.) and have multiple "Communications" (C37 or Modbus Addressable), communicate with actual device. "Logicals" are groups of things like DerCircuit, DerUnit, etc.  
![DeviceService Domain Configs Concept](./Docs/DeviceService%20Domain%20Configs%20Concept.drawio.svg)


## Device domain config design
![DeviceService Domain Configs](./Docs/DeviceService%20Domain%20Configs.drawio.svg)


## Device domain meter config design
![DeviceService Domain Meter Configs](./Docs/DeviceService%20Domain%20Meter%20Configs.drawio.svg)


## Device domain GMD config design
![DeviceService Domain GMD Configs](./Docs/DeviceService%20Domain%20GMD%20Configs.drawio.svg)
