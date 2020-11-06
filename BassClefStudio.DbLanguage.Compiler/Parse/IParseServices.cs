﻿using BassClefStudio.DbLanguage.Core.Lifecycle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Parse
{
    /// <summary>
    /// Represents a service for parsing a Db type as tokenized output (a <see cref="TokenType"/>) which can then be compiled and run.
    /// </summary>
    public interface ITypeParseService
    {
        /// <summary>
        /// Parses the given text input into a tokenized <see cref="TokenType"/>.
        /// </summary>
        /// <param name="type">The <see cref="string"/> input representing the <see cref="TokenType"/>.</param>
        TokenType ParseType(string type);

        /// <summary>
        /// Parses the given text input into a tokenized <see cref="TokenType"/>.
        /// </summary>
        /// <param name="typeReader">The stream of text input representing the <see cref="TokenType"/>.</param>
        TokenType ParseType(TextReader typeReader);
    }

    /// <summary>
    /// Represents a service for parsing a Db type as tokenized output (a <see cref="TokenType"/>) which can then be compiled and run.
    /// </summary>
    public interface IMetadataParseService
    {
        /// <summary>
        /// Parses the given project file to a <see cref="PackageInfo"/> object.
        /// </summary>
        /// <param name="type">The <see cref="string"/> metadata representing the <see cref="PackageInfo"/>.</param>
        PackageInfo ParseProject(string type);

        /// <summary>
        /// Parses the given project file to a <see cref="PackageInfo"/> object.
        /// </summary>
        /// <param name="typeReader">The stream of metadata representing the <see cref="PackageInfo"/>.</param>
        PackageInfo ParseProject(TextReader typeReader);
    }

    /// <summary>
    /// Represents an <see cref="Exception"/> thrown during <see cref="string"/>-to-token parsing.
    /// </summary>
    public class ParseException : Exception
    {
        /// <inheritdoc/>
        internal ParseException() { }
        /// <inheritdoc/>
        internal ParseException(string message) : base(message) { }
        /// <inheritdoc/>
        internal ParseException(string message, Exception inner) : base(message, inner) { }
    }
}