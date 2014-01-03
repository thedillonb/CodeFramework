using CodeFramework.Core.Services;
using CodeFramework.Core.Utils;

namespace CodeFramework.Core.Services
{
	public class JsonSerializationService : IJsonSerializationService
    {
		class UnderscoreMappingResolver : Newtonsoft.Json.Serialization.DefaultContractResolver 
		{
			protected override string ResolvePropertyName(string propertyName)
			{
				return propertyName.ToRubyCase();
			}
		}

		private static readonly Newtonsoft.Json.JsonSerializerSettings _settings;

		static JsonSerializationService()
		{
			_settings = new Newtonsoft.Json.JsonSerializerSettings()
			{ 
				ContractResolver = new UnderscoreMappingResolver(),
				NullValueHandling = Newtonsoft.Json.NullValueHandling.Include,
			};
		}

		public T Deserialize<T>(string data)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data, _settings);
		}

		public string Serialize(object data)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(data, _settings);
		}
    }
}

