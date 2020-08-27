using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Documentation
{
    /// <summary>
    /// Represents a full name of a type or value, split into sections. This can include a full namespace and the path inside of that namespace. Provides methods for resolving namespaces.
    /// </summary>
    public class Namespace
    {
        /// <summary>
        /// Represents all of the individual parts of the name and namespace, as would usually be separated by a dot ('.').
        /// </summary>
        public string[] NameParts { get; }

        /// <summary>
        /// Creates a namespace from a collection of the name sections.
        /// </summary>
        /// <param name="parts">A collection of sections of the name and namespace.</param>
        public Namespace(params string[] parts)
        {
            NameParts = parts;
        }

        /// <summary>
        /// Creates a namespace from a collection of the name sections.
        /// </summary>
        /// <param name="parts">A collection of sections of the name and namespace.</param>
        public Namespace(IEnumerable<string> parts)
        {
            NameParts = parts.ToArray();
        }

        /// <summary>
        /// Creates a namespace from a constructed namespace.
        /// </summary>
        /// <param name="name">The string form of the full namespace.</param>
        public Namespace(string name)
        {
            NameParts = name.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Creates a namespace from a base <see cref="Namespace"/> and a relative path within that <see cref="Namespace"/>.
        /// </summary>
        /// <param name="baseName">The base <see cref="Namespace"/>.</param>
        /// <param name="addedParts">A relative path to the type or value within <paramref name="baseName"/>.</param>
        public Namespace(Namespace baseName, string[] addedParts)
            : this(baseName.NameParts.Concat(addedParts))
        { }

        /// <summary>
        /// Creates a namespace from a base <see cref="Namespace"/> and a relative path within that <see cref="Namespace"/>.
        /// </summary>
        /// <param name="baseName">The base <see cref="Namespace"/>.</param>
        /// <param name="addedParts">The string form of the relative path to the type or value within <paramref name="baseName"/>.</param>
        public Namespace(Namespace baseName, string addedParts)
            : this(baseName.NameParts.Concat(
                addedParts.Split(
                    new string[] { "." },
                    StringSplitOptions.RemoveEmptyEntries)))
        { }

        public override string ToString()
        {
            return $"{{{string.Join(".", NameParts)}}}";
        }

        public static implicit operator string(Namespace n)
        {
            return n.ToString();
        }

        public static implicit operator Namespace(string s)
        {
            return new Namespace(s);
        }
    }
}
