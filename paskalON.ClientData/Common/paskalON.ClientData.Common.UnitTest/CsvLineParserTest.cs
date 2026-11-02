// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common.UnitTest
{
    /// <summary>
    /// Tests of the class <see cref="CsvLineParser"/>.
    /// </summary>
    [TestClass]
    public class CsvLineParserTest
    {
        [TestMethod]
        public void ParseSplitsCellsAndTrimsUnquotedTextTest()
        {
            string line = "Type=A, Name=Der 1 ,Port=4400";

            IReadOnlyList<CsvCell> cells = CsvLineParser.Parse(line);

            Assert.HasCount(3, cells);
            Assert.AreEqual(new CsvCell("Name", "Der 1", false), cells[1]);
        }


        [TestMethod]
        [DataRow("Key=\"a, b\"", "a, b")]
        [DataRow("\"Key=a, b\"", "a, b")]
        [DataRow("Key=\" a \"", " a ")]
        [DataRow("Key=\"say \"\"hi\"\"\"", "say \"hi\"")]
        [DataRow("Key=\"a=b\"", "a=b")]
        [DataRow("Key=\"\"", "")]
        public void ParseHandlesQuotesTest(string line, string expectedValue)
        {
            IReadOnlyList<CsvCell> cells = CsvLineParser.Parse(line);

            Assert.AreEqual(new CsvCell("Key", expectedValue, true), cells[0]);
        }


        [TestMethod]
        [DataRow("Key=\"a")]
        [DataRow("NoSeparator")]
        [DataRow("=Value")]
        [DataRow("Key=1,")]
        public void ParseRejectsMalformedLineTest(string line)
        {
            Action action = () => CsvLineParser.Parse(line);

            Assert.ThrowsExactly<FormatException>(action);
        }
    }
}
