# C37.118 Protocol
The C37.118 standard defines a method for exchange of synchronized phasor measurement data between power system equipment.


## Frame Structure & Types
Every C37.118 packet shares a common Sync and Header structure, followed by a variable payload, and ends with a 16-bit CRC for error checking.

| Frame Type | Hex ID | Direction | Description |
|------------|--------|-----------|-------------|
| Data Frame | 0x0000 |PMU → PDC | Contains actual real-time measurement values. Sent continuously.|
| Config1 | 0x1000 | PMU → PDC | Identifies PMU capabilities, channel names, and calibration factors.|
| Config2 | 0x2000 | PMU → PDC | Defines the active data stream format. Required to parse the Data Frame.|
| Config3 | 0x3000 | PMU → PDC | Advanced XML-like configuration (Introduced in C37.118.2-2011).|
| Header Frame | 0x4000 | PMU → PDC | Human-readable ASCII information about the PMU/installation.|
| Command Frame | 0x5000 | PDC → PMU | Controls the PMU (e.g., Start/Stop data, Request Config).|



## Detailed Payload Layouts


### Id Definitions
| Identifier | Field | Purpose | Scope |
|------------|-------|---------|-------|
| Station ID (STN)	| String field | Human-readable name of the PMU or PDC | Identification |
| Stream ID (IDCODE) | 16-bit integer | Numeric identifier used to associate frames	| Protocol / Communication |


### Data Frame Payload (0x0000)
Data frames are stripped of metadata to save bandwidth. To read a data frame, the client must map fields using the layout received in the Configuration Frame Payload.

| Order | Field | Count | Size | Type | Description |
|------:|-------|------:|------|------|-------------|
| 1 | STAT | 1 | 2 bytes | UInt16 | Status and quality flags for the PMU measurements. |
| 2 | PHASOR | PHNMR | 4 or 8 bytes each | Int16×2 or Float32×2 | Phasor measurements. Format (Rectangular/Polar) and data type are defined by the `FORMAT` field in CFG-2. |
| 3 | FREQ | 1 | 2 or 4 bytes | Int16 or Float32 | Frequency deviation from the nominal system frequency. |
| 4 | DFREQ (ROCOF) | 1 | 2 or 4 bytes | Int16 or Float32 | Rate of Change of Frequency (ROCOF). |
| 5 | ANALOG | ANNMR | 2 or 4 bytes each | Int16 or Float32 | Analog measurement values. |
| 6 | DIGITAL | DGNMR | 2 bytes each | UInt16 | Digital status word containing 16 digital input states. |
- `PHNMR` = Number of phasors configured for the PMU.
- `ANNMR` = Number of analog channels configured for the PMU.
- `DGNMR` = Number of digital status words configured for the PMU.



### Configuration Frame Payload (0x2000)
This frame acts as a parsing dictionary for the client application.


#### Overall Payload Layout

| Order | Field | Count | Size | Type | Description |
|------:|-------|------:|------|------|-------------|
| 1 | TIME_BASE | 1 | 4 bytes | UInt32 | Time base used to interpret the FRACSEC field. |
| 2 | NUM_PMU | 1 | 2 bytes | UInt16 | Number of PMUs described in this configuration. |
| 3 | PMU Configuration Block | NUM_PMU | Variable | Structure | Configuration block for each PMU. |
| 4 | DATA_RATE | 1 | 2 bytes | Int16 | Data reporting rate (frames per second or seconds per frame). |


#### PMU Configuration Block

The following block is repeated once for each PMU.

| Order | Field | Count | Size | Type | Description |
|------:|-------|------:|------|------|-------------|
| 1 | STN | 1 | 16 bytes | ASCII | Station name (null or space padded) (StationId). |
| 2 | IDCODE | 1 | 2 bytes | UInt16 | Unique identifier of the PMU. |
| 3 | FORMAT | 1 | 2 bytes | UInt16 | Defines measurement encoding formats. |
| 4 | PHNMR | 1 | 2 bytes | UInt16 | Number of phasor channels. |
| 5 | ANNMR | 1 | 2 bytes | UInt16 | Number of analog channels. |
| 6 | DGNMR | 1 | 2 bytes | UInt16 | Number of digital status words. |
| 7 | CHNAM | PHNMR + ANNMR + (DGNMR × 16) | 16 bytes each | ASCII | Channel names. |
| 8 | PHUNIT | PHNMR | 4 bytes each | UInt32 | Phasor conversion factors and type. |
| 9 | ANUNIT | ANNMR | 4 bytes each | UInt32 | Analog conversion factors and type. |
| 10 | DGUNIT | DGNMR | 4 bytes each | UInt32 | Digital masks (normal state and valid bits). |
| 11 | FNOM | 1 | 2 bytes | UInt16 | Nominal system frequency (50 Hz or 60 Hz). |
| 12 | CFGCNT | 1 | 2 bytes | UInt16 | Configuration change counter. |



### Header Frame Payload 0x4000

| Order | Field | Size | Type | Description |
|------:|-------|-----:|------|-------------|
| 1 | SYNC | 2 bytes | UInt16 | Synchronization word containing frame type and protocol version. |
| 2 | FRAMESIZE | 2 bytes | UInt16 | Total frame length in bytes, including header, payload, and CRC. |
| 3 | IDCODE | 2 bytes | UInt16 | Stream/PMU identifier. |
| 4 | SOC | 4 bytes | UInt32 | Seconds of Century timestamp. |
| 5 | FRACSEC | 4 bytes | UInt32 | Fraction of second and time quality flags. |
| 6 | HEADER | Variable | ASCII | Vendor-defined human-readable text. |
| 7 | CHK | 2 bytes | UInt16 | CRC-CCITT checksum. |



### Command Frame Payload (0x5000)
The Command Frame is used to control a PMU or PDC. 
Typical commands include starting or stopping data transmission and requesting Configuration or Header Frames.

| Order | Field | Size | Type | Description |
|------:|-------|-----:|------|-------------|
| 1 | SYNC | 2 bytes | UInt16 | Synchronization word containing protocol version and frame type. |
| 2 | FRAMESIZE | 2 bytes | UInt16 | Total frame size including header, payload, and CRC. |
| 3 | IDCODE | 2 bytes | UInt16 | Destination PMU or stream identifier. |
| 4 | SOC | 4 bytes | UInt32 | Seconds of Century timestamp. |
| 5 | FRACSEC | 4 bytes | UInt32 | Fraction of second and time quality flags. |
| 6 | Command Payload | Variable | Structure | Command code and optional extension data. |
| 7 | CHK | 2 bytes | UInt16 | CRC-CCITT checksum. |


#### Command Codes

| Value | Command | Description |
|------:|---------|-------------|
| 0x0001 | Stop Transmission | Stop sending Data Frames. |
| 0x0002 | Start Transmission | Begin sending Data Frames. |
| 0x0003 | Send Header | Request a Header Frame. |
| 0x0004 | Send CFG-1 | Request Configuration Frame 1. |
| 0x0005 | Send CFG-2 | Request Configuration Frame 2. |
| 0x0006 | Send CFG-3 | Request Configuration Frame 3. |
| 0x0007 | Extended Command | Vendor-specific command. |
