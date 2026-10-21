// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;
using paskalON.DeviceSimulator.Dto.Endpoints;
using System.Text.Json;

namespace paskalON.DeviceSimulator.Application.UnitTest.Dto
{
    [TestClass]
    public sealed class C37EndpointDtoTest
    {
        [TestMethod]
        [DataRow(C37SignalType.Analog, "Analog")]
        [DataRow(C37SignalType.Phasor, "Phasor")]
        public void C37EndpointDtoSerializesSignalTypeAsNameTest(C37SignalType signalType, string expected)
        {
            C37EndpointDto endpoint = new C37EndpointDto
            {
                Name = "ActivePower",
                Endpoint = "Analog1",
                SignalType = signalType,
                Value = 1800000
            };

            string json = JsonSerializer.Serialize(endpoint, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            using JsonDocument document = JsonDocument.Parse(json);
            Assert.AreEqual(expected, document.RootElement.GetProperty("signalType").GetString());
        }
    }
}
