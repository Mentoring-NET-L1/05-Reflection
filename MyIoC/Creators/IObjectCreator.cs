using System;

namespace MyIoC.Creators
{
    internal interface IObjectCreator
    {
        Type ObjectType { get; }

        object Create(Container container);
    }
}
