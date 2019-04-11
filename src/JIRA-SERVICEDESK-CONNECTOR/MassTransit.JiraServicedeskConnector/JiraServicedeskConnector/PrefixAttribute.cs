using System;

namespace MassTransit.JiraServicedeskConnector
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PrefixAttribute : Attribute
    {
        public string Prefix { get; }

        public PrefixAttribute(string prefix) => Prefix = prefix;
    }
}
