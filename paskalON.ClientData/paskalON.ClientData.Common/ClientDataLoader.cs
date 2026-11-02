// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Creates the domain entities defined in the lines of a client project file.
    /// </summary>
    public class ClientDataLoader
    {
        /// <summary>
        /// The user all the entities are changed by.
        /// </summary>
        public const string ChangedBy = "System Init";

        /// <summary>
        /// Name of the column that defines the entity type in a line.
        /// </summary>
        private const string TypeColumn = "Type";

        /// <summary>
        /// Name of the column that defines the entity name in a line.
        /// </summary>
        private const string NameColumn = "Name";


        /// <summary>
        /// Represents the string literal used to indicate a null value.
        /// </summary>
        private const string NullValue = "null";


        /// <summary>
        /// Represents the character used to separate items in a list.
        /// </summary>
        private const char ListSeparator = ';';


        /// <summary>
        /// Names of domain base properties managed automatically.
        /// </summary>
        private static readonly string[] _automaticProperties = [nameof(DomainBase.Id), nameof(DomainBase.ChangedBy), nameof(DomainBase.ChangedDate)];


        /// <summary>
        /// Maps entity names to their corresponding CLR types.
        /// </summary>
        private readonly IReadOnlyDictionary<string, Type> _entityTypes;


        /// <summary>
        /// Represents the date and time when the associated value changed.
        /// </summary>
        private readonly DateTimeOffset _changedDate;


        /// <summary>
        /// Creates the loader.
        /// </summary>
        /// <param name="entityTypes">The entity types that can be created by their class name.</param>
        /// <param name="changedDate">The date all the entities are changed at.</param>
        public ClientDataLoader(IReadOnlyDictionary<string, Type> entityTypes, DateTimeOffset changedDate)
        {
            ArgumentNullException.ThrowIfNull(entityTypes);

            _entityTypes = entityTypes;
            _changedDate = changedDate;
        }


        /// <summary>
        /// Validates all the lines and creates their entities in the order of the file.
        /// </summary>
        /// <param name="file">The name of the file, used for the error messages.</param>
        /// <param name="lines">The lines of the file.</param>
        /// <returns>The created entities.</returns>
        /// <exception cref="ClientDataValidationException">Thrown with all the errors if at least one line is not valid.</exception>
        public IReadOnlyList<object> Load(string file, IReadOnlyList<string> lines)
        {
            ArgumentException.ThrowIfNullOrEmpty(file);
            ArgumentNullException.ThrowIfNull(lines);

            List<ClientDataError> errors = new List<ClientDataError>();
            List<object> entities = new List<object>();
            Dictionary<string, object> entitiesByName = new Dictionary<string, object>(StringComparer.Ordinal);

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith('#') || string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }

                // Parses the line and creates the entity (domain object) if its valid.
                object? entity = LoadLine(new LineContext(file, i + 1, errors), lines[i], entitiesByName);

                if (entity is not null)
                {
                    entities.Add(entity);
                }
            }

            if (errors.Count > 0)
            {
                throw new ClientDataValidationException(errors);
            }

            return entities;
        }


        /// <summary>
        /// Creates the entity of a line.
        /// </summary>
        /// <param name="line">The position in the file.</param>
        /// <param name="text">The text of the line.</param>
        /// <param name="entitiesByName">The entities previously defined in the file.</param>
        /// <returns>The entity or null if it could not be created.</returns>
        private object? LoadLine(LineContext line, string text, Dictionary<string, object> entitiesByName)
        {
            IReadOnlyList<CsvCell> cells;

            try
            {
                cells = CsvLineParser.Parse(text);
            }
            catch (FormatException ex)
            {
                line.AddError(null, null, ex.Message);

                return null;
            }

            if (cells[0].Key != TypeColumn)
            {
                line.AddError(null, null, $"The first column must be {TypeColumn}=<entity type>.");

                return null;
            }

            bool hasName = cells.Count > 1 && cells[1].Key == NameColumn;
            string? name = hasName ? cells[1].Value : null;

            if (_entityTypes.TryGetValue(cells[0].Value, out Type? entityType) is false)
            {
                line.AddError(name, null, $"Unknown entity type '{cells[0].Value}'.");

                return null;
            }

            bool isDomain = typeof(DomainBase).IsAssignableFrom(entityType);

            if (isDomain && string.IsNullOrWhiteSpace(name))
            {
                line.AddError(name, null, $"The second column must be {NameColumn}=<entity name>.");

                return null;
            }

            if (name is not null && entitiesByName.ContainsKey(name))
            {
                line.AddError(name, null, $"An entity with the name '{name}' is already defined.");

                return null;
            }

            object? entity = CreateEntity(line, entityType, name, isDomain);

            if (entity is null)
            {
                return null;
            }

            if (name is not null)
            {
                entitiesByName.Add(name, entity);
            }

            PopulateProperties(line, entity, name, cells.Skip(hasName ? 2 : 1).ToList(), entitiesByName);

            return entity;
        }


        /// <summary>
        /// Creates the entity and sets the automatic properties and the name.
        /// </summary>
        /// <param name="line">The position in the file.</param>
        /// <param name="entityType">The entity type.</param>
        /// <param name="name">The entity name if there is one.</param>
        /// <param name="isDomain">Whether the entity is a domain entity.</param>
        /// <returns>The entity or null if it could not be created.</returns>
        private object? CreateEntity(LineContext line, Type entityType, string? name, bool isDomain)
        {
            object entity;

            try
            {
                entity = Activator.CreateInstance(entityType)!;
            }
            catch (MissingMethodException)
            {
                line.AddError(name, null, $"Entity type '{entityType.Name}' cannot be created because it has no parameterless constructor.");

                return null;
            }

            if (isDomain)
            {
                DomainBase domain = (DomainBase)entity;
                domain.ChangedBy = ChangedBy;
                domain.ChangedDate = _changedDate;
            }

            // Types without a name property use the name only to identify them within the file
            PropertyInfo? nameProperty = entityType.GetProperty(NameColumn);

            if (name is not null && nameProperty is not null && nameProperty.PropertyType == typeof(string))
            {
                nameProperty.SetValue(entity, name);
            }

            return entity;
        }


        /// <summary>
        /// Sets the properties defined by the cells and checks that no required property is missing.
        /// </summary>
        /// <param name="line">The position in the file.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="name">The entity name if there is one.</param>
        /// <param name="cells">The property cells of the line.</param>
        /// <param name="entitiesByName">The entities previously defined in the file.</param>
        private static void PopulateProperties(LineContext line, object entity, string? name, List<CsvCell> cells, Dictionary<string, object> entitiesByName)
        {
            Type entityType = entity.GetType();
            HashSet<string> definedProperties = new HashSet<string>(StringComparer.Ordinal);

            if (name is not null)
            {
                definedProperties.Add(NameColumn);
            }

            foreach (CsvCell cell in cells)
            {
                if (definedProperties.Add(cell.Key) is false)
                {
                    line.AddError(name, cell.Key, "The property is defined more than once.");

                    continue;
                }

                PropertyInfo? property = entityType.GetProperty(cell.Key, BindingFlags.Public | BindingFlags.Instance);
                string? notSettableReason = GetNotSettableReason(entityType, property, cell.Key);

                if (notSettableReason is not null)
                {
                    line.AddError(name, cell.Key, notSettableReason);

                    continue;
                }

                try
                {
                    Type? itemType = GetReferenceCollectionItemType(property!.PropertyType);

                    if (itemType is not null)
                    {
                        AddReferences(entity, property, itemType, cell, entitiesByName);
                    }
                    else
                    {
                        object? value = ConvertValue(cell, property, entitiesByName);
                        property.SetValue(entity, value);
                    }
                }
                catch (FormatException ex)
                {
                    line.AddError(name, cell.Key, ex.Message);
                }
                catch (TargetInvocationException ex)
                {
                    // The domain rejected the value
                    line.AddError(name, cell.Key, ex.InnerException?.Message ?? ex.Message);
                }
            }

            foreach (PropertyInfo property in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                bool isSetAutomatically = _automaticProperties.Contains(property.Name);

                if (isSetAutomatically is false && property.IsDefined(typeof(RequiredMemberAttribute)) && definedProperties.Contains(property.Name) is false)
                {
                    line.AddError(name, property.Name, "The required property is missing.");
                }
            }
        }


        /// <summary>
        /// Gets the reason why the property cannot be set from the file.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="property">The property or null if it does not exist.</param>
        /// <param name="propertyName">The property name in the file.</param>
        /// <returns>The reason or null if the property can be set.</returns>
        private static string? GetNotSettableReason(Type entityType, PropertyInfo? property, string propertyName)
        {
            if (_automaticProperties.Contains(propertyName))
            {
                return "The property is set automatically and must not be defined in the file.";
            }

            if (property is null)
            {
                return $"Unknown property of entity type '{entityType.Name}'.";
            }

            if (GetReferenceCollectionItemType(property.PropertyType) is not null)
            {
                // The references are added to the existing list, therefore only the getter is required
                bool hasPublicGetter = property.GetMethod is not null && property.GetMethod.IsPublic;

                if (hasPublicGetter)
                {
                    return null;
                }

                return "The property cannot be set from the file.";
            }

            bool isCollection = property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType);

            if (property.SetMethod is null || property.SetMethod.IsPublic is false || isCollection)
            {
                return "The property cannot be set from the file.";
            }

            bool isForeignKey = propertyName.EndsWith("Id", StringComparison.Ordinal) && entityType.GetProperty(propertyName[..^2]) is not null;

            if (isForeignKey)
            {
                return $"The foreign key is set by the reference property '{propertyName[..^2]}'.";
            }

            return null;
        }


        /// <summary>
        /// Converts the cell to the value of the property.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="property">The property.</param>
        /// <param name="entitiesByName">The entities previously defined in the file.</param>
        /// <returns>The value.</returns>
        /// <exception cref="FormatException">Thrown when the cell cannot be converted.</exception>
        private static object? ConvertValue(CsvCell cell, PropertyInfo property, Dictionary<string, object> entitiesByName)
        {
            if (cell.IsQuoted is false && cell.Value == NullValue)
            {
                bool isNullable = new NullabilityInfoContext().Create(property).WriteState != NullabilityState.NotNull;

                if (isNullable is false)
                {
                    throw new FormatException("The property must not be null.");
                }

                return null;
            }

            if (cell.IsQuoted is false && cell.Value.Length == 0)
            {
                throw new FormatException($"The value is empty, use {NullValue} or \"\".");
            }

            if (PropertyValueConverter.IsScalar(property.PropertyType))
            {
                return PropertyValueConverter.Convert(cell.Value, property.PropertyType);
            }

            return ResolveReference(cell.Value, property.PropertyType, entitiesByName);
        }


        /// <summary>
        /// Adds the references separated by semicolons to the existing list of the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="property">The list property.</param>
        /// <param name="itemType">The type of the list items.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="entitiesByName">The entities previously defined in the file.</param>
        /// <exception cref="FormatException">Thrown when a reference cannot be resolved.</exception>
        private static void AddReferences(object entity, PropertyInfo property, Type itemType, CsvCell cell, Dictionary<string, object> entitiesByName)
        {
            if (cell.IsQuoted is false && (cell.Value == NullValue || cell.Value.Length == 0))
            {
                throw new FormatException("The list needs at least one reference, omit the property for an empty list.");
            }

            object collection = property.GetValue(entity) ?? throw new FormatException("The list of the entity is not initialized.");
            MethodInfo add = typeof(ICollection<>).MakeGenericType(itemType).GetMethod(nameof(ICollection<object>.Add))!;
            HashSet<string> referenceNames = new HashSet<string>(StringComparer.Ordinal);

            foreach (string item in cell.Value.Split(ListSeparator))
            {
                string referenceName = item.Trim();

                if (referenceName.Length == 0)
                {
                    throw new FormatException("The list contains an empty reference.");
                }

                if (referenceNames.Add(referenceName) is false)
                {
                    throw new FormatException($"The reference '{referenceName}' is in the list more than once.");
                }

                add.Invoke(collection, [ResolveReference(referenceName, itemType, entitiesByName)]);
            }
        }


        /// <summary>
        /// Finds the previously defined entity by its name.
        /// </summary>
        /// <param name="referenceName">The name of the referenced entity.</param>
        /// <param name="expectedType">The type the entity must have.</param>
        /// <param name="entitiesByName">The entities previously defined in the file.</param>
        /// <returns>The referenced entity.</returns>
        /// <exception cref="FormatException">Thrown when the entity is not defined earlier or has the wrong type.</exception>
        private static object ResolveReference(string referenceName, Type expectedType, Dictionary<string, object> entitiesByName)
        {
            if (entitiesByName.TryGetValue(referenceName, out object? reference) is false)
            {
                throw new FormatException($"The reference '{referenceName}' is not defined earlier in the file.");
            }

            if (expectedType.IsInstanceOfType(reference) is false)
            {
                throw new FormatException($"The reference '{referenceName}' is of type '{reference.GetType().Name}' but '{expectedType.Name}' is expected.");
            }

            return reference;
        }


        /// <summary>
        /// Gets the item type if the property type is a list of entity references.
        /// </summary>
        /// <param name="type">The property type.</param>
        /// <returns>The item type or null if the type is not a list of entity references.</returns>
        private static Type? GetReferenceCollectionItemType(Type type)
        {
            bool isCollectionInterface = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>);
            Type? collectionType = isCollectionInterface ? type : type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

            if (collectionType is null)
            {
                return null;
            }

            Type itemType = collectionType.GetGenericArguments()[0];

            if (PropertyValueConverter.IsScalar(itemType))
            {
                return null;
            }

            return itemType;
        }


        /// <summary>
        /// Position in the file that collects the errors.
        /// </summary>
        /// <param name="File">The name of the file.</param>
        /// <param name="LineNumber">The 1-based line number.</param>
        /// <param name="Errors">The list the errors are added to.</param>
        private record LineContext(string File, int LineNumber, List<ClientDataError> Errors)
        {
            /// <summary>
            /// Adds an error for this line.
            /// </summary>
            /// <param name="entityName">The entity name if it is known.</param>
            /// <param name="propertyName">The property name if the error relates to a property.</param>
            /// <param name="message">The description of the error.</param>
            public void AddError(string? entityName, string? propertyName, string message)
            {
                Errors.Add(new ClientDataError(File, LineNumber, entityName, propertyName, message));
            }
        }
    }
}
