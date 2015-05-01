using System;
using System.Reflection;
using System.Windows;

namespace MPDisplay.Common.Controls.Core
{
    public sealed class StaticResourceKey : ResourceKey
    {
        private string _key;
        public string Key { get { return _key; } }

        private Type _type;
        public Type Type { get { return _type; } }

        public StaticResourceKey(Type type, string key)
        {
            _type = type;
            _key = key;
        }

        public override Assembly Assembly
        {
            get { return _type.Assembly; }
        }
    }
}
