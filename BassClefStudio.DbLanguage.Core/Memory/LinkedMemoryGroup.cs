using BassClefStudio.DbLanguage.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Memory
{
    /// <summary>
    /// Represnts an <see cref="IMemoryGroup"/> which is the sum of a collection of <see cref="LinkedGroups"/>, containing the properties of each child <see cref="IMemoryGroup"/>.
    /// </summary>
    public class LinkedMemoryGroup : IMemoryGroup
    {
        /// <summary>
        /// A collection of all of the memory groups that make up the linked memory
        /// </summary>
        public IEnumerable<IMemoryGroup> LinkedGroups { get; }

        /// <summary>
        /// Creates a linked memory group from a collection of existing memory groups.
        /// </summary>
        /// <param name="linkedGroups">A collection of <see cref="IMemoryGroup"/> to add to memory.</param>
        public LinkedMemoryGroup(IEnumerable<IMemoryGroup> linkedGroups)
        {
            LinkedGroups = linkedGroups;
        }

        /// <inheritdoc/>
        public string[] GetKeys()
        {
            return LinkedGroups.SelectMany(g => g.GetKeys()).ToArray();
        }

        /// <inheritdoc/>
        public bool ContainsKey(string key)
        {
            return LinkedGroups.SelectMany(g => g.GetKeys()).Contains(key);
        }

        /// <summary>
        /// Internal - gets the group in <see cref="LinkedGroups"/> that contains the specified key.
        /// </summary>
        /// <param name="key">The unique identifier for the memory item.</param>
        private IMemoryGroup GetGroupFor(string key)
        {
            return LinkedGroups.First(g => g.ContainsKey(key));
        }

        /// <inheritdoc/>
        public MemoryItem Get(string key)
        {
            return GetGroupFor(key).Get(key);
        }

        /// <inheritdoc/>
        public bool Set(string key, DataObject value)
        {
            return GetGroupFor(key).Set(key, value);
        }
    }

    /// <summary>
    /// Represents a <see cref="LinkedMemoryGroup"/> where <see cref="MemoryItem"/>s can be written to an additional <see cref="IMemoryGroup"/> in the collection through the <see cref="IWritableMemoryGroup"/> interface methods.
    /// </summary>
    public class WritableLinkedMemoryGroup : LinkedMemoryGroup, IWritableMemoryGroup
    {
        /// <summary>
        /// The <see cref="IWritableMemoryGroup"/> in the collection of <see cref="IMemoryGroup"/> in linked memory. Used for adding new items to the memory group.
        /// </summary>
        public IWritableMemoryGroup WriteGroup { get; }

        /// <summary>
        /// Creates a linked memory group that is writable (items can be added) from a collection of existing memory groups.
        /// </summary>
        /// <param name="writeGroup">The <see cref="IWritableMemoryGroup"/> that will be used for adding new items to memory.</param>
        /// <param name="linkedGroups">A collection of <see cref="IMemoryGroup"/> to add to memory.</param>
        public WritableLinkedMemoryGroup(IWritableMemoryGroup writeGroup, IEnumerable<IMemoryGroup> linkedGroups) : base(new List<IMemoryGroup>(linkedGroups) { writeGroup })
        {
            WriteGroup = writeGroup;
        }

        /// <summary>
        /// Creates a linked memory group that is writable (items can be added) from an existing <see cref="LinkedMemoryGroup"/>.
        /// </summary>
        /// <param name="writeGroup">The <see cref="IWritableMemoryGroup"/> that will be used for adding new items to memory.</param>
        /// <param name="linkedMemory">The existing linked memory group.</param>
        public WritableLinkedMemoryGroup(IWritableMemoryGroup writeGroup, LinkedMemoryGroup linkedMemory) : base(new List<IMemoryGroup>(linkedMemory.LinkedGroups) { writeGroup })
        {
            WriteGroup = writeGroup;
        }

        /// <inheritdoc/>
        public bool Add(MemoryItem item)
        {
            if (ContainsKey(item.Property.Key))
            {
                return false;
            }
            else
            {
                return WriteGroup.Add(item);
            }
        }
    }
}
