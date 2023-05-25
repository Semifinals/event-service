namespace Semifinals.EventService.Utils.Converters;

public class ListConverter<T, TConverter> : JsonConverter<IList<T>>
    where TConverter : JsonConverter<T>, new()
{
    private readonly JsonConverter<T> _converter = new TConverter();
    
    public override IList<T>? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            reader.Skip();
            return null;
        }

        List<T> list = new();

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            var item = _converter.Read(ref reader, typeof(T), options);
            if (item != null)
                list.Add(item);
        }

        return list;
    }

    public override void Write(
        Utf8JsonWriter writer,
        IList<T> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (T item in value)
            _converter.Write(writer, item, options);

        writer.WriteEndArray();
    }
}