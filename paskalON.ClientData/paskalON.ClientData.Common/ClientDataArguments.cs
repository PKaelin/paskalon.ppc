// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common
{
    /// <summary>
    /// The input arguments of a Data project.
    /// </summary>
    /// <param name="ClientProjectName">The name of the client project.</param>
    /// <param name="ClientProjectFile">The path to the client project file.</param>
    /// <param name="SchemaVersion">The schema version to use or null for the latest version.</param>
    public record ClientDataArguments(string ClientProjectName, string ClientProjectFile, string? SchemaVersion)
    {
        /// <summary>
        /// Usage description of the arguments.
        /// </summary>
        public const string Usage = "Usage: <ClientProjectName> <ClientProjectFile> [SchemaVersion]";


        /// <summary>
        /// Parses the positional program arguments.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        /// <returns>The parsed arguments.</returns>
        /// <exception cref="ClientDataArgumentException">Thrown when the arguments are invalid.</exception>
        public static ClientDataArguments Parse(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            if (args.Length is < 2 or > 3)
            {
                throw new ClientDataArgumentException($"Expected 2 or 3 arguments but received {args.Length}.");
            }

            if (string.IsNullOrWhiteSpace(args[0]))
            {
                throw new ClientDataArgumentException("ClientProjectName must not be empty.");
            }

            if (string.IsNullOrWhiteSpace(args[1]))
            {
                throw new ClientDataArgumentException("ClientProjectFile must not be empty.");
            }

            string? schemaVersion = args.Length == 3 && string.IsNullOrWhiteSpace(args[2]) is false ? args[2] : null;

            return new ClientDataArguments(args[0], args[1], schemaVersion);
        }
    }
}
