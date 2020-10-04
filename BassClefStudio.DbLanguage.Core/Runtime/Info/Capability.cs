using BassClefStudio.DbLanguage.Core.Runtime.Commands;
using BassClefStudio.DbLanguage.Core.Runtime.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Runtime.Info
{
    /// <summary>
    /// Represents a collection of <see cref="Capability"/> values that grant a <see cref="Thread"/> capability to access <see cref="ICommand"/> commands.
    /// </summary>
    public class CapabilitiesCollection
    {
        /// <summary>
        /// The collection of <see cref="Capability"/> values required for code execution.
        /// </summary>
        public Capability[] RequiredCapabilities { get; }

        /// <summary>
        /// The collection of <see cref="Capability"/> values optional for code execution but that can be used to provide additional functionality.
        /// </summary>
        public Capability[] OptionalCapabilities { get; }

        /// <summary>
        /// Creates a new capability collection from the given values.
        /// </summary>
        /// <param name="requiredCapabilities">The collection of required <see cref="Capability"/> values.</param>
        /// <param name="optionalCapabilities">The collection of optional <see cref="Capability"/> values that the script can request.</param>
        public CapabilitiesCollection(IEnumerable<Capability> requiredCapabilities, IEnumerable<Capability> optionalCapabilities)
        {
            RequiredCapabilities = requiredCapabilities.ToArray();
            OptionalCapabilities = optionalCapabilities.ToArray();
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
        /// Checks to see if a <see cref="Thread"/> with the current <see cref="CapabilitiesCollection"/> has capability to access an <see cref="ICommand"/> with the given <see cref="CapabilitiesCollection"/>.
        /// </summary>
        /// <param name="requiredCapabilities">The required <see cref="Capability"/> objects that an <see cref="ICommand"/> requests from the user.</param>
        /// <returns>A <see cref="bool"/> indicating whether this <see cref="CapabilitiesCollection"/> contains all of the required <see cref="Capability"/>s.</returns>
        public bool CanAccess(CapabilitiesCollection requiredCapabilities)
        {
            return requiredCapabilities.RequiredCapabilities.All(p => this.RequiredCapabilities.Contains(p));
        }
    }

    /// <summary>
    /// A capability, or a capability of the system that an app is allowed to use by the user, that an <see cref="ICommand"/> can request.
    /// </summary>
    public class Capability
    {
        /// <summary>
        /// The name of the <see cref="Capability"/>, used by the Db Language.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The user-friendly name for the <see cref="Capability"/> that is shown to the user when they are asked for consent.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// A description of the <see cref="Capability"/> which is shown to the user when they are asked for consent.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Creates a new <see cref="Capability"/> object from the names and description.
        /// </summary>
        /// <param name="name">The unique name of this <see cref="Capability"/> object.</param>
        /// <param name="displayName">The display name shown to the user.</param>
        /// <param name="description">The description of this <see cref="Capability"/> shown to the user.</param>
        public Capability(string name, string displayName, string description)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
        }
    }

    /// <summary>
    /// Represents an exception thrown when a running <see cref="Thread"/> does not have the required capabilities to run a certain <see cref="ICommand"/>.
    /// </summary>
    public class CapabilityException : Exception
    {
        public CapabilityException() { }
        public CapabilityException(string message) : base(message) { }
        public CapabilityException(string message, Exception inner) : base(message, inner) { }
    }
}
