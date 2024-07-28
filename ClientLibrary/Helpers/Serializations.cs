
using System.Collections;
using System.Text.Json;

namespace ClientLibrary.Helpers
{
    public static class Serializations
    {
        public static string SerializeObj<T>(T modelObject) => JsonSerializer.Serialize(modelObject, typeof(T));
        public static T DeserializeJsonString<T>(string jsonStr) => JsonSerializer.Deserialize<T>(jsonStr)!;
        public static IList<T> DeserializeJsonStringList<T>(string jsoStr) => JsonSerializer.Deserialize<IList<T>>(jsoStr)!;
    }
}
