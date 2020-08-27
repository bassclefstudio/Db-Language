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
        /// Gets the <see cref="MemoryItem"/> in this group with the specified key.
        /// </summary>
        /// <param name="key">The unique identifier for the memory item.</param>
        MemoryItem Get(string key);

        /// <summary>
        /// Sets the data store of this <see cref="MemoryItem"/> to a new <see cref="DataObject"/>.
        /// </summary>
        /// <param name="value">The value to set. The <paramref name="value"/> must have a <see cref="BassClefStudio.DbLanguage.Core.Data.DataObject.DataType"/> that inherits from or is the <see cref="MemoryItem.DataType"/>.</param>
        /// <param name="key">The unique identifier for the memory item.</param>
        /// <returns>A <see cref="bool"/> representing whether the operation was a success.</returns>
        bool Set(string key, DataObject value);

        /// <summary>
        /// Checks to see if a memory item with the given key exists in the group.
        /// </summary>
        /// <param name="key">The unique identifier for the memory item.</param>
        /// <returns><see cref="bool"/> value indicating the result of the check.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets every identifier key in the memory group.
        /// </summary>
        /// <returns>An array of all keys.</returns>
        string[] GetKeys();
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
        /// <param name="memoryGroup">The memory group to add.</param>
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
        /// Checks to see if a <see cref="MemoryItem"/> exists in memory, using the public properties of <see cref="DataObject"/>s as <see cref="IMemoryGroup"/> objects themselves.
        /// </summary>
        /// <param name="group">The base memory group.</param>
        /// <param name="path">The dot-delimited path to the desired <see cref="MemoryItem"/>.</param>
        /// <returns><see cref="bool"/> value indicating the result of the check.</returns>
        public static bool ContainsPath(this IMemoryGroup group, Namespace path)
        {
            if (group.ContainsKey(path.NameParts[0]))
            {
                MemoryItem current = group.Get(path.NameParts[0]);
                foreach (var part in path.NameParts.Skip(1))
                {
                    if (current.Value.ContainsKey(part))
                    {
                        current = current.Value.Get(part);
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a <see cref="MemoryItem"/> stored in memory, using the public properties of <see cref="DataObject"/>s as <see cref="IMemoryGroup"/> objects themselves.
        /// </summary>
        /// <param name="group">The base memory group.</param>
        /// <param name="path">The dot-delimited path to the desired <see cref="MemoryItem"/>.</param>
        /// <returns>The <see cref="MemoryItem"/> property value.</returns>
        public static MemoryItem GetPath(this IMemoryGroup group, Namespace path)
        {
            MemoryItem current = group.Get(path.NameParts[0]);
            foreach (var part in path.NameParts.Skip(1))
            {
                current = current.Value.Get(part);
            }
            return current;
        }

        /// <summary>
        /// Sets a <see cref="MemoryItem"/> stored in memory, using the public properties of <see cref="DataObject"/>s as <see cref="IMemoryGroup"/> objects themselves.
        /// </summary>
        /// <param name="group">The base memory group.</param>
        /// <param name="path">The dot-delimited path to the desired <see cref="MemoryItem"/>.</param>
        /// <param name="value">The value to set. The <paramref name="value"/> must have a <see cref="BassClefStudio.DbLanguage.Core.Data.DataObject.DataType"/> that inherits from or is the <see cref="MemoryItem.DataType"/>.</param>
        /// <returns>A <see cref="bool"/> indicating whether the operation succeeded.</returns>
        public static bool SetPath(this IMemoryGroup group, Namespace path, DataObject value)
        {
            MemoryItem current = group.Get(path.NameParts[0]);
            foreach (var part in path.NameParts.Skip(1).Take(path.NameParts.Length - 2))
            {
                current = current.Value.Get(part);
            }
            return current.Value.Set(path.NameParts.Last(), value);
        }
    }
}
