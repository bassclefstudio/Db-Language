using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Documentation
{
    /// <summary>
    /// Represents a full name of a type or value, split into sections. This can include a full namespace and the path inside of that namespace. Provides methods for resolving namespaces.
    /// </summary>
    public class Namespace : Path<string>
    {
        /// <summary>
        /// Creates a <see cref="Namespace"/> from its <see cref="string"/> form.
        /// </summary>
        /// <param name="name">The string form of the full namespace.</param>
        public Namespace(string name)
        {
            PathParts = name.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <inheritdoc/>
        public Namespace(params string[] parts) : base(parts)
        { }

        /// <inheritdoc/>
        public Namespace(IEnumerable<string> parts) : base(parts)
        { }

        /// <summary>
        /// Creates a namespace from a base <see cref="Namespace"/> and a relative path within that <see cref="Namespace"/>.
        /// </summary>
        /// <param name="baseName">The base <see cref="Namespace"/>.</param>
        /// <param name="addedParts">The string form of the relative path to the type or value within <paramref name="baseName"/>.</param>
        public Namespace(Namespace baseName, string addedParts)
            : base(baseName.PathParts.Concat(
                addedParts.Split(
                    new string[] { "." },
                    StringSplitOptions.RemoveEmptyEntries)))
        { }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{{string.Join(".", PathParts)}}}";
        }

        /// <inheritdoc/>
        public static implicit operator Namespace(string s)
        {
            return new Namespace(s);
        }

        /// <inheritdoc/>
        public static implicit operator string(Namespace n)
        {
            return n.ToString();
        }
    }
}
