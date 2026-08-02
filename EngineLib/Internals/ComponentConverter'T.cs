
using System.Reflection;

namespace PvZCards.Engine;

internal class ComponentConverter<T> : JsonConverter<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode node = JsonNode.Parse(ref reader) ?? throw new InvalidOperationException();
        Type type = typeToConvert;

        JsonNode? typeNode = node["$type"];
        if (typeNode is not null)
        {
            string typeName = typeNode.GetValue<string>();
            type = Type.GetType(typeName) ?? throw new InvalidDataException($"Type '{typeName}' name not found.\n{node}");
        }

        object instance = Activator.CreateInstance(type) ?? throw new InvalidOperationException();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonProperty == null) continue;

            JsonNode? current = typeNode is not null ? node["$data"] : node;
            if (current == null) throw new InvalidOperationException($"'$data' node not found.\n{node}");

            else Console.WriteLine(current);
            foreach (var path in jsonProperty.Path)
            {
                current = current[path];
                if (current == null)
                {
                    if (jsonProperty.IsRequired) {
                        string fp = string.Join(".", jsonProperty.Path);
                        throw new InvalidOperationException($"Property '{Type.FullName}.{fp}' is required.\n{node}");
                    }
                    else break;
                }
            }

            object? value = JsonSerializer.Deserialize(current, property.PropertyType, options);
            property.SetValue(instance, value);
        }

        return (T)instance;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonObject data = [];
        Type? type = value?.GetType();
        if (type == null) return;

        IEnumerable<PropertyInfo>? properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        if (properties == null) return;

        properties = properties.OrderBy(i => i.GetMethod?.GetBaseDefinition() != i.GetMethod);
        foreach (var property in properties)
        {
            var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonProperty == null) continue;

            object? propertyValue = property.GetValue(value);
            if (jsonProperty.IgnoredIfNull && propertyValue == null) continue;

            string[] paths = jsonProperty.Path;
            JsonObject current = data;

            for (int i = 0; i < paths.Length - 1; i++)
            {
                string path = paths[i];
                if (current[path] is JsonObject inner)
                {
                    current = inner["$data"] is JsonObject innerData ? innerData : inner;
                }
                else
                {
                    inner = [];
                    current.Add(path, inner);

                    if (jsonProperty.Annotations.ElementAtOrDefault(i) is Type innerType)
                    {
                        JsonObject innerData = [];
                        inner["$type"] = innerType.AssemblyQualifiedName;
                        inner["$data"] = innerData;
                        current = innerData;
                    }
                    else
                    {
                        current = inner;
                    }
                }
            }

            current[paths[^1]] = JsonSerializer.SerializeToNode(propertyValue, property.PropertyType, options);
        }

        var noType = type.GetCustomAttribute<NoTypeAnnotationAttribute>();
        if (noType is not null)
        {
            data.WriteTo(writer, options);
            return;
        }

        var jObject = new JsonObject
        {
            ["$type"] = type.AssemblyQualifiedName,
            ["$data"] = data
        };

        jObject.WriteTo(writer, options);
    }
}
