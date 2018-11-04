using System;

namespace MyIoC
{
    [AttributeUsage(AttributeTargets.Property)]

    public class ImportAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ImportConstructorAttribute : Attribute
    {
    }
}
