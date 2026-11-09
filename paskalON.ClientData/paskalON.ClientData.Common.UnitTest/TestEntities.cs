// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.ClientData.Common.UnitTest
{
    /// <summary>
    /// Connection test entity.
    /// </summary>
    public class ConnectionEntity : NameBase
    {
        /// <summary>
        /// Required scalar with validation in its setter.
        /// </summary>
        public required int RetryCount
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                field = value;
            }
        }


        /// <summary>
        /// Optional text.
        /// </summary>
        public string? Description { get; set; } = "Default";


        /// <summary>
        /// Optional enum.
        /// </summary>
        public DayOfWeek? Day { get; set; }
    }


    /// <summary>
    /// Other test entity that is of the wrong type for references to a connection.
    /// </summary>
    public class OtherEntity : NameBase
    {
    }


    /// <summary>
    /// Test entity with references.
    /// </summary>
    public class DeviceEntity : NameBase
    {
        /// <summary>
        /// Foreign key.
        /// </summary>
        public int ConnectionId { get; set; }


        /// <summary>
        /// Required reference.
        /// </summary>
        public required ConnectionEntity Connection { get; set; }


        /// <summary>
        /// Optional reference.
        /// </summary>
        public ConnectionEntity? Backup { get; set; }


        /// <summary>
        /// Collection.
        /// </summary>
        public ICollection<OtherEntity> Others { get; set; } = [];


        /// <summary>
        /// Collection of scalars that cannot be set from the file.
        /// </summary>
        public ICollection<string> Tags { get; set; } = [];
    }


    /// <summary>
    /// Domain entity without a name property which has a property called Type.
    /// </summary>
    public class SettingEntity : DomainBase
    {
        /// <summary>
        /// Property with the same name as the first column.
        /// </summary>
        public DayOfWeek Type { get; set; }
    }


    /// <summary>
    /// Localized text test entity whose name is the localization key.
    /// </summary>
    public class TextEntity : DomainBase
    {
        /// <summary>
        /// Localization key without braces, validated in its setter.
        /// </summary>
        public required string Name
        {
            get;
            set
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value);

                if (value.Contains('{'))
                {
                    throw new ArgumentException("The key must be written without braces.", nameof(value));
                }

                field = value;
            }
        }


        /// <summary>
        /// Language of the text.
        /// </summary>
        public required string Language { get; set; }


        /// <summary>
        /// Text of the key.
        /// </summary>
        public required string Text { get; set; }
    }


    /// <summary>
    /// Entity that does not inherit from the domain base.
    /// </summary>
    public class PlainEntity
    {
        /// <summary>
        /// Some value.
        /// </summary>
        public int Value { get; set; }
    }
}
