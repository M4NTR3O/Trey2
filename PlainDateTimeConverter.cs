using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Trey2
{
    public sealed class PlainDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            var s = reader.GetString()!;

            var dto = DateTimeOffset.Parse(
                s,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind
            );

            return DateTime.SpecifyKind(dto.DateTime, DateTimeKind.Unspecified);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(
                value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            );
        }
    }
}
