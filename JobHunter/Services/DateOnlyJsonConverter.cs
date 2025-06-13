using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JobHunter.Services
{ 
        public class DateOnlyJsonConverter : JsonConverter<DateOnly>
        {
            private readonly string _dateFormat = "yyyy-MM-dd";

            public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return default;

                var dateString = reader.GetString();
                if (string.IsNullOrEmpty(dateString))
                    return default;

                // Try to parse the date string
                if (DateTime.TryParse(dateString, out var date))
                    return DateOnly.FromDateTime(date);

                // Try parsing with specific format
                if (DateTime.TryParseExact(dateString, _dateFormat, null, System.Globalization.DateTimeStyles.None, out date))
                    return DateOnly.FromDateTime(date);

                throw new JsonException($"Unable to parse \"{dateString}\" to DateOnly.");
            }

            public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(_dateFormat));
            }
        }

        public class NullableDateOnlyJsonConverter : JsonConverter<DateOnly?>
        {
            private readonly string _dateFormat = "yyyy-MM-dd";

            public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;

                var dateString = reader.GetString();
                if (string.IsNullOrEmpty(dateString))
                    return null;

                // Try to parse the date string
                if (DateTime.TryParse(dateString, out var date))
                    return DateOnly.FromDateTime(date);

                // Try parsing with specific format
                if (DateTime.TryParseExact(dateString, _dateFormat, null, System.Globalization.DateTimeStyles.None, out date))
                    return DateOnly.FromDateTime(date);

                throw new JsonException($"Unable to parse \"{dateString}\" to DateOnly.");
            }

            public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
            {
                if (value.HasValue)
                    writer.WriteStringValue(value.Value.ToString(_dateFormat));
                else
                    writer.WriteNullValue();
            }
        }
}
