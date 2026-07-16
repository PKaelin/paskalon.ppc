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

### Data Frame Payload (0x0000)
Data frames are stripped of metadata to save bandwidth. To read a data frame, the client must map fields using the layout received in the Configuration Frame Payload.
```
+---------------------------------------------------------------+
|                         DATA PAYLOAD                          |
+---------------------------------------------------------------+
| STATUS (2B)   | Bit-mapped errors, unlock states, triggers.   |
+---------------+-----------------------------------------------+
| PHASORS (4B/8B| Pair of values (Magnitude/Angle).             |
|  per phasor)  | Can be Integer or Floating-Point.             |
+---------------+-----------------------------------------------+
| FREQ (2B/4B)  | Actual frequency or frequency deviation.      |
+---------------+-----------------------------------------------+
| DFREQ (2B/4B) | ROCOF (Rate of Change of Frequency).          |
+---------------+-----------------------------------------------+
| ANALOGS (2/4B)| Miscellaneous analog inputs (e.g., MW, MVAR). |
+---------------+-----------------------------------------------+
| DIGITALS (2B) | Digital status words (e.g., breaker states).  |
+---------------------------------------------------------------+
```

### Configuration Frame Payload (0x2000)
This frame acts as a parsing dictionary for the client application.
```
+---------------------------------------------------------------+
|                     CONFIG 2 PAYLOAD                          |
+---------------------------------------------------------------+
| DATA_RATE     | Transmit frequency (e.g., 30 or 60 frames/sec)|
+---------------+-----------------------------------------------+
| NUM_PMU       | Number of PMUs included in this data stream.  |
+---------------+-----------------------------------------------+
| PMU_NAME      | 16-Byte ASCII string station identifier.      |
+---------------+-----------------------------------------------+
| FORMAT        | Data types (Floating point vs 16-bit Integer).|
+---------------+-----------------------------------------------+
| PH_NUM        | Number of phasors configured (Count).         |
+---------------+-----------------------------------------------+
| AN_NUM        | Number of analog channels configured (Count). |
+---------------+-----------------------------------------------+
| DG_NUM        | Number of digital status words (Count).       |
+---------------+-----------------------------------------------+
| CH_NAMES      | Array of ASCII strings naming every channel.  |
+---------------+-----------------------------------------------+
| CONVERSION    | Scaling factors for Voltage, Current, Analog. |
+---------------------------------------------------------------+
```

### Header Frame Payload 0x4000
```
+--------------------+--------------------------------------------------+
| HEADER PAYLOAD                                                        |
+--------------------+--------------------------------------------------+
| DATA_LEN (2B)      | 0x00 0x17   (Decimal 23)                         |
+--------------------+--------------------------------------------------+
| DATA (23B ASCII)   | 50 4D 55 2D 30 31 2C 20 66 69 72 6D 77 61 72 65  |
+--------------------+--------------------------------------------------+
```



### Command Frame Payload (0x5000)
```
+---------------------------------------------------------------+
|                       COMMAND PAYLOAD                         |
+---------------------------------------------------------------+
| CMD (2 Bytes) |  0x0001 = Turn OFF real-time data stream      |
|               |  0x0002 = Turn ON real-time data stream       |
|               |  0x0003 = Send Header Frame                   |
|               |  0x0004 = Send Config 1 Frame                 |
|               |  0x0005 = Send Config 2 Frame                 |
|               |  0x0006 = Send Config 3 Frame                 |
+---------------+-----------------------------------------------+
| EXT_REG (Var) | Extended data parameters (optional).          |
+---------------------------------------------------------------+
```

