using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planclap.Client.Infrastructures.Helpers;

public class JsonHelper
{
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public T? Deserialize<T>(string text)
        => JsonSerializer.Deserialize<T>(text, _options);
}
