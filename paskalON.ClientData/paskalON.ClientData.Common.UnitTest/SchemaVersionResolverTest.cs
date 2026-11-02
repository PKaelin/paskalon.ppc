// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common.UnitTest
{
    /// <summary>
    /// Tests of the class <see cref="SchemaVersionResolver"/>.
    /// </summary>
    [TestClass]
    public class SchemaVersionResolverTest
    {
        private static readonly string[] _migrations = ["20260101000000_v_0_1", "20260201000000_v_1_0"];


        [TestMethod]
        [DataRow(null, null)]
        [DataRow("", null)]
        [DataRow("0.1", "20260101000000_v_0_1")]
        [DataRow("1.0", "20260201000000_v_1_0")]
        [DataRow("20260201000000_v_1_0", "20260201000000_v_1_0")]
        public void ResolveFindsMigrationTest(string? schemaVersion, string? expected)
        {
            string? result = SchemaVersionResolver.Resolve(schemaVersion, _migrations);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void ResolveRejectsUnknownVersionTest()
        {
            Action action = () => SchemaVersionResolver.Resolve("9.9", _migrations);

            Assert.ThrowsExactly<ClientDataArgumentException>(action);
        }
    }
}
