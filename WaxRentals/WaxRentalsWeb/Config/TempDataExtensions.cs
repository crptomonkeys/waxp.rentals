using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

// https://stackoverflow.com/a/35042391/128217

namespace WaxRentalsWeb.Config
{
    public static class TempDataExtensions
    {

        public static void Put<T>(this ITempDataDictionary @this, string key, T value)
            where T : class
        {
            @this[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary @this, string key)
            where T : class
        {
            return @this.TryGetValue(key, out object value) && value is string
                ? JsonConvert.DeserializeObject<T>((string)value)
                : null;
        }

    }
}
