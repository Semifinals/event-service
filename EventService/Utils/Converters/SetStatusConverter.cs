using Semifinals.EventService.Models;

namespace Semifinals.EventService.Utils.Converters;

public class SetStatusConverter : JsonConverter<SetStatus>
{
    public override SetStatus Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
            (SetStatus)reader.GetUInt16();

    public override void Write(
        Utf8JsonWriter writer,
        SetStatus value,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(((int)value).ToString());
}
