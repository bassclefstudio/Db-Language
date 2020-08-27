using BassClefStudio.DbLanguage.Core.Documentation;
using BassClefStudio.DbLanguage.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Data
{
    /// <summary>
    /// Represents a concrete type of a <see cref="DataObject"/> in the Db runtime.
    /// </summary>
    public class DataType : IType
    {
        #region TypeDefinition

        /// <inheritdoc/>
        public Namespace TypeName { get; }

        /// <summary>
        /// A collection of <see cref="DataContract"/>s that this <see cref="DataType"/> supports.
        /// </summary>
        public List<DataContract> InheritedContracts { get; }

        /// <summary>
        /// The parent <see cref="DataType"/> of this <see cref="DataType"/>, from which this type inherits all properties and fulfilled <see cref="DataContract"/>s.
        /// </summary>
        public DataType ParentType { get; }

        #endregion
        #region Initialize

        public DataType(Namespace typeName, IEnumerable<MemoryProperty> publicProperties, IEnumerable<MemoryProperty> privateProperties, IEnumerable<DataContract> contracts = null, DataType parentType = null)
        {
            TypeName = typeName;

            InheritedContracts = contracts == null ? new List<DataContract>() : new List<DataContract>(contracts);
            ParentType = parentType;

            InitializeProperties(publicProperties, privateProperties);
        }

        public DataType(Namespace typeName, Type boundType, IEnumerable<MemoryProperty> publicProperties, IEnumerable<MemoryProperty> privateProperties, IEnumerable<DataContract> contracts = null, DataType parentType = null)
            : this(typeName, publicProperties, privateProperties, contracts, parentType)
        {
            BoundType = boundType;
            if (HasTypeBinding)
            {
                boundTypeInfo = BoundType.GetTypeInfo();
            }
        }

        #endregion
        #region Memory

        public List<MemoryProperty> PublicProperties { get; private set; }

        public List<MemoryProperty> PrivateProperties { get; private set; }

        /// <summary>
        /// Initializes the <see cref="PublicProperties"/> and <see cref="PrivateProperties"/> lists from the given new <see cref="MemoryProperty"/> objects as well as parent data. Then checks the property lists against all <see cref="DataContract"/>s and expected conditions.
        /// </summary>
        /// <param name="newPublic">The new public <see cref="MemoryProperty"/> objects to add to the <see cref="DataType"/>.</param>
        /// <param name="newPrivate">The new private <see cref="MemoryProperty"/> objects to add to the <see cref="DataType"/>.</param>
        private void InitializeProperties(IEnumerable<MemoryProperty> newPublic, IEnumerable<MemoryProperty> newPrivate)
        {
            PublicProperties = new List<MemoryProperty>();
            PrivateProperties = new List<MemoryProperty>();

            if(ParentType != null)
            {
                PublicProperties.AddRange(ParentType.PublicProperties);
                PrivateProperties.AddRange(ParentType.PrivateProperties);
            }

            PublicProperties.AddRange(newPublic);
            PrivateProperties.AddRange(newPrivate);

            //// Get any properties that have duplicate paths.
            var allProperties = PublicProperties.Concat(PrivateProperties);
            var duplicates = allProperties.Where(p => allProperties.Count(a => a == p) > 1);
            if (duplicates.Any())
            {
                throw new TypePropertyException($"One or more property keys are used more than once in the same enclosing type {this.TypeName}: {string.Join(",", duplicates.Select(d => d.Key).Distinct())}.");
            }

            //// Get any DataContracts that are missing properties on the type.
            BuildContainedTypes();
            var unfulfilled = ContainedTypes.OfType<DataContract>().Where(c => c.GetProperties().All(p => PublicProperties.Contains(p)));
            if(unfulfilled.Any())
            {
                throw new TypePropertyException($"One or more DataContracts are missing required properties on type {this.TypeName}: {string.Join(",", unfulfilled.Select(u => u.TypeName))}.");
            }
        }

        #endregion
        #region Binding

        /// <summary>
        /// A boolean value indicating whether this <see cref="DataType"/> is bound to a .NET <see cref="Type"/>. For more information, see <see cref="BoundType"/>.
        /// </summary>
        public bool HasTypeBinding => BoundType != null;

        /// <summary>
        /// .NET objects that are binded to a <see cref="DataObject"/> can be type-bound to a <see cref="DataType"/> by using the <see cref="BindedType"/> property. All binded objects in <see cref="DataObject"/>s of this <see cref="DataType"/> must inherit from this .NET <see cref="Type"/>.
        /// </summary>
        public Type BoundType { get; }

        /// <summary>
        /// Stores type information about the <see cref="BoundType"/> for use in reflection (see <see cref="IsBoundType(Type)"/>).
        /// </summary>
        private TypeInfo boundTypeInfo;

        /// <summary>
        /// Returns a boolean indicating whether the provided type <paramref name="t"/> inherits from (or is the same as) the <see cref="BoundType"/> of a bound <see cref="DataType"/>.
        /// </summary>
        /// <param name="t">The .NET type to check for inheritance.</param>
        public bool IsBoundType(Type t)
        {
            return t.GetTypeInfo().IsAssignableFrom(boundTypeInfo);
        }

        #endregion
        #region Inheritance

        private List<IType> ContainedTypes = null;
        private void BuildContainedTypes()
        {
            if (ContainedTypes == null)
            {
                ContainedTypes = new List<IType>();
                ContainedTypes.AddRange(InheritedContracts);
                if (ParentType != null)
                {
                    ParentType.BuildContainedTypes();
                    ContainedTypes.AddRange(ParentType.ContainedTypes);
                }
            }
        }

        /// <inheritdoc/>
        public bool Is(IType other)
        {
            BuildContainedTypes();
            return ContainedTypes.Contains(other);
        }

        #endregion
    }

    public class TypePropertyException : Exception
    {
        public TypePropertyException() { }
        public TypePropertyException(string message) : base(message) { }
        public TypePropertyException(string message, Exception inner) : base(message, inner) { }
    }

    public class TypeBindingException : Exception
    {
        public TypeBindingException() { }
        public TypeBindingException(string message) : base(message) { }
        public TypeBindingException(string message, Exception inner) : base(message, inner) { }
    }
}
