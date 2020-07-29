using BassClefStudio.DbLanguage.Core.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Data
{
    /// <summary>
    /// Represents an instance of an object in the Db runtime.
    /// </summary>
    public class DataObject : IMemoryGroup
    {
        #region TypeInformation

        /// <summary>
        /// The <see cref="DataType"/> that describes the <see cref="Memory.MemoryProperty"/> objects, <see cref="DataContract"/>s, and inherited <see cref="Data.DataType"/>s of the instantiated object.
        /// </summary>
        public DataType DataType { get; }

        #endregion
        #region Constructor

        /// <summary>
        /// Creates a new instance of a <see cref="DataObject"/> of a given <see cref="DataType"/>.
        /// </summary>
        /// <param name="type">The <see cref="Data.DataType"/> of the object.</param>
        public DataObject(DataType type)
        {
            DataType = type;
            BuildMemory();
        }

        #endregion
        #region Memory

        /// <summary>
        /// Represents an <see cref="IWritableMemoryGroup"/> containing the public <see cref="MemoryProperty"/> objects and their set value for the <see cref="DataType.PublicProperties"/>.
        /// </summary>
        public IMemoryGroup PublicProperties { get; private set; }

        /// <summary>
        /// Represents an <see cref="IWritableMemoryGroup"/> containing the public <see cref="MemoryProperty"/> objects and their set value for the <see cref="DataType.PrivateProperties"/>.
        /// </summary>
        public IMemoryGroup PrivateProperties { get; private set; }

        private IMemoryStack memoryStack;
        /// <summary>
        /// Gets the current <see cref="IMemoryStack"/> representing all objects available to the current <see cref="DataObject"/>, including <see cref="PublicProperties"/> and <see cref="PrivateProperties"/>.
        /// </summary>
        public IMemoryStack MemoryStack
        {
            get
            {
                if(memoryStack == null)
                {
                    memoryStack = new MemoryStack();
                    //// TODO: Add some sort of context here.
                    // memoryStack.Push(context);
                    memoryStack.Push(PublicProperties);
                    memoryStack.Push(PrivateProperties);
                }
                return memoryStack;
            }
        }

        /// <summary>
        /// Builds the <see cref="IMemoryGroup"/>s for private and public properties as specified in the <see cref="DataType"/>.
        /// </summary>
        public void BuildMemory()
        {
            PublicProperties = new MemoryGroup(DataType.PublicProperties);
            PrivateProperties = new MemoryGroup(DataType.PrivateProperties);

            //// TODO: Run the constructor to initialize property values...
        }

        /// <inheritdoc/>
        public MemoryItem Get(string key)
            => PublicProperties.Get(key);

        /// <inheritdoc/>
        public bool Set(string key, DataObject value)
            => PublicProperties.Set(key, value);

        /// <inheritdoc/>
        public bool ContainsKey(string key)
            => PublicProperties.ContainsKey(key);

        /// <inheritdoc/>
        public string[] GetKeys()
            => PublicProperties.GetKeys();

        #endregion
        #region Binding

        /// <summary>
        /// Represents this <see cref="DataObject"/>'s bound .NET object. For more information, see <seealso cref="DataType.BoundType"/>.
        /// </summary>
        private object BoundObject;

        /// <summary>
        /// Attempts to retrieve this <see cref="DataObject"/>'s bound .NET object as a specific type.
        /// </summary>
        /// <typeparam name="T">The type of the object to return. This method fails if this is not equal to the type <see cref="DataType.BoundType"/> of the <see cref="DataObject"/>'s <see cref="DataType"/> property.</typeparam>
        /// <returns>The strongly-typed bound .NET object.</returns>
        public T GetObject<T>()
        {
            if (DataType.HasTypeBinding)
            {
                if (typeof(T) == DataType.BoundType)
                {
                    if (BoundObject != null)
                    {
                        return (T)BoundObject;
                    }
                    else
                    {
                        return default(T);
                    }
                }
                else
                {
                    throw new TypeBindingException($"Attempted to retrieve a value from a bound DataObject of a type ({typeof(T).Name}) not equal to the declared type ({DataType.BoundType.Name})");
                }
            }
            else
            {
                throw new TypeBindingException("Attempted to get the bound value of a DataObject whose type does not support .NET type binding.");
            }
        }

        /// <summary>
        /// Sets the bound .NET object of a <see cref="DataObject"/> to a specific value. For more information, see <seealso cref="DataType.BoundType"/>.
        /// </summary>
        /// <typeparam name="T">The .NET type of the <paramref name="value"/> being set.</typeparam>
        /// <param name="value">The <see cref="object"/> to set.</param>
        public void TrySetObject<T>(T value)
        {
            if (DataType.HasTypeBinding)
            {
                if (DataType.IsBoundType(typeof(T)))
                {
                    BoundObject = value;
                }
                else
                {
                    throw new TypeBindingException($"Attempted to set a value to a bound DataObject of a type ({typeof(T).Name}) that is not of type ({DataType.BoundType.Name})");
                }
            }
            else
            {
                throw new TypeBindingException("Attempted to set the bound value of a DataObject whose type does not support .NET type binding.");
            }
        }

        #endregion
    }
}
