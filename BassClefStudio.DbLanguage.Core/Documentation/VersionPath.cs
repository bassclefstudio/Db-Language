using BassClefStudio.DbLanguage.Core.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Documentation
{
    /// <summary>
    /// Represents a series of <see cref="int"/> versions (major, minor, etc.) that represent the version of a piece of Db code (such as an <see cref="IPackage"/>.
    /// </summary>
    public class VersionPath : Path<int>, IComparable<VersionPath>
    {
        /// <inheritdoc/>
        public VersionPath(params int[] parts) : base(parts)
        { }

        /// <inheritdoc/>
        public VersionPath(IEnumerable<int> parts) : base(parts)
        { }

        /// <summary>
        /// Creates a <see cref="VersionPath"/> from its <see cref="string"/> equivalent.
        /// </summary>
        /// <param name="name">The <see cref="string"/> form of the version number.</param>
        public VersionPath(string name) : 
            base(name.Split(
                    new string[] { "." }, 
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s)))
        { }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{v{string.Join(".", PathParts)}}}";
        }

        /// <inheritdoc/>
        public static implicit operator VersionPath(string s)
        {
            return new VersionPath(s);
        }

        /// <inheritdoc/>
        public static implicit operator string(VersionPath v)
        {
            return v.ToString();
        }

        #region Operators

        /// <inheritdoc/>
        public int CompareTo(VersionPath other)
        {
            int thisLength = this.PathParts.Length;
            int otherLength = other.PathParts.Length;
            for (int i = 0; i < new int[] { thisLength, otherLength }.Max(); i++)
            {
                if (thisLength < i + 1)
                {
                    return -1;
                }
                else if (otherLength < i + 1)
                {
                    return 1;
                }
                else
                {
                    if (this.PathParts[i] != other.PathParts[i])
                    {
                        return this.PathParts[i].CompareTo(other.PathParts[i]);
                    }
                }
            }

            return 0;
        }

        /// <inheritdoc/>
        public static bool operator >(VersionPath a, VersionPath b)
        {
            return a.CompareTo(b) == 1;
        }

        /// <inheritdoc/>
        public static bool operator <(VersionPath a, VersionPath b)
        {
            return a.CompareTo(b) == -1;
        }

        /// <inheritdoc/>
        public static bool operator <=(VersionPath a, VersionPath b)
        {
            return a < b || a == b;
        }

        /// <inheritdoc/>
        public static bool operator >=(VersionPath a, VersionPath b)
        {
            return a > b || a == b;
        }

        /// <inheritdoc/>
        public static bool operator ==(VersionPath a, VersionPath b)
        {
            return a.CompareTo(b) == 0;
        }

        /// <inheritdoc/>
        public static bool operator !=(VersionPath a, VersionPath b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is VersionPath v
                && this == v;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
