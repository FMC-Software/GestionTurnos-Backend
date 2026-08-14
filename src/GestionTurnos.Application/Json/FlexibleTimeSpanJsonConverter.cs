using System.Text.Json;
using System.Text.Json.Serialization;

namespace GestionTurnos.Application.Json
{
    public class FlexibleTimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new JsonException("El valor de tiempo no puede estar vacío.");
            }

            if (TimeSpan.TryParse(value, out var timeSpan))
            {
                return timeSpan;
            }

            if (DateTime.TryParse(value, out var dateTime))
            {
                return dateTime.TimeOfDay;
            }

            throw new JsonException($"No se pudo convertir \"{value}\" a un horario válido.");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(@"hh\:mm\:ss"));
        }
    }
}
