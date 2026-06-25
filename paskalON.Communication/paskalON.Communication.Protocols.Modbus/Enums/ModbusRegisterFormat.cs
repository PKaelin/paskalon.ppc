

namespace paskalON.Communication.Protocols.Modbus.Enums
{
    public enum ModbusRegisterFormat
    {
        MbBool,                     // A single boolean bit flag. TRUE == 0x01; FALSE == 0x00.
        MbFloatBe,                  // 32 bit IEEE float; reg N contains the most significant 16 bits.
        MbFloatLe,                  // 32 bit IEEE float; reg N contains the least significant 16 bits.
        MbDoubleBe,                 // 64 bit IEEE double precision float; reg N contains the most significant 16 bits.
        MbDoubleLe,                 // 64 bit IEEE double precision float; reg N contains the least significant 16 bits.
        MbInt16,                    // 16 bit signed int.
        MbInt32Be,                  // 32 bit signed int; reg N contains the most significant 16 bits.
        MbInt32Le,                  // 32 bit signed int; reg N contains the least significant 16 bits.
        MbInt64Be,                  // 64 bit signed int; reg N contains the most significant 16 bits.
        MbInt64Le,                  // 64 bit signed int; reg N contains the least significant 16 bits.
        MbInt32M10KBe,              // 32 bit signed int; reg N contains (value / 10000); reg N+1 contains (value mod 10000).
        MbInt32M10KLe,              // 32 bit signed int; reg N contains (value mod 10000); reg N+1 contains (value / 10000).
        MbPackedBool16,             // 16 packed booleans, 0..15; most significant bit is bit 0.
        MbPackedBool32Be,           // 32 packed booleans, 0..31; reg N contains 0..15, register N+1 contains 16..31.
        MbPackedBool32Le,           // 32 packed booleans, 0..31; reg N contains 16..31, register N+1 contains 0..15.
        MbUint16,                   // 16 bit unsigned int.
        MbUint32Be,                 // 32 bit unsigned int; reg N contains the most significant 16 bits.
        MbUint32Le,                 // 32 bit unsigned int; reg N contains the least significant 16 bits.
        MbUint64Be,                 // 64 bit unsigned int; reg N contains the most significant 16 bits.
        MbUint64Le,                 // 64 bit unsigned int; reg N contains the least significant 16 bits.
        MbUint32M10KBe,             // 32 bit unsigned int; reg N contains (value / 10000); reg N+1 contains (value mod 10000).
        MbUint32M10KLe              // 32 bit unsigned int; reg N contains (value mod 10000); reg N+1 contains (value / 10000).
    };
}