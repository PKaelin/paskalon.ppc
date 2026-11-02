// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common.UnitTest
{
    /// <summary>
    /// Tests of the class <see cref="ClientDataArguments"/>.
    /// </summary>
    [TestClass]
    public class ClientDataArgumentsTest
    {
        [TestMethod]
        [DataRow("Demo|Demo.csv", null)]
        [DataRow("Demo|Demo.csv|", null)]
        [DataRow("Demo|Demo.csv|1.0", "1.0")]
        public void ParseReadsArgumentsTest(string joinedArgs, string? expectedSchemaVersion)
        {
            string[] args = joinedArgs.Split('|');

            ClientDataArguments arguments = ClientDataArguments.Parse(args);

            Assert.AreEqual("Demo", arguments.ClientProjectName);
            Assert.AreEqual("Demo.csv", arguments.ClientProjectFile);
            Assert.AreEqual(expectedSchemaVersion, arguments.SchemaVersion);
        }


        [TestMethod]
        [DataRow("Demo")]
        [DataRow("|Demo.csv")]
        [DataRow("Demo| ")]
        [DataRow("Demo|Demo.csv|1.0|Extra")]
        public void ParseRejectsInvalidArgumentsTest(string joinedArgs)
        {
            string[] args = joinedArgs.Split('|');

            Action action = () => ClientDataArguments.Parse(args);

            Assert.ThrowsExactly<ClientDataArgumentException>(action);
        }
    }
}
