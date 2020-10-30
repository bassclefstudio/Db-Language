using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Memory
{
    /// <summary>
    /// A basic group of <see cref="MemoryItem"/> objects with get and set operations.
    /// </summary>
    public interface IMemoryGroup
    {
        /// <summary>
        /// Gets the <see cref="MemoryItem"/> in this group with the specified <see cref="MemoryProperty"/>.
        /// </summary>
        /// <param name="property">The <see cref="MemoryProperty"/> describing the desired item in memory.</param>
        MemoryItem Get(MemoryProperty property);

        /// <summary>
        /// Sets the data store of <see cref="MemoryItem"/> with the specified <see cref="MemoryProperty"/> to a given <see cref="DataObject"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to set. The <paramref name="value"/> must have a <see cref="DataObject.DataType"/> that inherits from or is the <see cref="MemoryProperty.Type"/>.</param>
        /// <param name="property">The <see cref="MemoryProperty"/> describing the desired item in memory.</param>
        /// <returns>A <see cref="bool"/> representing whether the operation was a success.</returns>
        void Set(MemoryProperty property, DataObject value);

        /// <summary>
        /// Checks to see if a memory item attached to the given <see cref="MemoryProperty"/> exists in the group.
        /// </summary>
        /// <param name="property">The <see cref="MemoryProperty"/> describing the desired item in memory.</param>
        /// <returns>A <see cref="bool"/> value indicating the result of the check.</returns>
        bool ContainsKey(MemoryProperty property);

        /// <summary>
        /// Gets every identifier key in the memory group.
        /// </summary>
        /// <returns>An array of all keys.</returns>
        MemoryProperty[] GetKeys();
    }

    /// <summary>
    /// A basic group of <see cref="MemoryItem"/> objects with get, set, and add operations.
    /// </summary>
    public interface IWritableMemoryGroup : IMemoryGroup
    {
        /// <summary>
        /// Adds a new <see cref="MemoryItem"/> to the memory store.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>A <see cref="bool"/> value indicating whether the operation succeeded.</returns>
        bool Add(MemoryItem item);
    }

    /// <summary>
    /// A stack of <see cref="IMemoryGroup"/> objects with collective get and set operations.
    /// </summary>
    public interface IMemoryStack : IMemoryGroup
    {
        /// <summary>
        /// Gets the layers in the stack.
        /// </summary>
        /// <returns>A collection of <see cref="IMemoryGroup"/> layers.</returns>
        IEnumerable<IMemoryGroup> GetLayers();

        /// <summary>
        /// Adds a layer to the top of the stack.
        /// </summary>
        /// <param name="newLayer">The memory group to add.</param>
        /// <returns>A <see cref="bool"/> indicating whether the operation succeeded.</returns>
        bool Push(IMemoryGroup newLayer);

        /// <summary>
        /// Adds an empty <see cref="IWritableMemoryGroup"/> layer to the top of the stack.
        /// </summary>
        void Push();

        /// <summary>
        /// Removes the top layer from of the memory stack.
        /// </summary>
        void Pull();
    }

    /// <summary>
    /// A stack of <see cref="IMemoryGroup"/> objects with collective get, set, and add operations.
    /// </summary>
    public interface IWritableMemoryStack : IWritableMemoryGroup, IMemoryStack
    {
    }

    /// <summary>
    /// Represents extension methods to the <see cref="IMemoryGroup"/> and related types.
    /// </summary>
    public static class MemoryExtensions
    {
        /// <summary>
        /// Checks whether a collection of <see cref="IMemoryGroup"/> objects can be linked (i.e. they have unique keys)
        /// </summary>
        /// <param name="memoryGroups">The collection of <see cref="IMemoryGroup"/> to check.</param>
        /// <returns><see cref="bool"/> value indicating the result of the check.</returns>
        public static bool CanLink(this IEnumerable<IMemoryGroup> memoryGroups)
        {
            var allKeys = memoryGroups.SelectMany(g => g.GetKeys());

            ////Checks that all keys are unique
            var diffChecker = new HashSet<string>();
            bool allDifferent = allKeys.All(s => diffChecker.Add(s));
            return allDifferent;
        }

        /// <summary>
        /// Checks to see if a memory item with the given key exists in the group.
        /// </summary>
        /// <param name="group">The given <see cref="IMemoryGroup"/>.</param>
        /// <param name="key">The unique identifier for the memory item.</param>
        /// <returns><see cref="bool"/> value indicating the result of the check.</returns>
        public static bool ContainsKey(this IMemoryGroup group, string key)
        {
            return group.GetKeyNames().Contains(key);
        }

        /// <summary>
        /// Gets every identifier key name in the memory group.
        /// </summary>
        /// <param name="group">The given <see cref="IMemoryGroup"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all <see cref="string"/> key names.</returns>
        public static IEnumerable<string> GetKeyNames(this IMemoryGroup group)
        {
            return group.GetKeys().Select(p => p.Key).ToArray();
        }

    }
}
