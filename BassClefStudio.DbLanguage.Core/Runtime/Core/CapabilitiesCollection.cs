using System.Collections.Generic;
using System.Linq;

namespace BassClefStudio.DbLanguage.Core.Runtime.Core
{
    /// <summary>
    /// Represents a collection of <see cref="Capability"/> values that either a piece of code is requesting or that a process has permission to access.
    /// </summary>
    public class CapabilitiesCollection
    {
        /// <summary>
        /// An array of <see cref="Capability"/> objects that this <see cref="CapabilitiesCollection"/> must provide.
        /// </summary>
        public IEnumerable<Capability> RequiredCapabilities { get; }

        /// <summary>
        /// An array of <see cref="Capability"/> objects that are not guaranteed by this <see cref="CapabilitiesCollection"/> but that can be optionally requested.
        /// </summary>
        public IEnumerable<Capability> OptionalCapabilities { get; }

        /// <summary>
        /// Creates a new capability collection from the given values.
        /// </summary>
        /// <param name="requiredCapabilities">The collection of required <see cref="Capability"/> values.</param>
        /// <param name="optionalCapabilities">The collection of optional <see cref="Capability"/> values that code can request.</param>
        public CapabilitiesCollection(IEnumerable<Capability> requiredCapabilities, IEnumerable<Capability> optionalCapabilities)
        {
            RequiredCapabilities = requiredCapabilities;
            OptionalCapabilities = optionalCapabilities;
        }

        /// <summary>
        /// Creates a new capability collection with no required or optional <see cref="Capability"/> values.
        /// </summary>
        public CapabilitiesCollection()
        {
            RequiredCapabilities = new Capability[0];
            OptionalCapabilities = new Capability[0];
        }

        /// <summary>
        /// Creates a new <see cref="CapabilitiesCollection"/> which is the joined equivalent of a given collection of <see cref="CapabilitiesCollection"/>s.
        /// </summary>
        /// <param name="collections">The <see cref="CapabilitiesCollection"/>s to join together.</param>
        public CapabilitiesCollection(IEnumerable<CapabilitiesCollection> collections)
        {
            this.RequiredCapabilities = collections
                .SelectMany(c => c.RequiredCapabilities)
                .Distinct()
                .ToArray();

            this.OptionalCapabilities = collections
                .SelectMany(c => c.OptionalCapabilities)
                .Where(c => !this.RequiredCapabilities.Contains(c))
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Checks to see if this <see cref="CapabilitiesCollection"/> contains all of the required <see cref="Capability"/> objects of a given <paramref name="requiredCapabilities"/> <see cref="CapabilitiesCollection"/>.
        /// </summary>
        /// <param name="requiredCapabilities">The required <see cref="Capability"/> objects.</param>
        /// <returns>A <see cref="bool"/> indicating whether this <see cref="CapabilitiesCollection"/> contains all of the required <see cref="Capability"/> objects.</returns>
        public bool CanAccess(CapabilitiesCollection requiredCapabilities)
        {
            return requiredCapabilities.RequiredCapabilities.All(p => this.RequiredCapabilities.Contains(p));
        }
    }
}
