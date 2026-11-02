// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;

namespace paskalON.ClientData.Common.UnitTest
{
    /// <summary>
    /// Tests of the class <see cref="ClientDataRunner"/>.
    /// </summary>
    [TestClass]
    public class ClientDataRunnerTest
    {
        [TestMethod]
        public async Task RunAsyncReturnsInvalidArgumentsTest()
        {
            string[] args = ["OnlyOneArgument"];

            int exitCode = await ClientDataRunner.RunAsync<DbContext>(args, () => throw new InvalidOperationException());

            Assert.AreEqual((int)ClientDataExitCode.InvalidArguments, exitCode);
        }
    }
}
