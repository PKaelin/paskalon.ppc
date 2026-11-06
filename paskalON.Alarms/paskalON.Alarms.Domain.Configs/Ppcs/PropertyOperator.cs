// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Configs.Ppcs
{
    /// <summary>
    /// Operator used to condition the value associated with the property name against the configured property value.
    /// </summary>
    public enum PropertyOperator
    {
        /// <summary>
        /// Value associated with the property name == property value.
        /// </summary>
        Equal = 0,


        /// <summary>
        /// Value associated with the property name != property value.
        /// </summary>
        NotEqual = 1,


        /// <summary>
        /// Value associated with the property name &gt; property value.
        /// </summary>
        GreaterThan = 2,


        /// <summary>
        /// Value associated with the property name &lt; property value.
        /// </summary>
        SmallerThan = 3,


        /// <summary>
        /// Value associated with the property name &gt;= property value.
        /// </summary>
        GreaterThanOrEqual = 4,


        /// <summary>
        /// Value associated with the property name &lt;= property value.
        /// </summary>
        SmallerThanOrEqual = 5
    }
}
