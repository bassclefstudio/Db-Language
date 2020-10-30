using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Documentation
{
    /// <summary>
    /// Represents a period-delimited path (such as a namespace or version number) of objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects in the path (i.e. <see cref="string"/>, <see cref="int"/>).</typeparam>
    public abstract class Path<T>
    {
        /// <summary>
        /// Represents all of the individual parts of the <see cref="Path{T}"/>, as would usually be separated by a dot ('.').
        /// </summary>
        public T[] PathParts { get; protected set; }

        /// <summary>
        /// Creates an empty <see cref="Path{T}"/>.
        /// </summary>
        protected Path()
        { }

        /// <summary>
        /// Creates a <see cref="Path{T}"/> from a collection of the path sections.
        /// </summary>
        /// <param name="parts">A collection of sections of the <see cref="Path{T}"/>.</param>
        public Path(params T[] parts)
        {
            if (!parts.Any())
            {
                throw new ArgumentException("Path must consist of at least one value.");
            }

            PathParts = parts;
        }

        /// <summary>
        /// Creates a <see cref="Path{T}"/> from a collection of the path sections.
        /// </summary>
        /// <param name="parts">A collection of sections of the <see cref="Path{T}"/>.</param>
        public Path(IEnumerable<T> parts) : this(parts.ToArray())
        { }

        /// <summary>
        /// Creates a <see cref="Path{T}"/> from a base <see cref="Path{T}"/> and a relative path within that <see cref="Path{T}"/>.
        /// </summary>
        /// <param name="basePath">The base <see cref="Path{T}"/>.</param>
        /// <param name="addedParts">A relative path to the type or value within <paramref name="basePath"/>.</param>
        public Path(Path<T> basePath, T[] addedParts)
            : this(basePath.PathParts.Concat(addedParts))
        { }   
    }
}
