using BassClefStudio.DbLanguage.Compiler.Parse;
using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Documentation;
using BassClefStudio.DbLanguage.Core.Lifecycle;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Commands;
using BassClefStudio.DbLanguage.Core.Runtime.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BassClefStudio.DbLanguage.Compiler.Build
{
    /// <summary>
    /// A service for building, type-checking, compiling, and creating<see cref="IPackage"/>s from <see cref="StringLibrary"/> objects created by a relevant <see cref="IParseService"/>.
    /// </summary>
    internal class DbBuildService
    {
        /// <inheritdoc/>
        public IPackage CreatePackage(StringLibrary stringLib, IEnumerable<IPackage> installedPackages)
        {
            CompiledPackage lib = new CompiledPackage(stringLib.Name);
            IEnumerable<IPackage> dependencies = installedPackages.Where(p => stringLib.Dependencies.Contains((string)p.Name));

            //Create pairs of string/Db types with names.
            IEnumerable<TypeBuilder> pairs = stringLib.DefinedTypes.Select(s => new TypeBuilder(s));
            IDictionary<Namespace, IType> types = pairs.Select(p => p.DbType).Concat(dependencies.SelectMany(d => d.DefinedTypes)).ToDictionary(t => t.TypeName);

            //Iterate through the headers of TypePairs, creating parent/child structure and inheritance.
            foreach (var p in pairs)
            {
                if (p.DbType is DataContract contract)
                {
                    foreach (var d in p.StringType.Header.Dependencies)
                    {
                        if (types.TryGetValue(d, out var dependency))
                        {
                            if (dependency is DataContract dC)
                            {
                                contract.InheritedContracts.Add(dC);
                            }
                            else
                            {
                                throw new BuildException($"A DataContract cannot inherit from a concrete type. (contract: {p.Name}, inheriting: {dependency.TypeName})");
                            }
                        }
                        else
                        {
                            throw new BuildException($"DataContract could not find inherited contract with name {d}. (contract: {p.Name})");
                        }
                    }
                }
                else if (p.DbType is DataType type)
                {
                    foreach (var d in p.StringType.Header.Dependencies)
                    {
                        if (types.TryGetValue(d, out var dependency))
                        {
                            if (dependency is DataContract dC)
                            {
                                type.InheritedContracts.Add(dC);
                            }
                            else if (dependency is DataType dT)
                            {
                                if (type.ParentType == null)
                                {
                                    type.ParentType = dT;
                                }
                                else
                                {
                                    throw new BuildException($"A DataType can only have a single DataType parent. (type: {p.Name}, inheriting: {dependency.TypeName})");
                                }
                            }
                            else
                            {
                                throw new BuildException($"A DataType only inherits from DataTypes or DataContracts. (type: {p.Name}, inheriting: {dependency.TypeName})");
                            }
                        }
                        else
                        {
                            throw new BuildException($"DataType could not find inherited contract or parent type with name {d}. (type: {p.Name})");
                        }
                    }
                }
            }

            foreach (var p in pairs)
            {
                foreach (var prop in p.StringType.Properties.OfType<StringProperty>())
                {
                    if (types.TryGetValue(prop.Type, out var type))
                    {
                        MemoryProperty property = new MemoryProperty(prop.Name, type);
                        if (prop.IsPublic)
                        {
                            p.PublicProperties.Add(property);
                        }
                        else
                        {
                            p.PrivateProperties.Add(property);
                        }
                    }
                    else
                    {
                        throw new BuildException($"Could not find type {prop.Type} for property {p.Name}.{prop.Name}.");
                    }
                }

                foreach (var script in p.StringType.Properties.OfType<StringScript>())
                {
                    if (p.DbType is DataType dataType)
                    {
                        if (types.TryGetValue(script.ReturnType, out var type))
                        {
                            IEnumerable<ScriptInput> inputs = script.Inputs.Select(i => new ScriptInput(i.Name, types[i.Type]));
                            ScriptInfo scriptInfo = new ScriptInfo(script.Name, type, inputs.ToArray());
                        }
                        else
                        {
                            throw new BuildException($"Could not find return type {script.ReturnType} for script {p.Name}.{script.Name}.");
                        }
                    }
                    else
                    {
                        throw new BuildException($"Only DataTypes may contain script blocks. (type: {p.Name})");
                    }
                }
            }

            lib.DefinedTypes = pairs.Select(p => p.DbType);
            return lib;
        }
    }

    internal class TypeBuilder
    {
        public IType DbType { get; }
        public StringType StringType { get; }

        public Namespace Name => DbType?.TypeName;

        public List<MemoryProperty> PublicProperties { get; }
        public List<MemoryProperty> PrivateProperties { get; }

        public TypeBuilder(IType Db, StringType stringType)
        {
            DbType = Db;
            StringType = stringType;
            PublicProperties = new List<MemoryProperty>();
            PrivateProperties = new List<MemoryProperty>();
        }

        public TypeBuilder(StringType stringType) : this(stringType.Header.IsConcrete ? new DataType(stringType.Header.Name) as IType : new DataContract(stringType.Header.Name) as IType, stringType)
        { }

        public void BuildProperties()
        {
            if(DbType is DataType type)
            {
                type.PublicProperties.AddRange(this.PublicProperties);
                type.PrivateProperties.AddRange(this.PrivateProperties);
            }
            else if(DbType is DataContract contract)
            {
                if(this.PrivateProperties.Any())
                {
                    throw new BuildException($"DataContract {Name} cannot have any private properties associated with it.");
                }

                contract.ContractProperties.AddRange(this.PublicProperties);
            }
            else
            {
                throw new BuildException($"IType {Name} was not of a known type, or null.");
            }
        }
    }

    /// <summary>
    /// An <see cref="Exception"/> thrown when building of an <see cref="IPackage"/> from tokenized input fails.
    /// </summary>
    public class BuildException : Exception
    {
        /// <inheritdoc/>
        public BuildException() { }
        /// <inheritdoc/>
        public BuildException(string message) : base(message) { }
        /// <inheritdoc/>
        public BuildException(string message, Exception inner) : base(message, inner) { }
    }
}
