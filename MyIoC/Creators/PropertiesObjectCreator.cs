using System;
using System.Linq;
using System.Reflection;

namespace MyIoC.Creators
{
    internal class PropertiesObjectCreator : IObjectCreator
    {
        private readonly PropertyInfo[] _importProperties;

        public PropertiesObjectCreator(Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            var typeInfo = objectType.GetTypeInfo();
            _importProperties = typeInfo.DeclaredProperties
                .Where(property => property.GetCustomAttribute<ImportAttribute>() != null)
                .ToArray();

            foreach (var importProperty in _importProperties)
            {
                if (importProperty.GetSetMethod(false) == null)
                    throw new ContainerException("Import property must have public setter.");
            }

            ObjectType = objectType;
        }

        public Type ObjectType { get; }

        public object Create(Container container)
        {
            var instance = Activator.CreateInstance(ObjectType);
            foreach (var importProperty in _importProperties)
            {
                importProperty.SetValue(instance, container.CreateInstance(importProperty.PropertyType));
            }
            return instance;
        }
    }
}
