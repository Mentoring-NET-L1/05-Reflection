using System;
using System.Reflection.Emit;
using System.Text;

namespace MyIoC.Creators
{
    internal class IndependentObjectCreator : IObjectCreator
    {
        public IndependentObjectCreator(Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            ObjectType = objectType;
        }

        public Type ObjectType { get; }

        public object Create(Container container)
        {
            return Activator.CreateInstance(ObjectType);
        }
    }
}
