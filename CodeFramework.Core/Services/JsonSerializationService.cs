using CodeFramework.Core.Services;

namespace CodeFramework.Core.Services
{
	public class JsonSerializationService : IJsonSerializationService
    {
		static JsonSerializationService()
		{
			ServiceStack.Text.JsConfig.EmitLowercaseUnderscoreNames = true;
			ServiceStack.Text.JsConfig.PropertyConvention = ServiceStack.Text.JsonPropertyConvention.Lenient;
			ServiceStack.Text.JsConfig.ConvertObjectTypesIntoStringDictionary = true;
			ServiceStack.Text.JsConfig.IncludeNullValues = true;
		}

		public T Deserialize<T>(string data)
		{
			return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(data);
		}

		public string Serialize(object data)
		{
			return ServiceStack.Text.JsonSerializer.SerializeToString(data);
		}
    }
}

