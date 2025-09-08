using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Infrastructure;

public class UnityReleaseDigitalValueConverter : JsonConverter<UnityReleaseDigitalValue>
{
    public override UnityReleaseDigitalValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // If the API provides something other than an object (e.g., null, or an empty string),
        // treat it as a null UnityReleaseDigitalValue object.
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            reader.Skip(); // Important: advance the reader past the invalid token!
            return null;
        }

        // Initialize with a default for the 'required' Unit property to avoid compile errors.
        var releaseValue = new UnityReleaseDigitalValue { Unit = "BYTE" };

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return releaseValue;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string? propertyName = reader.GetString();
                reader.Read(); // Advance to the property's value

                switch (propertyName)
                {
                    case "value":
                        if (reader.TokenType == JsonTokenType.Number)
                        {
                            releaseValue.Value = reader.GetDouble();
                        }
                        else if (reader.TokenType == JsonTokenType.String)
                        {
                            // Handle cases where the value is a string representation of a number.
                            if (double.TryParse(reader.GetString(), out double val))
                            {
                                releaseValue.Value = val;
                            }
                            // If parsing fails, Value remains null.
                        }
                        // For any other token type (null, object, array), we do nothing,
                        // effectively treating the value as null.
                        break;
                    case "unit":
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            releaseValue.Unit = reader.GetString() ?? "BYTE";
                        }
                        break;
                }
            }
        }
        throw new JsonException("Unexpected end of JSON when reading UnityReleaseDigitalValue.");
    }

    public override void Write(Utf8JsonWriter writer, UnityReleaseDigitalValue value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (value.Value.HasValue)
        {
            writer.WriteNumber("value", value.Value.Value);
        }
        else
        {
            writer.WriteNull("value");
        }
        writer.WriteString("unit", value.Unit);
        writer.WriteEndObject();
    }
}
