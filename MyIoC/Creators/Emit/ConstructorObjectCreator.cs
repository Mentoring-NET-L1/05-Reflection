using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MyIoC.Creators.Emit
{
    internal class ConstructorObjectCreator : IObjectCreator
    {
        private DynamicMethod _instanceCreator;
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

            GenerateCreateInstanceMethod(constructors[0]);
        }

        public Type ObjectType { get; }

        public object Create(Container container)
        {
            var parameters = _parameterTypes.Select(type => container.CreateInstance(type)).ToArray();
            return _instanceCreator.Invoke(null, parameters);
        }

        private void GenerateCreateInstanceMethod(ConstructorInfo constructor)
        {
            _instanceCreator = new DynamicMethod("CreateInstance", ObjectType, _parameterTypes, true);
            var il = _instanceCreator.GetILGenerator();
            for (int i = 0; i < _parameterTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg, i);
            }
            il.Emit(OpCodes.Newobj, constructor);
            il.Emit(OpCodes.Ret);
        }
    }
}
