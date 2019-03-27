using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace MassTransit.JiraServicedeskConnector
{
    sealed class PrefixContractResolver : DefaultContractResolver
    {
        readonly DefaultContractResolver _contractResolver;

        public PrefixContractResolver(DefaultContractResolver contractResolver) => _contractResolver = contractResolver;

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);

            var prefixAttribute = member.GetCustomAttribute<PrefixAttribute>();

            if(prefixAttribute != null)
                jsonProperty.PropertyName = $"{prefixAttribute.Prefix}{jsonProperty.PropertyName}";

            return jsonProperty;
        }

        protected override string ResolvePropertyName(string propertyName) => _contractResolver.GetResolvedPropertyName(propertyName);
    }
}
