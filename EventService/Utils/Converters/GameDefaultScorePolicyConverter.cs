using Semifinals.EventService.Models;

namespace Semifinals.EventService.Utils.Converters;

public class GameDefaultScorePolicyConverter : JsonConverter<GameDefaultScorePolicy>
{
    public override GameDefaultScorePolicy Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
            (GameDefaultScorePolicy)reader.GetUInt16();

    public override void Write(
        Utf8JsonWriter writer,
        GameDefaultScorePolicy value,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(((int)value).ToString());
}
