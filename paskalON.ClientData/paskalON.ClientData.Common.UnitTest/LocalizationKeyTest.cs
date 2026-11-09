// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common.UnitTest
{
    /// <summary>
    /// Tests of the class <see cref="LocalizationKey"/>.
    /// </summary>
    [TestClass]
    public class LocalizationKeyTest
    {
        [TestMethod]
        [DataRow("{{PCS.CommunicationError.Name}}", "PCS.CommunicationError.Name")]
        [DataRow("{{PCS.FaultStates.HighInputVoltage.Desc}}", "PCS.FaultStates.HighInputVoltage.Desc")]
        [DataRow("{{AlarmId}}", "AlarmId")]
        public void TryParseKeyReturnsKeyWithoutBracesTest(string value, string expectedKey)
        {
            bool isKey = LocalizationKey.TryParse(value, out string key);

            Assert.IsTrue(isKey);
            Assert.AreEqual(expectedKey, key);
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("PCS communication error")]
        [DataRow("{{}}")]
        [DataRow("{PCS.Name}")]
        [DataRow("Text {{PCS.Name}}")]
        [DataRow("{{PCS.Name}} text")]
        [DataRow("{{PCS.{Name}}}")]
        public void TryParsePlainTextReturnsFalseTest(string? value)
        {
            bool isKey = LocalizationKey.TryParse(value, out string key);

            Assert.IsFalse(isKey);
            Assert.AreEqual(string.Empty, key);
        }
    }
}
