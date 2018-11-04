using System;
using System.Linq;
using System.Reflection;

namespace MyIoC.Creators
{
    internal class ConstructorObjectCreator : IObjectCreator
    {
        private readonly Type[] _parameterTypes;

        public ConstructorObjectCreator(Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            if (objectType.GetCustomAttribute<ImportConstructorAttribute>() == null)
                throw new ContainerException("Generic type must have ImportConstructorAttribute.");

            var constructors = objectType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length != 1)
                throw new ContainerException("Type that injected using construtor must have only one public cunstructor.");

            ObjectType = objectType;
            _parameterTypes = constructors[0].GetParameters()
                .Select(parameter => parameter.ParameterType)
                .ToArray();
        }

        public Type ObjectType { get; }

        public object Create(Container container)
        {
            var parameters = _parameterTypes.Select(type => container.CreateInstance(type)).ToArray();
            return Activator.CreateInstance(ObjectType, parameters);
        }
    }
}
