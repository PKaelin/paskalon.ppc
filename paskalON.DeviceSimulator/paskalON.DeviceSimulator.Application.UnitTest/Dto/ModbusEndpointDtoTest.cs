// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.DeviceSimulator.Dto.Endpoints;
using System.Text.Json;

namespace paskalON.DeviceSimulator.Application.UnitTest.Dto
{
    [TestClass]
    public sealed class ModbusEndpointDtoTest
    {
        [TestMethod]
        [DataRow(ModbusDataType.MbInt16, "MbInt16")]
        [DataRow(ModbusDataType.MbUint16, "MbUint16")]
        [DataRow(ModbusDataType.MbFloatLe, "MbFloatLe")]
        public void ModbusEndpointDtoSerializesDataTypeAsNameTest(ModbusDataType dataType, string expected)
        {
            ModbusEndpointDto endpoint = new ModbusEndpointDto
            {
                Name = "TotalStateOfCharge",
                Address = 43000,
                DataType = dataType,
                Value = (ushort)8750
            };

            string json = JsonSerializer.Serialize(endpoint, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            using JsonDocument document = JsonDocument.Parse(json);
            Assert.AreEqual(expected, document.RootElement.GetProperty("dataType").GetString());
        }
    }
}
