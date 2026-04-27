using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace final_project_PvZ;

public class PlantInventory
{
    private Dictionary<PlantType, int> plants;

    public PlantInventory()
    {
        plants = new Dictionary<PlantType, int>();
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
}