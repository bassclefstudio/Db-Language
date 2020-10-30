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
        public MemoryProperty[] GetKeys()
        {
            return Items.Select(i => i.Property).ToArray();
        }

        /// <inheritdoc/>
        public bool ContainsKey(MemoryProperty property)
        {
            return Items.Any(i => i.Property == property);
        }

        /// <inheritdoc/>
        public MemoryItem Get(MemoryProperty property)
        {
            return Items.First(i => i.Property == property);
        }

        /// <inheritdoc/>
        public void Set(MemoryProperty property, DataObject value)
        {
            if (ContainsKey(property))
            {
                Items.First(i => i.Property == property).Set(value);
            }
            else
            {
                throw new MemoryException($"Attempted to set the value of property {property.Key} which does not exist in this MemoryGroup.");
            }
        }

        /// <inheritdoc/>
        public bool Add(MemoryItem item)
        {
            if (ContainsKey(item.Property))
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
