using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Extensions;

public static class ObjectExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = 
        new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    
    public static string ToJson(this object value)
    {
        var json = JsonSerializer.Serialize(value, JsonSerializerOptions);
        return json;
    }
}