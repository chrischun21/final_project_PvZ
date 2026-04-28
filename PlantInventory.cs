using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace final_project_PvZ;

public class PlantInventory
{
    private Dictionary<PlantType, int> plants;
    private Texture2D Texture;
    private FrameDefinition Backdrop;
    private FrameDefinition Hand;
    private FrameDefinition Roster;

    public PlantInventory(Texture2D texture, NativeAtlasLoader loader)
    {
        plants = new Dictionary<PlantType, int>();
        Texture = texture; // texture atlas access
        
        Backdrop = loader.Frames["frame"];
        Hand = loader.Frames["hand"];
        Roster = loader.Frames["roster"];
    }

    public void AddPlant(PlantType type)
    {
        if (!plants.ContainsKey(type))
            plants[type] = 0;

        plants[type]++;
    }

    public bool UsePlant(PlantType type)
    {
        if (plants.ContainsKey(type) && plants[type] > 0)
        {
            plants[type]--;
            return true;
        }

        return false;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        int x = 50;
        int y = 20;
        
        spriteBatch.Draw(Texture, new Vector2(20, 10), Backdrop.SourceRectangle, Color.White);
        spriteBatch.Draw(Texture, new Vector2(35, 20), Roster.SourceRectangle, Color.White);
        spriteBatch.Draw(Texture, new Vector2(740, 20), Hand.SourceRectangle, Color.White);
        

        foreach (var kvp in plants)
        {
            string keyNumber = GetKeyNumber(kvp.Key);

            string text = $"{kvp.Key} ({keyNumber}): {kvp.Value}";

            spriteBatch.DrawString(
                font,
                text,
                new Vector2(x, y),
                Color.White
            );
            
            // I need to rework this display to finish this part... hardcoded vals !
            // x += 90;

            y += 30;
        }
    }

    private string GetKeyNumber(PlantType type)
    {
        return type switch
        {
            PlantType.Peashooter => "1",
            PlantType.SnowPea => "2",
            PlantType.Repeater => "3",
            PlantType.Walnut => "4",
            PlantType.Plantern => "5",
            PlantType.PotatoMine => "6",
            PlantType.Squash => "7",
            _ => "?"
        };
    }

    public bool Reset()
    {
        plants.Clear();
        return true;
    }
}