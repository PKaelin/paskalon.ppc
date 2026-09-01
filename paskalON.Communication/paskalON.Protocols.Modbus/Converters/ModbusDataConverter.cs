// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace paskalON.Protocols.Modbus.Converters
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
            ArgumentNullException.ThrowIfNull(rawData);
            // Calculate offset index in rawData array
            int index = register.Register - startAddress;
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, rawData.Length);
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);

            return rawData[index];
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object? ConvertRawData(ushort[] rawData, IModbusRegisterEntry register, ushort startAddress)
        {
            ArgumentNullException.ThrowIfNull(rawData);
            ArgumentOutOfRangeException.ThrowIfLessThan(startAddress, 0);

            if (rawData.Length == 0)
            {
                return null;
            }

            // Extract only the specific registers needed for this entry
            int sliceIndex = register.Register - startAddress;
            ArgumentOutOfRangeException.ThrowIfLessThan(sliceIndex, 0);

            int registerCount = GetRegisterLength(register.DataType);
            // Create a byte buffer spanned across the target ushort array
            ushort[] registerSlice = rawData.AsSpan(sliceIndex, registerCount).ToArray();

            // Convert registers to raw bytes           
            byte[] byteBuffer = ConvertToByteArray(registerSlice, register.DataType);

            // Convert bytes to the target platform type
            object parsedValue = ConvertBytesToType(byteBuffer, register.DataType);

            return ApplyScale(parsedValue, register.Scale);
        }


        /// <summary>
        /// Gets the register length of the Modbus data type.
        /// </summary>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>The length of the data type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws an exception when the data type is not considered.</exception>
        public int GetRegisterLength(ModbusDataType type)
        {
            switch (type)
            {
                case ModbusDataType.MbBool:
                case ModbusDataType.MbInt16:
                case ModbusDataType.MbUint16:
                case ModbusDataType.MbPackedBool16:
                    return 1;
                case ModbusDataType.MbFloatBe:
                case ModbusDataType.MbFloatLe:
                case ModbusDataType.MbInt32Be:
                case ModbusDataType.MbInt32Le:
                case ModbusDataType.MbInt32M10KBe:
                case ModbusDataType.MbInt32M10KLe:
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
        public bool IsBigEndian(ModbusDataType type)
        {
            // Consider 32/64 bit data types
            if (type == ModbusDataType.MbUint64Be || type == ModbusDataType.MbPackedBool32Be || type == ModbusDataType.MbDoubleBe ||
                type == ModbusDataType.MbFloatBe || type == ModbusDataType.MbInt32Be || type == ModbusDataType.MbInt32M10KBe ||
                type == ModbusDataType.MbUint32Be || type == ModbusDataType.MbUint32M10KBe)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        ///  Get a register array from a value.
        /// </summary>
        /// <param name="value">The value to turn into a register array.</param>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>Returns an array of ushort representing the passed in value.</returns>
        public ushort[] RegisterArrayFromValue(double value, ModbusDataType type, double scale)
        {
            int registerLenght = GetRegisterLength(type);
            ushort[] registers = new ushort[registerLenght];

            // Create a byte buffer spanned across the target ushort array
            Span<byte> byteBuffer = MemoryMarshal.AsBytes(registers.AsSpan());
            double scaledValue = (double)ApplyScale(value, scale);

            switch (type)
            {
                // Booleans and packed bits
                case ModbusDataType.MbBool:
                    registers[0] = (ushort)(value != 0 ? 0x0001 : 0x0000);
                    break;
                case ModbusDataType.MbPackedBool16:
                    // Packs up to a 16-bit bitmask; clamps safely
                    registers[0] = (ushort)Math.Clamp(scaledValue, Int16.MinValue, Int16.MaxValue);
                    break;
                case ModbusDataType.MbPackedBool32Be:
                    BinaryPrimitives.WriteUInt32BigEndian(byteBuffer, (uint)Math.Clamp(scaledValue, uint.MinValue, uint.MaxValue));
                    break;
                case ModbusDataType.MbPackedBool32Le:
                    BinaryPrimitives.WriteUInt32LittleEndian(byteBuffer, (uint)Math.Clamp(scaledValue, uint.MinValue, uint.MaxValue));
                    break;
                // Floating point type
                case ModbusDataType.MbFloatBe:
                    BinaryPrimitives.WriteSingleBigEndian(byteBuffer, (float)scaledValue);
                    break;
                case ModbusDataType.MbFloatLe:
                    BinaryPrimitives.WriteSingleLittleEndian(byteBuffer, (float)scaledValue);
                    break;
                case ModbusDataType.MbDoubleBe:
                    BinaryPrimitives.WriteDoubleBigEndian(byteBuffer, scaledValue);
                    break;
                case ModbusDataType.MbDoubleLe:
                    BinaryPrimitives.WriteDoubleLittleEndian(byteBuffer, scaledValue);
                    break;
                // Signed integers
                case ModbusDataType.MbInt16:
                    registers[0] = (ushort)Math.Clamp(scaledValue, Int16.MinValue, Int16.MaxValue);
                    break;
                case ModbusDataType.MbInt32Be:
                    BinaryPrimitives.WriteInt32BigEndian(byteBuffer, (int)Math.Clamp(scaledValue, int.MinValue, int.MaxValue));
                    break;
                case ModbusDataType.MbInt32Le:
                    int i32Le = (int)Math.Clamp(scaledValue, int.MinValue, int.MaxValue);
                    BinaryPrimitives.WriteInt32LittleEndian(byteBuffer, i32Le);
                    break;
                case ModbusDataType.MbInt64Be:
                    BinaryPrimitives.WriteInt64BigEndian(byteBuffer, (Int64)Math.Clamp(scaledValue, Int64.MinValue, Int64.MaxValue));
                    break;
                case ModbusDataType.MbInt64Le:
                    BinaryPrimitives.WriteInt64LittleEndian(byteBuffer, (Int64)Math.Clamp(scaledValue, Int64.MinValue, Int64.MaxValue));
                    break;
                // Unsigned integers
                case ModbusDataType.MbUint16:
                    registers[0] = (ushort)Math.Clamp(scaledValue, UInt16.MinValue, UInt16.MaxValue);
                    break;
                case ModbusDataType.MbUint32Be:
                    BinaryPrimitives.WriteUInt32BigEndian(byteBuffer, (uint)Math.Clamp(scaledValue, uint.MinValue, uint.MaxValue));
                    break;
                case ModbusDataType.MbUint32Le:
                    BinaryPrimitives.WriteUInt32LittleEndian(byteBuffer, (uint)Math.Clamp(scaledValue, uint.MinValue, uint.MaxValue));
                    break;
                case ModbusDataType.MbUint64Be:
                    BinaryPrimitives.WriteUInt64BigEndian(byteBuffer, (UInt64)Math.Clamp(scaledValue, UInt64.MinValue, UInt64.MaxValue));
                    break;
                case ModbusDataType.MbUint64Le:
                    BinaryPrimitives.WriteUInt64LittleEndian(byteBuffer, (UInt64)Math.Clamp(scaledValue, UInt64.MinValue, UInt64.MaxValue));
                    break;
                // Modulo types
                case ModbusDataType.MbInt32M10KBe:
                    int m10kInt = (int)Math.Clamp(scaledValue, -99999999, 99999999);
                    registers[0] = (ushort)(m10kInt / 10000);   // High value (reg N)
                    registers[1] = (ushort)(m10kInt % 10000);   // Low value  (reg N+1)
                    break;
                case ModbusDataType.MbInt32M10KLe:
                    int m10kIntLe = (int)Math.Clamp(scaledValue, -99999999, 99999999);
                    registers[0] = (ushort)(m10kIntLe % 10000); // Low value  (reg N)
                    registers[1] = (ushort)(m10kIntLe / 10000); // High value (reg N+1)
                    break;
                case ModbusDataType.MbUint32M10KBe:
                    uint m10kUint = (uint)Math.Clamp(scaledValue, 0, 99999999);
                    registers[0] = (ushort)(m10kUint / 10000);  // High value (reg N)
                    registers[1] = (ushort)(m10kUint % 10000);  // Low value  (reg N+1)
                    break;
                case ModbusDataType.MbUint32M10KLe:
                    uint m10kUintLe = (uint)Math.Clamp(scaledValue, 0, 99999999);
                    registers[0] = (ushort)(m10kUintLe % 10000); // Low value  (reg N)
                    registers[1] = (ushort)(m10kUintLe / 10000); // High value (reg N+1)
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported data type: {type}");
            }

            // Correct Endianness adjustments for local system execution
            // If the system is Little-Endian reverse the individual ushort byte pairs.
            if (BitConverter.IsLittleEndian == false)
            {
                for (int i = 0; i < registers.Length; i++)
                {
                    registers[i] = BinaryPrimitives.ReverseEndianness(registers[i]);
                }
            }

            return registers;
        }


        /// <summary>
        /// Converts the register values to a bite array.
        /// </summary>
        /// <param name="registers">Array of register values that make up the whole data value.</param>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>Byte array containing the whole data value.</returns>
        public byte[] ConvertToByteArray(ushort[] registers, ModbusDataType type)
        {
            // Clone the array so we dont mutate the callers data
            ushort[] localRegisters = (ushort[])registers.Clone();

            // We want everything in little endian for now.
            if (IsBigEndian(type))
            {
                Array.Reverse(localRegisters);
            }

            byte[] totalBytes = new byte[localRegisters.Length * 2];
            Span<byte> byteSpan = totalBytes.AsSpan();

            for (int i = 0; i < localRegisters.Length; i++)
            {
                if (BitConverter.IsLittleEndian)
                {
                    // Host is Little Endian: Write individual registers as Little Endian
                    BinaryPrimitives.WriteUInt16LittleEndian(byteSpan.Slice(i * 2, 2), localRegisters[i]);
                }
                else
                {
                    // Host is Big Endian: Write individual registers as Big Endian
                    BinaryPrimitives.WriteUInt16BigEndian(byteSpan.Slice(i * 2, 2), localRegisters[i]);
                }
            }

            return totalBytes.ToArray();
        }


        /// <summary>
        /// Gets the actual value form an array of bytes.
        /// </summary>
        /// <param name="bytes">Byte array containing the whole data value.</param>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>The actual value.</returns>
        public object ConvertBytesToType(byte[] bytes, ModbusDataType type)
        {
            // Note the bytes have to come into the correct order. BigEndian/LittleEndian
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
        /// <example>
        ///  Upscale: To send a temperature of 22.56 Celsius you multiply by 100. The Modbus register stores this as the integer (2256).
        ///  Downscale: You read the integer 2256 from the register. You multiply this by 0.01 and your domain value becomes 22.56
        /// </example>
        public object ApplyScale(object value, double scale)
        {
            // If scale is 1 avoid float conversions to prevent precision loss on integers
            if (Math.Abs(scale - 1.0) < 0.0000001)
            {
                return value;
            }

            return Convert.ToDouble(value) * scale;
        }
    }
}
