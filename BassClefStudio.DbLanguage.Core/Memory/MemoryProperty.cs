using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Runtime.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Memory
{
    /// <summary>
    /// Represents a property in the memory space - i.e., an identifier of a space in memory which specifies a <see cref="string"/> key, <see cref="DataType"/>, and various <see cref="PropertyFlags"/>.
    /// </summary>
    public class MemoryProperty
    {
        /// <summary>
        /// The default <see cref="PropertyFlags"/> that populate <see cref="MemoryProperty.Flags"/>.
        /// </summary>
        public const PropertyFlags DefaultFlags = PropertyFlags.Get | PropertyFlags.Set;

        /// <summary>
        /// The unique key for this memory location.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The required type that all items in this memory location must inherit from.
        /// </summary>
        public IType Type { get; }
        
        /// <summary>
        /// A collection of flags that indicate how this memory can be read from and written to.
        /// </summary>
        public PropertyFlags Flags { get; }

        /// <summary>
        /// Creates a new property, given the 
        /// </summary>
        /// <param name="key">The unique key for the property.</param>
        /// <param name="type">The <see cref="IType"/> of data stored in the property.</param>
        /// <param name="flags">Optionally, a new property can specify specific flags to be set that control how the memory can be accessed. For more information, see <seealso cref="PropertyFlags"/>.</param>
        public MemoryProperty(string key, IType type, PropertyFlags flags = DefaultFlags)
        {
            Key = key;
            Type = type;
            if (Type == null)
            {
                throw new ArgumentException("The type parameter of a MemoryProperty is required and cannot be null.");
            }
            Flags = flags;
        }

        /// <inheritdoc/>
        public static bool operator ==(MemoryProperty a, MemoryProperty b)
        {
            return a.Key == b.Key && a.Type == b.Type && a.Flags == b.Flags;
        }

        /// <inheritdoc/>
        public static bool operator !=(MemoryProperty a, MemoryProperty b)
        => !(a == b);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is MemoryProperty property && this == property;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Contains extension methods for dealing with <see cref="MemoryProperty"/> objects and their associated <see cref="PropertyFlags"/>.
    /// </summary>
    public static class PropertyExtensions
    {
        /// <summary>
        /// Returns a selection of the given <see cref="MemoryProperty"/>s where the <see cref="PropertyFlags.Static"/> flag is either set or unset (depending on the <paramref name="isStatic"/> <see cref="bool"/>).
        /// </summary>
        /// <param name="properties">This given collection of <see cref="MemoryProperty"/>s.</param>
        /// <param name="isStatic">A <see cref="bool"/> indicating whether the returned properties should be static or not.</param>
        public static IEnumerable<MemoryProperty> GetForStatic(this IEnumerable<MemoryProperty> properties, bool isStatic)
        {
            return properties.Where(p => p.Flags.HasFlag(PropertyFlags.Static) == isStatic);
        }
    }

    /// <summary>
    /// Represents a list of flags for a <see cref="MemoryProperty"/> that specify different actions.
    /// </summary>
    [Flags]
    public enum PropertyFlags
    {
        /// <summary>
        /// Indicates that all code with access to this location in memory can read the stored value.
        /// </summary>
        Get = 1,

        /// <summary>
        /// Indicates that all code with access to this location in memory can write values to that location.
        /// </summary>
        Set = 2,

        /// <summary>
        /// Indicates that the memory belongs to a static <see cref="DataObject"/> and cannot be accessed in instance objects.
        /// </summary>
        Static = 4,

        /// <summary>
        /// Indicates that the memory is part of the scope of an internal <see cref="Thread"/> and is used for local variables.
        /// </summary>
        Scoped = 8
    }
}
