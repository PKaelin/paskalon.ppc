// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Loads the localized texts of a client project from its text files and checks that every localization key
    /// used by the client project file has a text in the default language.
    /// </summary>
    /// <remarks>
    /// - A text file is next to the client project file and named [ClientProject].Texts.[Language].csv, e.g. DemoBattery.Texts.en.csv.
    /// - A text file has the same line format as the client project file but only contains text entities.
    /// - A value written as {{Key}} is a localization key, any other value is plain text and not checked.
    /// </remarks>
    /// <typeparam name="TText">The entity type of a localized text.</typeparam>
    public class LocalizedTextSupplement<TText> : IClientDataSupplement where TText : class
    {
        /// <summary>
        /// Part of the file name between the client project name and the language.
        /// </summary>
        public const string TextFileInfix = ".Texts.";


        /// <summary>
        /// Extension of the text files.
        /// </summary>
        private const string TextFileExtension = ".csv";


        /// <summary>
        /// Gets the localization key of the text.
        /// </summary>
        private readonly Func<TText, string> _keySelector;


        /// <summary>
        /// Gets the language of the text.
        /// </summary>
        private readonly Func<TText, string> _languageSelector;


        /// <summary>
        /// Gets the property names and values of an entity of the client project file that can be localization keys.
        /// </summary>
        private readonly Func<object, IEnumerable<KeyValuePair<string, string?>>> _localizableValuesSelector;


        /// <summary>
        /// Constructor of <see cref="LocalizedTextSupplement{TText}"/>.
        /// </summary>
        /// <param name="keySelector">Gets the localization key of the text.</param>
        /// <param name="languageSelector">Gets the language of the text.</param>
        /// <param name="localizableValuesSelector">Gets the property names and values of an entity of the client project file that can be localization keys.</param>
        /// <param name="defaultLanguage">The language every used localization key needs a text in, e.g. en.</param>
        public LocalizedTextSupplement(Func<TText, string> keySelector, Func<TText, string> languageSelector,
            Func<object, IEnumerable<KeyValuePair<string, string?>>> localizableValuesSelector, string defaultLanguage)
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            ArgumentNullException.ThrowIfNull(languageSelector);
            ArgumentNullException.ThrowIfNull(localizableValuesSelector);
            ArgumentException.ThrowIfNullOrWhiteSpace(defaultLanguage);

            _keySelector = keySelector;
            _languageSelector = languageSelector;
            _localizableValuesSelector = localizableValuesSelector;
            DefaultLanguage = defaultLanguage;
        }


        /// <summary>
        /// The language every used localization key needs a text in.
        /// </summary>
        public string DefaultLanguage { get; }


        /// <inheritdoc/>
        public IReadOnlyCollection<Type> EntityTypes { get; } = new Type[] { typeof(TText) };


        /// <summary>
        /// Finds the text files of the client project file in its folder, ordered by name.
        /// </summary>
        /// <param name="clientProjectFile">The client project file.</param>
        /// <returns>The paths of the text files.</returns>
        public static IReadOnlyList<string> FindTextFiles(string clientProjectFile)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(clientProjectFile);

            string directory = Path.GetDirectoryName(Path.GetFullPath(clientProjectFile)) ?? string.Empty;
            string prefix = GetTextFilePrefix(clientProjectFile);

            // The search pattern *.csv also matches longer extensions on Windows (e.g. .csvx), therefore the extension is checked again
            return Directory.EnumerateFiles(directory, $"{prefix}*{TextFileExtension}")
                .Where(file => file.EndsWith(TextFileExtension, StringComparison.OrdinalIgnoreCase))
                .Order(StringComparer.Ordinal)
                .ToList();
        }


        /// <summary>
        /// Gets the language of a text file from its name, e.g. en of DemoBattery.Texts.en.csv.
        /// </summary>
        /// <param name="clientProjectFile">The client project file.</param>
        /// <param name="textFile">The text file of the client project file.</param>
        /// <returns>The language of the file name.</returns>
        public static string GetLanguage(string clientProjectFile, string textFile)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(clientProjectFile);
            ArgumentException.ThrowIfNullOrWhiteSpace(textFile);

            string fileName = Path.GetFileName(textFile);
            string prefix = GetTextFilePrefix(clientProjectFile);

            if (fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) is false || fileName.EndsWith(TextFileExtension, StringComparison.OrdinalIgnoreCase) is false)
            {
                throw new ArgumentException($"The file '{fileName}' is not a text file of the client project, expected {prefix}<Language>{TextFileExtension}.", nameof(textFile));
            }

            return fileName[prefix.Length..^TextFileExtension.Length];
        }


        /// <inheritdoc/>
        public async Task<IReadOnlyList<object>> LoadAsync(string clientProjectFile, IReadOnlyList<ClientDataEntity> projectEntities, DateTimeOffset changedDate,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(clientProjectFile);
            ArgumentNullException.ThrowIfNull(projectEntities);

            List<ClientDataTextFile> textFiles = new List<ClientDataTextFile>();

            foreach (string textFile in FindTextFiles(clientProjectFile))
            {
                string[] lines = await File.ReadAllLinesAsync(textFile, cancellationToken);
                textFiles.Add(new ClientDataTextFile(textFile, GetLanguage(clientProjectFile, textFile), lines));
            }

            return Load(clientProjectFile, textFiles, projectEntities, changedDate);
        }


        /// <summary>
        /// Creates the texts of the text files and checks that every localization key of the client project file has a text in the default language.
        /// </summary>
        /// <param name="clientProjectFile">The client project file, used for the error messages.</param>
        /// <param name="textFiles">The content of the text files.</param>
        /// <param name="projectEntities">The entities of the client project file with their positions.</param>
        /// <param name="changedDate">The date all the texts are changed at.</param>
        /// <returns>The texts of all the text files.</returns>
        /// <exception cref="ClientDataValidationException">Thrown with all the errors if a text file is not valid or a localization key has no text.</exception>
        public IReadOnlyList<object> Load(string clientProjectFile, IReadOnlyList<ClientDataTextFile> textFiles, IReadOnlyList<ClientDataEntity> projectEntities,
            DateTimeOffset changedDate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(clientProjectFile);
            ArgumentNullException.ThrowIfNull(textFiles);
            ArgumentNullException.ThrowIfNull(projectEntities);

            List<ClientDataError> errors = new List<ClientDataError>();
            List<object> texts = new List<object>();
            HashSet<string> defaultLanguageKeys = new HashSet<string>(StringComparer.Ordinal);
            // A text file can only contain texts, any other entity type is reported as unknown
            Dictionary<string, Type> textTypes = new Dictionary<string, Type>(StringComparer.Ordinal) { [typeof(TText).Name] = typeof(TText) };
            ClientDataLoader loader = new ClientDataLoader(textTypes, changedDate);

            foreach (ClientDataTextFile textFile in textFiles)
            {
                IReadOnlyList<ClientDataEntity> entities;

                try
                {
                    entities = loader.LoadEntities(textFile.File, textFile.Lines);
                }
                catch (ClientDataValidationException ex)
                {
                    errors.AddRange(ex.Errors);

                    continue;
                }

                foreach (ClientDataEntity entity in entities)
                {
                    TText text = (TText)entity.Entity;
                    string language = _languageSelector(text);

                    if (string.Equals(language, textFile.Language, StringComparison.Ordinal) is false)
                    {
                        errors.Add(new ClientDataError(entity.File, entity.LineNumber, entity.EntityName, "Language",
                            $"The language '{language}' does not match the language '{textFile.Language}' of the file name."));

                        continue;
                    }

                    if (string.Equals(language, DefaultLanguage, StringComparison.Ordinal))
                    {
                        defaultLanguageKeys.Add(_keySelector(text));
                    }

                    texts.Add(text);
                }
            }

            string defaultTextFile = $"{GetTextFilePrefix(clientProjectFile)}{DefaultLanguage}{TextFileExtension}";

            foreach (ClientDataEntity projectEntity in projectEntities)
            {
                foreach (KeyValuePair<string, string?> value in _localizableValuesSelector(projectEntity.Entity))
                {
                    if (LocalizationKey.TryParse(value.Value, out string key) && defaultLanguageKeys.Contains(key) is false)
                    {
                        errors.Add(new ClientDataError(projectEntity.File, projectEntity.LineNumber, projectEntity.EntityName, value.Key,
                            $"The localization key '{key}' has no text in the language '{DefaultLanguage}', add it to the file {defaultTextFile}."));
                    }
                }
            }

            if (errors.Count > 0)
            {
                throw new ClientDataValidationException(errors);
            }

            return texts;
        }


        /// <summary>
        /// Gets the start of the text file names of a client project file, e.g. DemoBattery.Texts.
        /// </summary>
        /// <param name="clientProjectFile">The client project file.</param>
        /// <returns>The start of the text file names.</returns>
        private static string GetTextFilePrefix(string clientProjectFile)
        {
            return $"{Path.GetFileNameWithoutExtension(clientProjectFile)}{TextFileInfix}";
        }
    }
}
