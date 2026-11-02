// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text;

namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Splits a line of a client project file into its Key=Value cells.
    /// </summary>
    public static class CsvLineParser
    {
        /// <summary>
        /// Parses a line. Both Key="a, b" and "Key=a, b" are accepted and two quotes within quotes result in one quote.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The cells of the line.</returns>
        /// <exception cref="FormatException">Thrown when a quote is not closed or a cell is not in the Key=Value format.</exception>
        public static IReadOnlyList<CsvCell> Parse(string line)
        {
            ArgumentNullException.ThrowIfNull(line);

            List<CsvCell> cells = new List<CsvCell>();
            StringBuilder text = new StringBuilder();
            bool isInQuotes = false;
            bool isQuoted = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (isInQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        text.Append('"');
                        i++;
                    }
                    else
                    {
                        isInQuotes = isInQuotes is false;
                        isQuoted = true;
                    }
                }
                else if (c == ',' && isInQuotes is false)
                {
                    cells.Add(CreateCell(text.ToString(), isQuoted));
                    text.Clear();
                    isQuoted = false;
                }
                else
                {
                    text.Append(c);
                }
            }

            if (isInQuotes)
            {
                throw new FormatException("A quoted value is not closed.");
            }

            cells.Add(CreateCell(text.ToString(), isQuoted));

            return cells;
        }


        /// <summary>
        /// Creates the cell by splitting the text at the first equal sign.
        /// </summary>
        /// <param name="text">The cell text without the quotes.</param>
        /// <param name="isQuoted">Whether the cell contained quotes.</param>
        /// <returns>The cell.</returns>
        private static CsvCell CreateCell(string text, bool isQuoted)
        {
            int separator = text.IndexOf('=');

            if (separator < 0)
            {
                throw new FormatException($"Cell '{text.Trim()}' is not in the Key=Value format.");
            }

            string key = text[..separator].Trim();
            string value = text[(separator + 1)..];

            if (key.Length == 0)
            {
                throw new FormatException($"Cell '{text.Trim()}' has no key.");
            }

            return new CsvCell(key, isQuoted ? value : value.Trim(), isQuoted);
        }
    }
}
