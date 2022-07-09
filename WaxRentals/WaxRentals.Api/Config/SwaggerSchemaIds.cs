using System.Reflection;
using WaxRentals.Api.Entities;

#nullable disable

namespace WaxRentals.Api.Config
{
    public static class SwaggerSchemaIds
    {

        private static Dictionary<Assembly, Dictionary<Type, string>> CACHE = new();

        public static string Generate(Type type)
        {
            if (!CACHE.ContainsKey(type.Assembly))
            {
                CACHE[type.Assembly] = Load(type.Assembly);
            }
            return CACHE[type.Assembly][type];
        }

        private static Dictionary<Type, string> Load(Assembly assembly)
        {
            var types = assembly.ExportedTypes.GroupBy(type => type.Name);

            // Start with the simple types.
            var result = new Dictionary<Type, string>(
                types.Where(group => group.Count() == 1)
                     .Select(group => group.Single())
                     .Where(type => !type.IsGenericType)
                     .Select(type => KeyValuePair.Create(type, type.Name))
            );

            // Add types with duplicate names.
            result = result.Concat(
                types.Where(group => group.Count() > 1)
                     .SelectMany(group => group)
                     .Where(type => !type.IsGenericType)
                     .ToDictionary(type => type, type => QualifiedTypeName(type))
            ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Add generics - List.
            result = result.Concat(
                result.ToDictionary(
                    kvp => typeof(IEnumerable<>).MakeGenericType(kvp.Key),
                    kvp => $"List[{kvp.Value}]")
            ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Add generics - Result.
            result = result.Concat(
                result.ToDictionary(
                    kvp => typeof(Result<>).MakeGenericType(kvp.Key),
                    kvp => $"Result[{kvp.Value}]")
            ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return result;
        }

        private static string QualifiedTypeName(Type type)
        {
            var ns = type.Namespace;
            ns = ns[(ns.LastIndexOf('.') + 1)..];
            return $"{ns}_{type.Name}";
        }

    }
}
