using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyIoC.Creators.Emit;
using IObjectCreator = MyIoC.Creators.IObjectCreator;

namespace MyIoC
{
    public class Container
    {
        private readonly Dictionary<Type, IObjectCreator> _creators;

        public Container()
        {
            _creators = new Dictionary<Type, IObjectCreator>();
        }

        public void AddAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            foreach (var type in assembly.DefinedTypes)
            {
                TryAddType(type);
            }
        }

        public void AddType(Type type)
        {
            if (!TryAddType(type))
                throw new ContainerException($"Cann't add type {type.FullName} because it hasn't export or import attributes.");
        }

        public object CreateInstance(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (_creators.TryGetValue(type, out var objectCreator))
            {
                return objectCreator.Create(this);
            }
            else
            {
                throw new ContainerException("Type {type} isn't regesterd in container.");
            }
        }

        public T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        private bool TryAddType(Type type)
        {
            var isImportConstructor = (type.GetCustomAttribute<ImportConstructorAttribute>() != null);
            var isImportProperties = type.GetTypeInfo().DeclaredProperties
                .Any(property => property.GetCustomAttribute<ImportAttribute>() != null);
            var isExport = (type.GetCustomAttribute<ExportAttribute>() != null);

            Type baseType;
            if (isExport)
            {
                var exportAttribute = type.GetCustomAttribute<ExportAttribute>();
                baseType = exportAttribute.Contract ?? type;
            }
            else
            {
                baseType = type;
            }

            if (_creators.ContainsKey(baseType))
                throw new ContainerException($"Type {baseType.FullName} allredy regestered.");

            IObjectCreator creator;
            if (isImportConstructor && isImportProperties)
            {
                throw new ContainerException("Type cann't be imported using constructor and properties.");
            }
            else if (isImportConstructor)
            {
                creator = new ConstructorObjectCreator(type);
            }
            else if (isImportProperties)
            {
                creator = new PropertiesObjectCreator(type);
            }
            else if (isExport)
            {
                creator = new IndependentObjectCreator(type);
            }
            else return false;

            _creators.Add(baseType, creator);
            return true;
        }
    }
}
