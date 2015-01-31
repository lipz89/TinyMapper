﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using TinyMappers.Mappers;

namespace TinyMappers.Reflection
{
    internal class DynamicAssemblyBuilder
    {
        internal const string AssemblyName = "DynamicTinyMapper";
        private const string AssemblyNameFileName = AssemblyName + ".dll";
        private static readonly DynamicAssembly _dynamicAssembly = new DynamicAssembly();

        public static IDynamicAssembly Get()
        {
            return _dynamicAssembly;
        }


        private sealed class DynamicAssembly : IDynamicAssembly
        {
            private readonly AssemblyBuilder _assemblyBuilder;
            private readonly object _locker = new object();
            private readonly ModuleBuilder _moduleBuilder;
            private readonly TargetMapperBuilder _targetMapperBuilder;

            public DynamicAssembly()
            {
                var assemblyName = new AssemblyName(AssemblyName);
                _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);

                _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName.Name, AssemblyNameFileName, false);
                _targetMapperBuilder = new TargetMapperBuilder(this);
            }

            public TypeBuilder DefineType(string typeName, Type parentType)
            {
                lock (_locker)
                {
                    return _moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Sealed, parentType);
                }
            }

            public TargetMapperBuilder GetTypeBuilder()
            {
                return _targetMapperBuilder;
            }

            public void Save()
            {
                lock (_locker)
                {
                    _assemblyBuilder.Save(AssemblyNameFileName);
                }
            }
        }
    }
}
