using BassClefStudio.DbLanguage.Core.Data;
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
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="flags">Optionally, a new property can specify specific flags to be set that control how the memory can be accessed. For more information, see <seealso cref="PropertyFlags"/>.</param>
        public MemoryProperty(string key, IType type, PropertyFlags flags = DefaultFlags)
        {
            Key = key;
            Type = type;
            Flags = flags;
        }

        public static bool operator ==(MemoryProperty a, MemoryProperty b)
        {
            return a.Key == b.Key;
        }

        public static bool operator !=(MemoryProperty a, MemoryProperty b)
        => !(a == b);

        public override bool Equals(object obj)
        {
            return obj is MemoryProperty property && this == property;
        }
    }

    /// <summary>
    /// Contains extension methods for dealing with <see cref="MemoryProperty"/> objects and their associated <see cref="PropertyFlags"/>.
    /// </summary>
    public static class PropertyExtensions
    {
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
        /// Indicates that the memory is part of the scope of an internal <see cref="Scripts.Threading.Thread"/> and is used for local variables.
        /// </summary>
        Scoped = 8
    }
}
