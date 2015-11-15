using System;
using System.Reflection;
using System.Windows;

namespace MPDisplay.Common.Controls.Core
{
    public sealed class StaticResourceKey : ResourceKey
    {
        public string Key { get; private set; }

        public Type Type { get; }

        public StaticResourceKey(Type type, string key)
        {
            Type = type;
            Key = key;
        }

        public override Assembly Assembly => Type.Assembly;
    }
}
