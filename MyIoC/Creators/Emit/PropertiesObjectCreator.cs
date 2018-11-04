using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MyIoC.Creators.Emit
{
    internal class PropertiesObjectCreator : IObjectCreator
    {
        private DynamicMethod _instanceCreator;
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
            GenerateCreateInstanceMethod();
        }

        public Type ObjectType { get; }

        public object Create(Container container)
        {
            var propertyValues = _importProperties
                .Select(property => container.CreateInstance(property.PropertyType))
                .ToArray();
            return _instanceCreator.Invoke(null, propertyValues);
        }

        private void GenerateCreateInstanceMethod()
        {
            var constructor = ObjectType.GetConstructor(Array.Empty<Type>());
            if (constructor == null)
                throw new ContainerException("Export type must contain parameterless constructor.");

            var parameterTypes = _importProperties.Select(property => property.PropertyType).ToArray();
            _instanceCreator = new DynamicMethod("CreateInstance", ObjectType, parameterTypes, true);
            var il = _instanceCreator.GetILGenerator();

            il.Emit(OpCodes.Newobj, constructor);
            for (int i = 0; i < _importProperties.Length; i++)
            {
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldarg, i);
                il.Emit(OpCodes.Call, _importProperties[i].GetSetMethod());
            }
            il.Emit(OpCodes.Ret);
        }
    }
}
