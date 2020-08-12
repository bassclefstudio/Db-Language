using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Lifecycle
{
    /// <summary>
    /// Represents a group of associated <see cref="IType"/> definitions, <see cref="Scripts.Script"/>s, and related information, and provides a basis of type info and static <see cref="DataObject"/>s to running <see cref="Scripts.Commands.ICommand"/>s.
    /// </summary>
    public interface ILibrary
    {
        IEnumerable<IType> Definitions { get; }

        IMemoryGroup ManagedContext { get; }
    }
}
