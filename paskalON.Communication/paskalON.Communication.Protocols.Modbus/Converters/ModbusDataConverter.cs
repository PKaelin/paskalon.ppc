// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;

namespace paskalON.Communication.Protocols.Modbus.Converters
{
    /// <summary>
    /// Implementation of <see cref="IModbusDataConverter"/>.
    /// </summary>
    public class ModbusDataConverter : IModbusDataConverter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool ConvertRawData(bool[] rawData, IModbusRegisterEntry register, ushort startAddress)
        {
            // Calculate offset index in rawData array
            int index = register.Register - startAddress;

            return rawData[index];
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object ConvertRawData(ushort[] rawData, IModbusRegisterEntry register, ushort startAddress)
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
        /// <inheritdoc/>
        /// </summary>
        public int GetRegisterLength(ModbusDataType type)
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
        /// <inheritdoc/>
        /// </summary>
        public bool IsBigEndian(ModbusDataType type)
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
        /// <inheritdoc/>
        /// </summary>
        public byte[] ConvertToByteArray(ushort[] registers, ModbusDataType type)
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
        /// <inheritdoc/>
        /// </summary>
        public object ConvertBytesToType(byte[] bytes, ModbusDataType type)
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
        /// <inheritdoc/>
        /// </summary>
        public object ApplyScale(object value, double scale)
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
