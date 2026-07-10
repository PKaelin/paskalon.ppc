// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Communication.Protocols.Modbus;
using paskalON.Dataface.Modbus;

namespace paskalON.Devices.Equipments.Modbus
{
    /// <summary>
    /// Implementation of the Modbus polling engine <see cref="IModbusPollingEngine"/>.
    /// </summary>
    public class ModbusPollingEngine : IModbusPollingEngine
    {
        /// <summary>
        /// The Modbus client.
        /// </summary>
        private readonly IModbusClient _client;

        /// <summary>
        /// The Modbus dataface for loos coupling.
        /// </summary>
        private readonly IModbusDataface _dataface;


        /// <summary>
        /// Constructor of <see cref="ModbusPollingEngine"/>.
        /// </summary>
        /// <param name="client">The Modbus client interface.</param>
        /// <param name="dataface">The Modbus data interface.</param>
        public ModbusPollingEngine(IModbusClient client, IModbusDataface dataface)
        {
            _client = client;
            _dataface = dataface;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task PollAsync(int currentInterval, CancellationToken cancellationToken)
        {
            foreach (ModbusPollingRangeEntry range in _dataface.PollingRanges)
            {
                if (currentInterval % range.Interval == 0)
                {
                    ushort startAddress = range.From;
                    ushort endAddress = range.To;
                    ushort[]? rawShortData = null;
                    bool[]? rawBoolData = null;

                    switch (range.RegistryType)
                    {
                        // Fetch raw data via the decoupled client.
                        case ModbusRegistryType.Coil:
                            rawBoolData = await _client.ReadCoilsAsync(startAddress, endAddress, cancellationToken);
                            break;
                        case ModbusRegistryType.DiscreteInput:
                            rawBoolData = await _client.ReadDiscreteInputsAsync(startAddress, endAddress, cancellationToken);
                            break;
                        case ModbusRegistryType.InputRegister:
                            rawShortData = await _client.ReadInputRegistersAsync(startAddress, endAddress, cancellationToken);
                            break;
                        case ModbusRegistryType.HoldingRegister:
                            rawShortData = await _client.ReadHoldingRegistersAsync(startAddress, endAddress, cancellationToken);
                            break;

                    }

                    // Filter registers that fall within this range
                    IEnumerable<IModbusRegisterEntry> registers = _dataface.Registers.Where(r => r.Register >= startAddress && r.Register < endAddress).OrderBy(n => n.Register);

                    // Map, scale, handle word order and call update
                    foreach (var register in registers)
                    {
                        if (rawShortData != null)
                        {
                            object parsedValue = ProcessRawData(rawShortData, register, startAddress);
                            register.Update(parsedValue);
                        }
                        else if (rawBoolData != null)
                        {
                            object parsedValue = ProcessRawData(rawBoolData, register, startAddress);
                            register.Update(parsedValue);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Gets the bool value. 
        /// </summary>
        /// <param name="rawData">List of bool values</param>
        /// <param name="register">The Modbus register entry.</param>
        /// <param name="startAddress">The start address of the first raw data value.</param>
        /// <returns></returns>
        private bool ProcessRawData(bool[] rawData, IModbusRegisterEntry register, ushort startAddress)
        {
            // Calculate offset index in rawData array
            int index = register.Register - startAddress;

            return rawData[index];
        }


        /// <summary>
        /// Gets the value of type DataType.
        /// </summary>
        /// <param name="rawData">List of ushort value.</param>
        /// <param name="register">The Modbus register entry.</param>
        /// <param name="startAddress">The start address of the first raw data value.</param>
        /// <returns></returns>
        private object ProcessRawData(ushort[] rawData, IModbusRegisterEntry register, ushort startAddress)
        {
            // Extract only the specific registers needed for this entry
            int sliceIndex = register.Register - startAddress;
            int registerCount = GetRegisterLength(register.DataType);
            ushort[] registerSlice = rawData.AsSpan(sliceIndex, registerCount).ToArray();

            // Convert registers to raw bytes            
            byte[] byteBuffer = ConvertToByteArray(registerSlice, register.DataType);

            // Convert bytes to the target platform type
            object parsedValue = ConvertBytesToType(byteBuffer, register.DataType);

            // Apply scale dynamically based on type
            return ApplyScale(parsedValue, register.Scale);
        }


        /// <summary>
        /// Gets the register length of the Modbus data type.
        /// </summary>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>The length of the data type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws an exception when the data type is not considered.</exception>
        private int GetRegisterLength(ModbusDataType type)
        {
            switch (type)
            {
                case ModbusDataType.MbInt16:
                case ModbusDataType.MbUint16:
                    return 1;
                case ModbusDataType.MbFloatBe:
                case ModbusDataType.MbFloatLe:
                case ModbusDataType.MbInt32Be:
                case ModbusDataType.MbInt32Le:
                case ModbusDataType.MbInt32M10KBe:
                case ModbusDataType.MbInt32M10KLe:
                case ModbusDataType.MbPackedBool16:
                case ModbusDataType.MbPackedBool32Be:
                case ModbusDataType.MbPackedBool32Le:
                case ModbusDataType.MbUint32Be:
                case ModbusDataType.MbUint32Le:
                case ModbusDataType.MbUint32M10KBe:
                case ModbusDataType.MbUint32M10KLe:
                    return 2;
                case ModbusDataType.MbDoubleBe:
                case ModbusDataType.MbDoubleLe:
                case ModbusDataType.MbInt64Be:
                case ModbusDataType.MbInt64Le:
                case ModbusDataType.MbUint64Be:
                case ModbusDataType.MbUint64Le:

                    return 4;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(ModbusDataType)} of type {type} is not expected.");
            }
        }


        /// <summary>
        /// Gets whether the Modbus data type is big or little endian.
        /// </summary>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>True if Modbus data type is big endian otherwise false.</returns>
        private bool IsBigEndian(ModbusDataType type)
        {
            if (type == ModbusDataType.MbPackedBool16 || type == ModbusDataType.MbPackedBool32Be || type == ModbusDataType.MbDoubleBe ||
                type == ModbusDataType.MbFloatBe || type == ModbusDataType.MbInt32Be || type == ModbusDataType.MbInt32M10KBe ||
                type == ModbusDataType.MbUint32Be || type == ModbusDataType.MbUint32M10KBe || type == ModbusDataType.MbUint64Be)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Converts the register values to a bite array.
        /// </summary>
        /// <param name="registers">Array of register values that make up the whole data value.</param>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>Byte array containing the whole data value.</returns>
        private byte[] ConvertToByteArray(ushort[] registers, ModbusDataType type)
        {
            List<byte> totalBytes = new List<byte>();

            // LittleEndian word order means the lowest register comes first. 
            // BigEndian word order means the highest register comes first (requires reverse).
            if (IsBigEndian(type) == true)
            {
                Array.Reverse(registers);
            }

            // Convert each ushort to bytes
            foreach (ushort register in registers)
            {
                byte[] regBytes = BitConverter.GetBytes(register);

                // Modbus standard is natively Big Endian
                // If your target CPU architecture is Little Endian, swap individual register bytes.
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(regBytes);
                }

                totalBytes.AddRange(regBytes);
            }

            return totalBytes.ToArray();
        }


        /// <summary>
        /// Gets the actual value form an array of bytes.
        /// </summary>
        /// <param name="bytes">Byte array containing the whole data value.</param>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>The actual value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws an exception when the data type is not considered.</exception>
        private object ConvertBytesToType(byte[] bytes, ModbusDataType type)
        {
            switch (type)
            {
                // Two bytes
                case ModbusDataType.MbInt16: return BitConverter.ToInt16(bytes, 0);
                case ModbusDataType.MbUint16: return BitConverter.ToUInt16(bytes, 0);
                case ModbusDataType.MbPackedBool16: return BitConverter.ToUInt16(bytes, 0);
                // Four bytes
                case ModbusDataType.MbFloatBe: return BitConverter.ToSingle(bytes, 0);
                case ModbusDataType.MbFloatLe: return BitConverter.ToSingle(bytes, 0);
                case ModbusDataType.MbInt32Be: return BitConverter.ToInt32(bytes, 0);
                case ModbusDataType.MbInt32Le: return BitConverter.ToInt32(bytes, 0);
                case ModbusDataType.MbInt32M10KBe: return BitConverter.ToInt32(bytes, 0);
                case ModbusDataType.MbInt32M10KLe: return BitConverter.ToInt32(bytes, 0);
                case ModbusDataType.MbPackedBool32Be: return BitConverter.ToInt32(bytes, 0);
                case ModbusDataType.MbPackedBool32Le: return BitConverter.ToInt32(bytes, 0);
                case ModbusDataType.MbUint32Be: return BitConverter.ToUInt32(bytes, 0);
                case ModbusDataType.MbUint32Le: return BitConverter.ToUInt32(bytes, 0);
                case ModbusDataType.MbUint32M10KBe: return BitConverter.ToUInt32(bytes, 0);
                case ModbusDataType.MbUint32M10KLe: return BitConverter.ToUInt32(bytes, 0);
                // Eight bytes
                case ModbusDataType.MbDoubleBe: return BitConverter.ToDouble(bytes, 0);
                case ModbusDataType.MbDoubleLe: return BitConverter.ToDouble(bytes, 0);
                case ModbusDataType.MbInt64Be: return BitConverter.ToInt64(bytes, 0);
                case ModbusDataType.MbInt64Le: return BitConverter.ToInt64(bytes, 0);
                case ModbusDataType.MbUint64Be: return BitConverter.ToUInt64(bytes, 0);
                case ModbusDataType.MbUint64Le: return BitConverter.ToUInt64(bytes, 0);
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }


        /// <summary>
        /// Apply scale if scale is defined.
        /// </summary>
        /// <param name="value">The value to scale.</param>
        /// <param name="scale">The scale that gets applied.</param>
        /// <returns>Scaled value.</returns>
        private object ApplyScale(object value, double scale)
        {
            // If scale is exactly 1 avoid float conversions to prevent precision loss on integers
            if (Math.Abs(scale - 1.0) < 0.0000001)
            {
                return value;
            }

            // Convert underlying numeric to double, multiply and return
            return Convert.ToDouble(value) * scale;
        }
    }
}