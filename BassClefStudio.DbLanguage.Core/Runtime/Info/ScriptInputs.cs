using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Runtime.Info
{
    /// <summary>
    /// Represents the input to a <see cref="Script"/>, contained in the <see cref="ScriptInfo"/> metadata, which is strongly-typed and keyed for the creation for an input <see cref="Memory.IWritableMemoryGroup"/> for the running <see cref="Threading.Thread"/>.
    /// </summary>
    public class ScriptInput
    {
        /// <summary>
        /// The required type that all values for this input must inherit from.
        /// </summary>
        public IType Type { get; }

        /// <summary>
        /// A <see cref="string"/>, unique to the <see cref="Script"/>, which names the input value as it is passed to the <see cref="Memory.IWritableMemoryGroup"/>.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Creates a <see cref="ScriptInput"/>.
        /// </summary>
        /// <param name="key">A <see cref="string"/>, unique to the <see cref="Script"/>, which names the input value.</param>
        /// <param name="type">The required type that all values for this input must inherit from.</param>
        public ScriptInput(string key, IType type)
        {
            Key = key;
            Type = type;
        }

        /// <summary>
        /// Checks if a <see cref="MemoryItem"/> for this <see cref="ScriptInput"/> can be created with the given input <see cref="DataObject"/>.
        /// </summary>
        /// <param name="dataObject">The desired value of the <see cref="MemoryItem"/>.</param>
        /// <returns>A <see cref="bool"/> value indicating the result of the check.</returns>
        public bool CanCreateMemoryItem(DataObject dataObject)
        {
            return dataObject.DataType.Is(this.Type);
        }

        /// <summary>
        /// Creates a <see cref="MemoryItem"/> of the given type and key of the <see cref="ScriptInput"/> and attempts to set its <see cref="MemoryItem.Value"/> to the given <see cref="DataObject"/>.
        /// </summary>
        /// <param name="dataObject">The desired value of the <see cref="MemoryItem"/>.</param>
        public MemoryItem CreateMemoryItem(DataObject dataObject)
        {
            if(CanCreateMemoryItem(dataObject))
            {
                var item = new MemoryItem(
                    new MemoryProperty(
                        this.Key,
                        this.Type,
                        MemoryProperty.DefaultFlags | PropertyFlags.Scoped));
                item.Set(dataObject);
                return item;
            }
            else
            {
                throw new ScriptInputException($"Could not parse input {this.Key}: input was of invalid type.");
            }
        }
    }

    public class ScriptInputException : Exception
    {
        public ScriptInputException() { }
        public ScriptInputException(string message) : base(message) { }
        public ScriptInputException(string message, Exception inner) : base(message, inner) { }
    }
}
