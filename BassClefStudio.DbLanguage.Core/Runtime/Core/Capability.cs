using BassClefStudio.DbLanguage.Core.Runtime.Commands;
using System;

namespace BassClefStudio.DbLanguage.Core.Runtime.Core
{
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
    /// An <see cref="Exception"/> thrown when a context does not have the required <see cref="Capability"/> to run.
    /// </summary>
    public class CapabilityException : Exception
    {
        /// <inheritdoc/>
        public CapabilityException() { }
        /// <inheritdoc/>
        public CapabilityException(string message) : base(message) { }
        /// <inheritdoc/>
        public CapabilityException(string message, Exception inner) : base(message, inner) { }
    }
}
