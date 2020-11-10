using BassClefStudio.DbLanguage.Compiler.Parse;
using BassClefStudio.DbLanguage.Core.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Compiler
{
    /// <summary>
    /// Represents a service that can convert from serialized <typeparamref name="T"/> input to tokenized <see cref="TokenPackage"/>s that can be built or published.
    /// </summary>
    /// <typeparam name="T">The type of input this <see cref="IParsePipeline{T}"/> accepts.</typeparam>
    public interface IParsePipeline<in T>
    {
        /// <summary>
        /// Compiles the <typeparamref name="T"/> input and creates a tokenized <see cref="TokenPackage"/> that can be built or published.
        /// </summary>
        /// <param name="packageInput">The <typeparamref name="T"/> input representing this specific <see cref="IPackage"/>.</param>
        Task<TokenPackage> ParsePackageAsync(T packageInput);
    }

    /// <summary>
    /// Represents a service that can convert from parsed <see cref="TokenPackage"/>s to serialized <typeparamref name="T"/> output that can be stored or sent.
    /// </summary>
    /// <typeparam name="T">The type of output this <see cref="IPublishPipeline{T}"/> supports.</typeparam>
    public interface IPublishPipeline<T>
    {
        /// <summary>
        /// Creates a serialized <typeparamref name="T"/> object containing the information needed to build the input <see cref="IPackage"/>.
        /// </summary>
        /// <param name="package">The <see cref="TokenPackage"/> to serialize.</param>
        Task<T> PublishPackageAsync(TokenPackage package);
    }

    /// <summary>
    /// Represents a service that can build <see cref="TokenPackage"/>s into <see cref="IPackage"/>s that can be installed on an <see cref="ISystem"/>.
    /// </summary>
    public interface IBuildPipeline
    {
        /// <summary>
        /// Builds a <see cref="TokenPackage"/> and creates an <see cref="IPackage"/> that can be installed on an <see cref="ISystem"/>.
        /// </summary>
        /// <param name="packageInput">The <see cref="TokenPackage"/> input representing this specific <see cref="IPackage"/>.</param>
        Task<IPackage> BuildPackageAsync(TokenPackage packageInput);
    }
}
