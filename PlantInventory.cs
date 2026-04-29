using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace final_project_PvZ;

public class PlantInventory
{
    private const float Scale = 0.9f;
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
        List<FrameDefinition> rosterBox = new List<FrameDefinition>();

        int displayX = 35;
        Rectangle rect = new Rectangle(257, 462, 91, 105);

        rosterBox.Add(new FrameDefinition
        {
            SourceRectangle = rect,
            Size = new Point(rect.Width, rect.Height),
            Offset = new Vector2(displayX, 22)
        });

        displayX += 81;
        rect.X += rosterBox[0].SourceRectangle.Width;
        rect.Width = 100;
        for (int i = 1; i < 7; i++)
        {
            rosterBox.Add(new FrameDefinition
            {
                SourceRectangle = rect,
                Size = new Point(rect.Width, rect.Height),
                Offset = new Vector2(displayX, 22)
            });
            displayX += 91;
            rect.X += rosterBox[i].SourceRectangle.Width;
        }

        spriteBatch.Draw(Texture, new Vector2(20, 10), Backdrop.SourceRectangle, Color.White,
            0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
        spriteBatch.Draw(Texture, new Vector2(668, 20), Hand.SourceRectangle, Color.White,
            0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0.0f);

        
        foreach (PlantType type in PlantType.GetValuesAsUnderlyingType<PlantType>())
        {
            int i = (int)type;
            Color tint = (plants.ContainsKey(type) && plants[type] > 0) ? Color.White : Color.Gray;
            spriteBatch.Draw(Texture, rosterBox[i].Offset, rosterBox[i].SourceRectangle, tint,
                0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, $"({i+1})", rosterBox[i].Offset, Color.White);
            
            // inventory count
            string text = (plants.ContainsKey(type)) ? plants[type].ToString() : "";
            spriteBatch.DrawString(font, text,
                new Vector2(rosterBox[i].Offset.X + rosterBox[i].SourceRectangle.Width/2,
                    rosterBox[i].Offset.Y + 90), Color.White);
        }
        
        // flat inventory list display
        /*
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
        }
        */
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