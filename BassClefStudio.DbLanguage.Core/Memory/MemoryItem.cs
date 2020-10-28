using BassClefStudio.DbLanguage.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Memory
{
    /// <summary>
    /// Represents a single item in memory; i.e., an object and the associated adress. This location is strongly-typed.
    /// </summary>
    public class MemoryItem
    {
        /// <summary>
        /// A <see cref="MemoryProperty"/> item describing the access to and type of this <see cref="MemoryItem"/>.
        /// </summary>
        public MemoryProperty Property { get; }

        /// <summary>
        /// The currently stored value in memory.
        /// </summary>
        public DataObject Value { get; private set; }

        /// <summary>
        /// Creates a new memory item with given property information.
        /// </summary>
        /// <param name="property">Specifies information regarding the key and type of the <see cref="MemoryItem"/>, as well as setting various <see cref="PropertyFlags"/>.</param>
        public MemoryItem(MemoryProperty property)
        {
            Property = property;
        }

        /// <summary>
        /// Sets the data store of this <see cref="MemoryItem"/> to a new <see cref="DataObject"/>.
        /// </summary>
        /// <param name="value">The value to set. The <paramref name="value"/> must have a <see cref="BassClefStudio.DbLanguage.Core.Data.DataObject.DataType"/> that inherits from or is <see cref="DataType"/>.</param>
        /// <returns>A <see cref="bool"/> representing whether the operation was a success.</returns>
        public bool Set(DataObject value)
        {
            if(value.DataType.Is(this.Property.Type))
            {
                Value = value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}