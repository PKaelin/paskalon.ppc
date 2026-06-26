# Device Domain Configs
This project contains all persistent configuration related code.


## General concept overview
We define a specific "Devices" just once (PCS, Battery, etc.) and have multiple "Communications" (C37 or Modbus Addressables), communicate with actual device. "Logicals" are groups of things like DerCircuit, DerUnit, etc.  
![DeviceService Domain Configs Concept](./DeviceService%20Domain%20Configs%20Concept.drawio.svg)


## Device domain config design
![DeviceService Domain Configs](./DeviceService%20Domain%20Configs.drawio.svg)


## Device domain meter config design
![DeviceService Domain Meter Configs](./DeviceService%20Domain%20Meter%20Configs.drawio.svg)


## Device domain GMD config design
![DeviceService Domain GMD Configs](./DeviceService%20Domain%20GMD%20Configs.drawio.svg)
