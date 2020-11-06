using BassClefStudio.DbLanguage.Core.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler
{
    /// <summary>
    /// Represents a service that can convert from serialized <typeparamref name="T"/> input to <see cref="IPackage"/>s that can be installed on an <see cref="ISystem"/>.
    /// </summary>
    /// <typeparam name="T">The type of input this <see cref="ICompilePipeline{T}"/> accepts.</typeparam>
    public interface ICompilePipeline<in T>
    {
        /// <summary>
        /// Compiles the <typeparamref name="T"/> input and creates an <see cref="IPackage"/> that can be used with an <see cref="ISystem"/>.
        /// </summary>
        /// <param name="packageInput">The <typeparamref name="T"/> input representing this specific <see cref="IPackage"/>.</param>
        IPackage CompilePackage(T packageInput);
    }

    /// <summary>
    /// Represents a service that can convert from compiled <see cref="IPackage"/>s (installed on an <see cref="ISystem"/>) to serialized <typeparamref name="T"/> output that can be stored or sent.
    /// </summary>
    /// <typeparam name="T">The type of output this <see cref="ISerializePipeline{T}"/> supports.</typeparam>
    public interface ISerializePipeline<out T>
    {
        /// <summary>
        /// Creates a serialized <typeparamref name="T"/> object containing the information needed to build the input <see cref="IPackage"/>.
        /// </summary>
        /// <param name="package">The <see cref="IPackage"/> to serialize.</param>
        T SerializePackage(IPackage package);
    }

    /// <summary>
    /// Represents a service that can act as an <see cref="ICompilePipeline{T}"/> and an <see cref="ISerializePipeline{T}"/> - meaning that <see cref="IPackage"/>s can be both imported and exported as data of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The data type that this <see cref="IPackagePipeline{T}"/> converts <see cref="IPackage"/>s to and from.</typeparam>
    public interface IPackagePipeline<T> : ICompilePipeline<T>, ISerializePipeline<T>
    { }
}
