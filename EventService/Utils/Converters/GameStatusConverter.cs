using Semifinals.EventService.Models;

namespace Semifinals.EventService.Utils.Converters;

public class GameStatusConverter : JsonConverter<GameStatus>
{
    public override GameStatus Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
            (GameStatus)reader.GetUInt16();

    public override void Write(
        Utf8JsonWriter writer,
        GameStatus value,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(((int)value).ToString());
}
