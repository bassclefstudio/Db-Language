using BassClefStudio.DbLanguage.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Memory
{
    /// <summary>
    /// Represents the full memory of a <see cref="DataObject"/> with support for scopes and layering of memory for different accessors (scripts, other objects, etc.).
    /// </summary>
    public class MemoryStack : IWritableMemoryStack
    {
        /// <summary>
        /// Represents an ordered list of layers of memory for each scope.
        /// </summary>
        public List<IMemoryGroup> Layers { get; }

        /// <summary>
        /// Represents the writable top layer (if available) for adding items to the memory stack.
        /// </summary>
        public IWritableMemoryGroup WritableLayer { get; private set; }

        /// <summary>
        /// Creates a memory stack from an ordered collection of layers.
        /// </summary>
        /// <param name="layers">The layers of the memory stack, of type <see cref="IMemoryGroup"/>.</param>
        public MemoryStack(IEnumerable<IMemoryGroup> layers)
        {
            Layers = new List<IMemoryGroup>(layers);
            ////Sets writable layer to the new top layer if it can be written to.
            WritableLayer = Layers.Last() as IWritableMemoryGroup;
        }

        /// <summary>
        /// Creates an empty memory stack.
        /// </summary>
        public MemoryStack()
        {
            Layers = new List<IMemoryGroup>();
        }

        /// <inheritdoc/>
        public IEnumerable<IMemoryGroup> GetLayers() => Layers;

        /// <inheritdoc/>
        public bool Push(IMemoryGroup memoryGroup)
        {
            if(new List<IMemoryGroup>(Layers) { memoryGroup }.CanLink())
            {
                Layers.Add(memoryGroup);
                WritableLayer = memoryGroup as IWritableMemoryGroup;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public void Push() => Push(new MemoryGroup());

        /// <inheritdoc/>
        public void Pull()
        {
            Layers.Remove(Layers.Last());
            ////Sets writable layer to the new top layer if it can be written to.
            WritableLayer = Layers.Last() as IWritableMemoryGroup;
        }

        /// <inheritdoc/>
        public string[] GetKeys()
        {
            return Layers.SelectMany(l => l.GetKeys()).ToArray();
        }

        /// <inheritdoc/>
        public bool ContainsKey(string key)
        {
            return GetKeys().Contains(key);
        }

        /// <summary>
        /// Internal - gets the group in <see cref="Layers"/> that contains the specified key.
        /// </summary>
        /// <param name="key">The unique identifier for the memory item.</param>
        private IMemoryGroup GetGroupFor(string key)
        {
            return Layers.First(l => l.ContainsKey(key));
        }

        /// <inheritdoc/>
        public MemoryItem Get(string key)
        {
            return GetGroupFor(key).Get(key);
        }

        /// <inheritdoc/>
        public bool Set(string key, DataObject value)
        {
            if(ContainsKey(key))
            {
                return GetGroupFor(key).Set(key, value);
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Add(MemoryItem item)
        {
            if (ContainsKey(item.Property.Key) || WritableLayer == null)
            {
                return false;
            }
            else
            {
                return WritableLayer.Add(item);
            }
        }
    }
}
