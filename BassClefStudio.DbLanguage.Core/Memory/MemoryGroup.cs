using BassClefStudio.DbLanguage.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Memory
{
    /// <summary>
    /// Represents a collection of <see cref="MemoryItem"/> objects that are part of a similar scope.
    /// </summary>
    public class MemoryGroup : IWritableMemoryGroup
    {
        /// <summary>
        /// The list of all memory items in the layer.
        /// </summary>
        public List<MemoryItem> Items { get; }

        /// <summary>
        /// Creates a generic memory store from the collection of <see cref="MemoryItem"/>.
        /// </summary>
        /// <param name="items">A list of items to add to memory.</param>
        public MemoryGroup(IEnumerable<MemoryItem> items)
        {
            Items = new List<MemoryItem>(items);
        }

        /// <summary>
        /// Creates a generic memory store from a collection of <see cref="MemoryProperty"/>.
        /// </summary>
        /// <param name="properties">A collection of <see cref="MemoryProperty"/>. For each property, a new <see cref="MemoryItem"/> will be added to memory with that property.</param>
        public MemoryGroup(IEnumerable<MemoryProperty> properties)
        {
            Items = properties.Select(p => new MemoryItem(p)).ToList();
        }

        /// <summary>
        /// Creates an empty generic memory store.
        /// </summary>
        public MemoryGroup()
        {
            Items = new List<MemoryItem>();
        }

        /// <inheritdoc/>
        public string[] GetKeys()
        {
            return Items.Select(i => i.Property.Key).ToArray();
        }

        /// <inheritdoc/>
        public bool ContainsKey(string key)
        {
            return Items.Any(i => i.Property.Key == key);
        }

        /// <inheritdoc/>
        public MemoryItem Get(string key)
        {
            return Items.First(i => i.Property.Key == key);
        }

        /// <inheritdoc/>
        public bool Set(string key, DataObject value)
        {
            if (ContainsKey(key))
            {
                return Items.First(i => i.Property.Key == key).Set(value);
            }
            else
            {
                return false;
            }
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
                Items.Add(item);
                return true;
            }
        }
    }
}
