// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    /// <summary>
    /// Enum for Modbus types incorporating Big and Little Endian types.
    /// </summary>
    public enum ModbusDataType
    {
        /// <summary>
        /// A single boolean bit flag. TRUE == 0x01; FALSE == 0x00.
        /// </summary>
        MbBool,
        /// <summary>
        /// 32 bit IEEE float; reg N contains the most significant 16 bits (Big Endian). 
        /// </summary>
        MbFloatBe,
        /// <summary>
        /// 32 bit IEEE float; reg N contains the least significant 16 bits (Little Endian).
        /// </summary>
        MbFloatLe,
        /// <summary>
        /// 64 bit IEEE double precision float; reg N contains the most significant 16 bits (Big Endian).
        /// </summary>
        MbDoubleBe,
        /// <summary>
        /// 64 bit IEEE double precision float; reg N contains the least significant 16 bits (Little Endian).
        /// </summary>
        MbDoubleLe,
        /// <summary>
        /// 16 bit signed int.
        /// </summary>
        MbInt16,
        /// <summary>
        /// 32 bit signed int; reg N contains the most significant 16 bits (Big Endian).
        /// </summary>
        MbInt32Be,
        /// <summary>
        /// 32 bit signed int; reg N contains the least significant 16 bits (Little Endian).
        /// </summary>
        MbInt32Le,
        /// <summary>
        /// 64 bit signed int; reg N contains the most significant 16 bits (Big Endian).
        /// </summary>
        MbInt64Be,
        /// <summary>
        /// 64 bit signed int; reg N contains the least significant 16 bits (Little Endian).
        /// </summary>
        MbInt64Le,
        /// <summary>
        /// 32 bit signed int; reg N contains (value / 10000); reg N+1 contains (value mod 10000) (Big Endian).
        /// </summary>
        MbInt32M10KBe,
        /// <summary>
        /// 32 bit signed int; reg N contains (value mod 10000); reg N+1 contains (value / 10000) (Little Endian).
        /// </summary>
        MbInt32M10KLe,
        /// <summary>
        /// 16 packed booleans, 0..15; most significant bit is bit 0.
        /// </summary>
        MbPackedBool16,
        /// <summary>
        /// 32 packed booleans, 0..31; reg N contains 0..15, register N+1 contains 16..31 (Big Endian).
        /// </summary>
        MbPackedBool32Be,
        /// <summary>
        /// 32 packed booleans, 0..31; reg N contains 16..31, register N+1 contains 0..15 (Little Endian).
        /// </summary>
        MbPackedBool32Le,
        /// <summary>
        /// 16 bit unsigned int.
        /// </summary>
        MbUint16,
        /// <summary>
        /// 32 bit unsigned int; reg N contains the most significant 16 bits (Big Endian).
        /// </summary>
        MbUint32Be,
        /// <summary>
        /// 32 bit unsigned int; reg N contains the least significant 16 bits (Little Endian).
        /// </summary>
        MbUint32Le,
        /// <summary>
        /// 64 bit unsigned int; reg N contains the most significant 16 bits (Big Endian).
        /// </summary>
        MbUint64Be,
        /// <summary>
        /// 64 bit unsigned int; reg N contains the least significant 16 bits (Little Endian).
        /// </summary>
        MbUint64Le,
        /// <summary>
        /// 32 bit unsigned int; reg N contains (value / 10000); reg N+1 contains (value mod 10000) (Big Endian).
        /// </summary>
        MbUint32M10KBe,
        /// <summary>
        /// 32 bit unsigned int; reg N contains (value mod 10000); reg N+1 contains (value / 10000) (Little Endian).
        /// </summary>
        MbUint32M10KLe
    }
}

