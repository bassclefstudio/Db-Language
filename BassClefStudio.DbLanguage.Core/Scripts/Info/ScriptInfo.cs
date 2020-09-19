using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Documentation;
using BassClefStudio.DbLanguage.Core.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Scripts.Info
{
    /// <summary>
    /// Represents the metadata for a <see cref="Script"/>, including its <see cref="System.Documentation.Namespace"/> and the input <see cref="DataStructure"/>s.
    /// </summary>
    public class ScriptInfo
    {
        /// <summary>
        /// The inputs of the <see cref="Script"/>, as <see cref="ScriptInput"/> objects indicating the desired type and name of the input.
        /// </summary>
        public ScriptInput[] Inputs { get; }

        /// <summary>
        /// The name of the script, which should be a unique name within the containing <see cref="DataObject"/>.
        /// </summary>
        public Namespace Namespace { get; }

        /// <summary>
        /// The <see cref="IType"/> of objects returned from the parent <see cref="Script"/>.
        /// </summary>
        public IType ReturnType { get; }

        /// <summary>
        /// Creates a new <see cref="ScriptInfo"/> object with a script name and collection of inputs.
        /// </summary>
        /// <param name="name">The name of the script.</param>
        /// <param name="inputs">A collection of <see cref="ScriptInput"/> values indicating the types and names of the inputs.</param>
        /// <param name="returnType">The <see cref="IType"/> of objects returned from the parent <see cref="Script"/>.</param>
        public ScriptInfo(Namespace name, IType returnType, params ScriptInput[] inputs)
        {
            Namespace = name;
            Inputs = inputs;
            ReturnType = returnType;
        }

        /// <summary>
        /// Gets a unique identifier for a <see cref="Threading.Thread"/> running the <see cref="Script"/> based on the name of the <see cref="Script"/>.
        /// </summary>
        public string GetUniqueId()
        {
            return $"{Namespace}_{Guid.NewGuid():N}";
        }

        /// <summary>
        /// Creates the <see cref="IWritableMemoryGroup"/> that will be used by the running <see cref="Threading.Thread"/> as the input memory, used for local variables.
        /// </summary>
        /// <param name="dataObjects">The ordered list of input <see cref="DataObject"/>s, which will be converted using the </param>
        public IWritableMemoryGroup CreateMemoryFromInputs(DataObject[] dataObjects)
        {
            List<MemoryItem> memoryItems = new List<MemoryItem>();
            for (int i = 0; i < dataObjects.Length; i++)
            {
                memoryItems.Add(
                    Inputs[i].CreateMemoryItem(dataObjects[i]));
            }

            return new MemoryGroup(memoryItems);
        }
    }

}
