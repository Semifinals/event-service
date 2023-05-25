namespace Semifinals.EventService.Utils.Converters;

public class UnixEpochConverter : JsonConverter<DateTime>
{
    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
            DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64()).UtcDateTime;

    public override void Write(
        Utf8JsonWriter writer,
        DateTime value,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(((DateTimeOffset)value).ToUnixTimeSeconds().ToString());
}
