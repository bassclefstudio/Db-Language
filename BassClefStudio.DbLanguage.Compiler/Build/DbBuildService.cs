using BassClefStudio.DbLanguage.Compiler.Parse;
using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Documentation;
using BassClefStudio.DbLanguage.Core.Lifecycle;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Info;
using BassClefStudio.DbLanguage.Core.Runtime.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Build
{
    /// <summary>
    /// An <see cref="IBuildService"/> for building, type-checking, compiling, and creating <see cref="ILibrary"/> instances from <see cref="StringLibrary"/> objects created by a relevant <see cref="IParseService"/>.
    /// </summary>
    internal class DbBuildService : IBuildService
    {
        /// <inheritdoc/>
        public ILibrary Build(StringLibrary stringLib, IEnumerable<ILibrary> dependencies)
        {
            IEnumerable<ILibrary> usedDependencies = dependencies
                .Where(l => stringLib.Dependencies.Contains((string)l.Name));

            CompilerLibrary lib = new CompilerLibrary();

            lib.DependentLibraries = usedDependencies;
            lib.Name = stringLib.Name;

            //Create pairs of string/Db types with names.
            IEnumerable<TypeBuilder> pairs = stringLib.DefinedTypes.Select(s => new TypeBuilder(s));
            IDictionary<Namespace, IType> types = pairs.Select(p => p.DbType).Concat(usedDependencies.SelectMany(d => d.Definitions)).ToDictionary(t => t.TypeName);

            //Iterate through the headers of TypePairs, creating parent/child structure and inheritance.
            foreach (var p in pairs)
            {
                if (p.DbType is DataContract contract)
                {
                    foreach (var d in p.StringType.Header.Dependencies)
                    {
                        if(types.TryGetValue(d, out var dependency))
                        {
                            if(dependency is DataContract dC)
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
                                if(type.ParentType == null)
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
                foreach (var child in p.StringType.Properties)
                {
                    if (child is StringProperty prop)
                    {
                        if (types.TryGetValue(prop.Type, out var type))
                        {
                            MemoryProperty property = new MemoryProperty(prop.Name, type);
                            if(prop.IsPublic)
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
                            throw new BuildException($"Could not find type {prop.Type} for property {p.Name}.{child.Name}.");
                        }
                    }
                    else if (child is StringScript stringScript)
                    {
                        if (p.DbType is DataType dataType)
                        {
                            if (types.TryGetValue(stringScript.ReturnType, out var type))
                            {
                                //IEnumerable<ScriptInput> inputs = stringScript.Inputs.Select(i => new ScriptInput(i.Name, types[i.Type]));
                                //ScriptInfo scriptInfo = new ScriptInfo(stringScript.Name, type, inputs.ToArray());

                                //Func<DataObject, Script> scriptBuilder =
                                //    o => new Script(o, scriptInfo, BuildCommands(stringScript.Commands));

                                //Func<DataObject, DataObject> createScriptObject =
                                //    me =>
                                //    {
                                //        var o = new DataObject(types["Core.Script"] as DataType);
                                //        o.TrySetObject<Script>(scriptBuilder(me));
                                //        return o;
                                //    };

                                //dataType.Constructors.Add(new SetCommand(stringScript.Name, new ValueCommand(createScriptObject)));
                            }
                            else
                            {
                                throw new BuildException($"Could not find return type {stringScript.ReturnType} for script {p.Name}.{child.Name}.");
                            }
                        }
                        else
                        {
                            throw new BuildException($"Only DataTypes may contain script blocks. (type: {p.Name})");
                        }
                    }
                    else
                    {
                        throw new BuildException($"Property {p.Name}.{child.Name} is of unknown type or is null.");
                    }
                }
            }

            lib.Definitions = pairs.Select(p => p.DbType);
            return lib;
        }

        //private IEnumerable<ICommand> BuildCommands(IEnumerable<ICodeStatement> statements)
        //{
        //    throw new NotImplementedException();
        //}
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
    /// Represents an <see cref="Exception"/> thrown when building of an <see cref="ILibrary"/> from tokenized input fails.
    /// </summary>
    public class BuildException : Exception
    {
        public BuildException() { }
        public BuildException(string message) : base(message) { }
        public BuildException(string message, Exception inner) : base(message, inner) { }
        protected BuildException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
