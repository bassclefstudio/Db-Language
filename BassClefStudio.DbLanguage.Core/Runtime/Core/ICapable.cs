using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Runtime.Core
{
    /// <summary>
    /// Represents an object that has a set of required <see cref="Capability"/>s in order to be used or executed.
    /// </summary>
    public interface ICapable
    {
        /// <summary>
        /// Retreives the collection of <see cref="Capability"/> objects (required and optional) this <see cref="ICapable"/> has.
        /// </summary>
        CapabilitiesCollection GetCapabilities();
    }
}
