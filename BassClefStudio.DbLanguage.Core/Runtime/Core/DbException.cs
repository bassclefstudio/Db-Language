using BassClefStudio.DbLanguage.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Runtime.Core
{
    /// <summary>
    /// Represents an <see cref="Exception"/> thrown from within the Db language.
    /// </summary>
    public class DbException : Exception
    {
        /// <summary>
        /// The Db <see cref="DataObject"/> provided as the exception information object.
        /// </summary>
        public DataObject ExceptionObject { get; }

        /// <inheritdoc/>
        public DbException(string message, DataObject exceptionObject = null) : base(message) => ExceptionObject = exceptionObject;
        /// <inheritdoc/>
        public DbException(string message, Exception innerException, DataObject exceptionObject = null) : base(message, innerException) => ExceptionObject = exceptionObject;
    }
}
