using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace final_project_PvZ;

public class NativeAtlasLoader
{
    public Dictionary<string, FrameDefinition> Frames { get; private set; } = new();

    public void Load(string jsonContent)
    {
        using JsonDocument doc = JsonDocument.Parse(jsonContent);
        JsonElement root = doc.RootElement;

        JsonElement textures = root.GetProperty("textures");
        JsonElement firstTexture = textures[0];
        JsonElement framesCategory = firstTexture.GetProperty("frames");

        foreach (JsonProperty category in framesCategory.EnumerateObject())
        {
            if (category.Value.ValueKind == JsonValueKind.Array)
            {
                // handle arrays
                foreach (JsonElement item in category.Value.EnumerateArray())
                {
                    foreach (JsonProperty sprite in item.EnumerateObject())
                    {
                        Frames[sprite.Name] = ParseFrameData(sprite.Value);
                    }
                }
            }
            else if (category.Value.ValueKind == JsonValueKind.Object)
            {
                // handle single objects
                Frames[category.Name] = ParseFrameData(category.Value);
            }
        }
    }

    private FrameDefinition ParseFrameData(JsonElement element)
    {
        JsonElement frame = element.GetProperty("frame");
        
        var def = new FrameDefinition
        {
            SourceRectangle = new Rectangle(
                frame.GetProperty("x").GetInt32(),
                frame.GetProperty("y").GetInt32(),
                frame.GetProperty("w").GetInt32(),
                frame.GetProperty("h").GetInt32()
            ),
            Pivot = Vector2.Zero,
            Size = Point.Zero
        };

        // check for pivots
        if (element.TryGetProperty("pivot", out JsonElement pivot))
        {
            def.Pivot = new Vector2(
                (float)pivot.GetProperty("x").GetDouble(),
                (float)pivot.GetProperty("y").GetDouble()
            );
        }

        return def;
    }
}