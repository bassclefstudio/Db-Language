using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Lifecycle
{
    /// <summary>
    /// Represents a system which can install and manage <see cref="IPackage"/>s as well as executing code on them.
    /// </summary>
    public interface ISystem : IPackageRepo
    {
        /// <summary>
        /// Adds an <see cref="IPackage"/> to the <see cref="ISystem"/>, installing it and registering it with other components.
        /// </summary>
        /// <param name="package">The <see cref="IPackage"/> to install.</param>
        Task InstallPackageAsync(IPackage package);

        /// <summary>
        /// Removes the <see cref="IPackage"/> specified by the given <see cref="PackageInfo"/> from the <see cref="ISystem"/>.
        /// </summary>
        /// <param name="info">The <see cref="PackageInfo"/> specifying the <see cref="IPackage"/> to remove.</param>
        Task RemovePackageAsync(PackageInfo info);

        /// <summary>
        /// A read-only dynamic collection of the running <see cref="SystemProcess"/>es containing the relevant <see cref="PackageInfo"/>s and their <see cref="ProcessStatus"/>.
        /// </summary>
        ReadOnlyObservableCollection<SystemProcess> RunningProcesses { get; }
    }
}
