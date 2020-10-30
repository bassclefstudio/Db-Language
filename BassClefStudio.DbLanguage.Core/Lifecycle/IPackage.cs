using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Documentation;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Lifecycle
{
    /// <summary>
    /// Represents a container of <see cref="IType"/>s and the information describing how these types connect to other <see cref="IPackage"/>s and the system.
    /// </summary>
    public interface IPackage
    {
        /// <summary>
        /// A collection of <see cref="IType"/> objects described in this <see cref="IPackage"/>.
        /// </summary>
        IEnumerable<IType> DefinedTypes { get; }

        /// <summary>
        /// Gets the <see cref="PackageInfo"/> information about this <see cref="IPackage"/>.
        /// </summary>
        PackageInfo Info { get; }
    }

    /// <summary>
    /// Represents the defining information and unique name of an <see cref="IPackage"/>.
    /// </summary>
    public class PackageInfo
    {
        /// <summary>
        /// The full name of the given <see cref="IPackage"/>.
        /// </summary>
        public Namespace Name { get; }

        private string displayName;
        /// <summary>
        /// The display name for the given <see cref="IPackage"/>. Defaults to <see cref="Name"/>.
        /// </summary>
        public string DisplayName { get => displayName ?? Name; private set => displayName = value; }

        /// <summary>
        /// The version number of the <see cref="IPackage"/>, as a <see cref="VersionPath"/>.
        /// </summary>
        public VersionPath PackageVersion { get; }

        /// <summary>
        /// A short description of the <see cref="IPackage"/> and its functionality.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Creates a new <see cref="PackageInfo"/>
        /// </summary>
        /// <param name="name">The full name of the given <see cref="IPackage"/>.</param>
        /// <param name="version">The version number of the <see cref="IPackage"/>, as a <see cref="VersionPath"/>.</param>
        /// <param name="description">A short description of the <see cref="IPackage"/> and its functionality.</param>
        /// <param name="displayName">The display name for the given <see cref="IPackage"/>. Defaults to <paramref name="name"/>.</param>
        public PackageInfo(Namespace name, VersionPath version, string description = null, string displayName = null)
        {
            Name = name;
            PackageVersion = version;
            Description = description;
            DisplayName = displayName;
        }

        /// <summary>
        /// Compares two <see cref="PackageInfo"/> objects, returning true if they refer to the same <see cref="IPackage"/>.
        /// </summary>
        public static bool operator ==(PackageInfo a, PackageInfo b)
        {
            return a.Name == b.Name && a.PackageVersion == b.PackageVersion;
        }

        /// <summary>
        /// Compares two <see cref="PackageInfo"/> objects to see if they refer to the same <see cref="IPackage"/>.
        /// </summary>
        public static bool operator !=(PackageInfo a, PackageInfo b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is PackageInfo info
                && this == info;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    /// <summary>
    /// An extension class for the <see cref="IPackage"/> interface.
    /// </summary>
    public static class PackageExtensions
    {
    }
}
