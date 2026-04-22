namespace final_project_PvZ;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;


public enum VaseType
{
    Normal,
    Plant,
    Zombie
}
public class Vases
{
    private List<Vase> vases;

    // X positions (9 columns)
    private int[] xPositions = { 270, 383, 494, 595, 702, 809, 912, 1024, 1145 };

    // Y positions (5 rows)
    private int[] yPositions = { 200, 342, 484, 615, 746 };
    


    public Vases()
    {
        vases = new List<Vase>();
    }

    // Spawn N columns from the RIGHT
    public void Spawn(int numColumns, int plantCount, int zombieCount)
    {
        vases.Clear();

        List<Vector2> positions = new List<Vector2>();

        numColumns = MathHelper.Clamp(numColumns, 1, 9);

        // 1. Collect all positions (same as before)
        for (int c = 0; c < numColumns; c++)
        {
            int colIndex = 8 - c;

            for (int row = 0; row < 5; row++)
            {
                positions.Add(new Vector2(
                    xPositions[colIndex],
                    yPositions[row]
                ));
            }
        }

        int total = positions.Count;

        if (plantCount + zombieCount > total)
            throw new Exception("Too many special vases requested.");

        // 2. Create type pool
        List<VaseType> types = new List<VaseType>();

        for (int i = 0; i < plantCount; i++)
            types.Add(VaseType.Plant);

        for (int i = 0; i < zombieCount; i++)
            types.Add(VaseType.Zombie);

        while (types.Count < total)
            types.Add(VaseType.Normal);

        // 3. Shuffle types (Fisher-Yates)
        Random rand = new Random();

        for (int i = types.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            (types[i], types[j]) = (types[j], types[i]);
        }

        // 4. Assign types to positions
        for (int i = 0; i < total; i++)
        {
            vases.Add(new Vase(positions[i], types[i]));
        }
    }

    public void Draw(
        SpriteBatch spriteBatch,
        Texture2D normal,
        Texture2D normalCracked,
        Texture2D plant,
        Texture2D plantCracked,
        Texture2D zombie,
        Texture2D zombieCracked,
        float scale)
    {
        foreach (var vase in vases)
        {
            if (vase.IsBroken) continue;

            Texture2D texture = null;

            if (!vase.IsBreaking)
            {
                texture = vase.Type switch
                {
                    VaseType.Plant => plant,
                    VaseType.Zombie => zombie,
                    _ => normal
                };
            }
            else
            {
                texture = vase.Type switch
                {
                    VaseType.Plant => plantCracked,
                    VaseType.Zombie => zombieCracked,
                    _ => normalCracked
                };
            }
            
            float yOffset = 0f;

            switch (vase.Type)
            {
                case VaseType.Plant:
                    yOffset = -5f;   // tweak this
                    break;

                case VaseType.Zombie:
                    yOffset = 5f;  // tweak this
                    break;
            }
            
            Vector2 drawPos = vase.Position + new Vector2(0, yOffset);

            spriteBatch.Draw(
                texture,
                drawPos,
                null,
                Color.White,
                0f,
                new Vector2(texture.Width / 2f, texture.Height / 2f),
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
    public void HandleClick(Vector2 mousePosition, Texture2D vaseTexture, float scale)
    {
        float width = vaseTexture.Width * scale;
        float height = vaseTexture.Height * scale;

        foreach (var vase in vases)
        {
            if (vase.IsBroken || vase.IsBreaking) continue;

            Rectangle hitbox = new Rectangle(
                (int)(vase.Position.X - width / 2f),
                (int)(vase.Position.Y - height / 2f),
                (int)width,
                (int)height
            );

            if (hitbox.Contains(mousePosition))
            {
                vase.IsBreaking = true;
                vase.BreakTimer = 0f;
                break;
            }
        }
    }
    
    public void Update(float deltaTime)
    {
        foreach (var vase in vases)
        {
            if (vase.IsBreaking && !vase.IsBroken)
            {
                vase.BreakTimer += deltaTime;

                if (vase.BreakTimer >= 0.15f) // 0.15 sec crack animation
                {
                    vase.IsBroken = true;
                }
            }
        }
    }
}