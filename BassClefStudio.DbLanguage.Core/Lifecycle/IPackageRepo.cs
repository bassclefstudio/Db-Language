using BassClefStudio.DbLanguage.Core.Documentation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Lifecycle
{
    /// <summary>
    /// Represents a store of <see cref="IPackage"/>s in a system, folder, or online repository.
    /// </summary>
    public interface IPackageRepo
    {
        /// <summary>
        /// Gets a collection of all <see cref="PackageInfo"/> objects for every <see cref="IPackage"/> available in the <see cref="IPackageRepo"/>.
        /// </summary>
        Task<IEnumerable<PackageInfo>> GetPackageListingsAsync();

        /// <summary>
        /// Gets a collection of all <see cref="PackageInfo"/> objects for <see cref="IPackage"/>s available in the <see cref="IPackageRepo"/> that match the given <see cref="Namespace"/>.
        /// </summary>
        /// <param name="packageName">The name of the <see cref="IPackage"/>s to returen <see cref="PackageInfo"/> for.</param>
        Task<IEnumerable<PackageInfo>> GetPackageListingsAsync(Namespace packageName);

        /// <summary>
        /// Gets and deserializes the <see cref="IPackage"/> corresponding to the given <see cref="PackageInfo"/>.
        /// </summary>
        /// <param name="info">The <see cref="PackageInfo"/> reference to the desired <see cref="IPackage"/>.</param>
        Task<IPackage> GetPackageAsync(PackageInfo info);
    }
}
