using BassClefStudio.DbLanguage.Core.Documentation;
using BassClefStudio.DbLanguage.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Data
{
    /// <summary>
    /// Represents a contract that any <see cref="DataType"/> can support which enables different <see cref="DataType"/>s which share property and method types and names can be cast as one another (similar to interfaces in C#).
    /// </summary>
    public class DataContract : IType
    {
        /// <inheritdoc/>
        public Namespace TypeName { get; }

        /// <summary>
        /// A collection of <see cref="MemoryProperty"/> objects specified specifically by this <see cref="DataContract"/> (and not its parents).
        /// </summary>
        public List<MemoryProperty> ContractProperties { get; }

        /// <summary>
        /// A collection of parent <see cref="DataContract"/>s that provide <see cref="MemoryProperty"/> objects to <see cref="GetProperties"/>.
        /// </summary>
        public List<DataContract> InheritedContracts { get; }

        /// <summary>
        /// Creates a <see cref="DataContract"/> with the specified name.
        /// </summary>
        /// <param name="typeName">The full namespace of the <see cref="DataContract"/>.</param>
        public DataContract(Namespace typeName)
        {
            TypeName = typeName;
            InheritedContracts = new List<DataContract>();
            ContractProperties = new List<MemoryProperty>();
        }

        /// <summary>
        /// Gets a collection of <see cref="MemoryProperty"/> objects that <see cref="DataType"/>s inheriting this <see cref="DataContract"/> are expected to have. This only includes publicly visible properties.
        /// </summary>
        public IEnumerable<MemoryProperty> GetProperties()
        {
            return ContractProperties.Concat(InheritedContracts.SelectMany(c => c.GetProperties()));
        }

        /// <inheritdoc/>
        public bool Is(IType other)
        {
            return other is DataContract contract 
                && (other == this || InheritedContracts.Contains(contract));
        }
    }
}
