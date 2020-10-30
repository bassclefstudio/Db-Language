using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Memory
{
    /// <summary>
    /// An <see cref="Exception"/> thrown when issues in setting or getting <see cref="MemoryItem"/>s, dealing with <see cref="MemoryProperty"/> objects, or creating and managing <see cref="IMemoryGroup"/>s occur.
    /// </summary>
    [Serializable]
    public class MemoryException : Exception
    {
        /// <inheritdoc/>
        public MemoryException() { }
        /// <inheritdoc/>
        public MemoryException(string message) : base(message) { }
        /// <inheritdoc/>
        public MemoryException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected MemoryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
