using System;
using System.Reflection.Emit;

namespace MyIoC.Creators.Emit
{
    internal class IndependentObjectCreator : IObjectCreator
    {
        private DynamicMethod _instanceCreator;

        public IndependentObjectCreator(Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            ObjectType = objectType;
            GenerateCreateInstanceMethod();
        }

        public Type ObjectType { get; }

        public object Create(Container container)
        {
            return _instanceCreator.Invoke(null, null);
        }

        private void GenerateCreateInstanceMethod()
        {
            var constructor = ObjectType.GetConstructor(Array.Empty<Type>());
            if (constructor == null)
                throw new ContainerException("Export type must contain parameterless constructor.");

            _instanceCreator = new DynamicMethod("CreateInstance", ObjectType, null, true);
            var il = _instanceCreator.GetILGenerator();
            il.Emit(OpCodes.Newobj, constructor);
            il.Emit(OpCodes.Ret);
        }
    }
}
