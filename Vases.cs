using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace final_project_PvZ;

public class Vases
{
    private List<Vase> vases;

    private int[] xPositions = { 270, 383, 494, 595, 702, 809, 912, 1024, 1145 };
    private int[] yPositions = { 200, 342, 484, 615, 746 };

    public Action<PlantType> OnPlantCollected;
    public Action<Vector2, ZombieType> OnZombieSpawned;

    private const float PlanternVisionRadius = 200f;

    private Random rand = new();

    public Vases()
    {
        vases = new List<Vase>();
    }

    public void SpawnLevel(Difficulty difficulty)
    {
        vases.Clear();

        List<Vase> contents = new();

        int shownPlantHints = 0;
        int shownZombieHints = 0;

        switch (difficulty)
        {
            case Difficulty.Easy:
                shownPlantHints = 9;
                shownZombieHints = 9;

                AddZombie(contents, ZombieType.Normal, 8);
                AddZombie(contents, ZombieType.Buckethead, 3);

                AddPlant(contents, PlantType.SnowPea, 2);
                AddPlant(contents, PlantType.Peashooter, 2);
                AddPlant(contents, PlantType.Repeater, 5);
                AddPlant(contents, PlantType.Walnut, 1);
                AddPlant(contents, PlantType.Squash, 3);
                AddPlant(contents, PlantType.PotatoMine, 1);
                break;

            case Difficulty.Medium:
                shownPlantHints = 6;
                shownZombieHints = 6;

                AddZombie(contents, ZombieType.Normal, 10);
                AddZombie(contents, ZombieType.Buckethead, 3);

                AddPlant(contents, PlantType.SnowPea, 2);
                AddPlant(contents, PlantType.Peashooter, 4);
                AddPlant(contents, PlantType.Repeater, 6);
                AddPlant(contents, PlantType.Walnut, 1);
                AddPlant(contents, PlantType.Squash, 3);
                AddPlant(contents, PlantType.PotatoMine, 1);
                break;

            case Difficulty.Hard:
                shownPlantHints = 3;
                shownZombieHints = 3;

                AddZombie(contents, ZombieType.Normal, 11);
                AddZombie(contents, ZombieType.Buckethead, 4);
                AddZombie(contents, ZombieType.Gargantuar, 1);

                AddPlant(contents, PlantType.PotatoMine, 1);
                AddPlant(contents, PlantType.Plantern, 1);
                AddPlant(contents, PlantType.SnowPea, 2);
                AddPlant(contents, PlantType.Repeater, 7);
                AddPlant(contents, PlantType.Squash, 4);
                AddPlant(contents, PlantType.Walnut, 1);
                AddPlant(contents, PlantType.Peashooter, 3);
                break;
        }

        Shuffle(contents);

        AssignVisibleHints(contents, shownPlantHints, shownZombieHints);

        PlaceOnBoard(contents);
    }

    private void AddPlant(List<Vase> list, PlantType type, int count)
    {
        for (int i = 0; i < count; i++)
            list.Add(new Vase(Vector2.Zero, storedPlant: type));
    }

    private void AddZombie(List<Vase> list, ZombieType type, int count)
    {
        for (int i = 0; i < count; i++)
            list.Add(new Vase(Vector2.Zero, storedZombie: type));
    }

    private void AssignVisibleHints(List<Vase> contents, int plantHints, int zombieHints)
    {
        var plantVases = contents
            .Where(v => v.ContainsPlant)
            .OrderBy(_ => rand.Next())
            .ToList();

        var zombieVases = contents
            .Where(v => v.ContainsZombie)
            .OrderBy(_ => rand.Next())
            .ToList();

        for (int i = 0; i < Math.Min(plantHints, plantVases.Count); i++)
            plantVases[i].Appearance = VaseAppearance.PlantHint;

        for (int i = 0; i < Math.Min(zombieHints, zombieVases.Count); i++)
            zombieVases[i].Appearance = VaseAppearance.ZombieHint;
    }

    private void PlaceOnBoard(List<Vase> contents)
    {
        int total = contents.Count;
        int columnsNeeded = (int)Math.Ceiling(total / 5f);

        int index = 0;

        for (int c = 0; c < columnsNeeded; c++)
        {
            int colIndex = 8 - c;

            for (int row = 0; row < 5; row++)
            {
                if (index >= total)
                    return;

                contents[index].Position = new Vector2(
                    xPositions[colIndex],
                    yPositions[row]);

                vases.Add(contents[index]);

                index++;
            }
        }
    }

    private void Shuffle(List<Vase> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
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

        Texture2D peashooterTex,
        Texture2D snowTex,
        Texture2D repeaterTex,
        Texture2D walnutTex,
        Texture2D planternTex,
        Texture2D mineTex,
        Texture2D squashTex,

        Texture2D zombieTex,
        Texture2D bucketTex,
        Texture2D gargTex,

        float scale)
    {
        foreach (var vase in vases)
        {
            if (vase.IsBroken)
                continue;

            VaseAppearance displayAppearance = vase.Appearance;

            if (vase.RevealedByPlantern)
            {
                if (vase.ContainsPlant)
                    displayAppearance = VaseAppearance.PlantHint;
                else if (vase.ContainsZombie)
                    displayAppearance = VaseAppearance.ZombieHint;
            }

            Texture2D texture;

            if (!vase.IsBreaking)
            {
                texture = displayAppearance switch
                {
                    VaseAppearance.PlantHint => plant,
                    VaseAppearance.ZombieHint => zombie,
                    _ => normal
                };
            }
            else
            {
                texture = displayAppearance switch
                {
                    VaseAppearance.PlantHint => plantCracked,
                    VaseAppearance.ZombieHint => zombieCracked,
                    _ => normalCracked
                };
            }

            float yOffset = displayAppearance switch
            {
                VaseAppearance.PlantHint => -5f,
                VaseAppearance.ZombieHint => 5f,
                _ => 0f
            };

            Vector2 vaseDrawPos = vase.Position + new Vector2(0, yOffset);

            spriteBatch.Draw(
                texture,
                vaseDrawPos,
                null,
                Color.White,
                0f,
                new Vector2(texture.Width / 2f, texture.Height / 2f),
                scale,
                SpriteEffects.None,
                0f
            );

            if (vase.RevealedByPlantern)
            {
                Texture2D icon = null;
                SpriteEffects effects = SpriteEffects.None;

                if (vase.StoredPlant.HasValue)
                {
                    switch (vase.StoredPlant.Value)
                    {
                        case PlantType.Peashooter: icon = peashooterTex; break;
                        case PlantType.SnowPea: icon = snowTex; break;
                        case PlantType.Repeater:
                            icon = repeaterTex;
                            effects = SpriteEffects.FlipHorizontally;
                            break;
                        case PlantType.Walnut: icon = walnutTex; break;
                        case PlantType.Plantern: icon = planternTex; break;
                        case PlantType.PotatoMine: icon = mineTex; break;
                        case PlantType.Squash: icon = squashTex; break;
                    }
                }
                else if (vase.StoredZombie.HasValue)
                {
                    switch (vase.StoredZombie.Value)
                    {
                        case ZombieType.Normal: icon = zombieTex; break;
                        case ZombieType.Buckethead: icon = bucketTex; break;
                        case ZombieType.Gargantuar: icon = gargTex; break;
                    }
                }

                if (icon != null)
                {
                    spriteBatch.Draw(
                        icon,
                        vaseDrawPos + new Vector2(0, -10),
                        null,
                        Color.White,
                        0f,
                        new Vector2(icon.Width / 2f, icon.Height / 2f),
                        0.4f,
                        effects,
                        0f
                    );
                }
            }
        }
    }

    public bool HandleClick(Vector2 mousePosition, Texture2D vaseTexture, float scale)
    {
        float width = vaseTexture.Width * scale;
        float height = vaseTexture.Height * scale;

        foreach (var vase in vases)
        {
            if (vase.IsBroken || vase.IsBreaking)
                continue;

            Rectangle hitbox = new(
                (int)(vase.Position.X - width / 2f),
                (int)(vase.Position.Y - height / 2f),
                (int)width,
                (int)height);

            if (hitbox.Contains(mousePosition))
            {
                vase.IsBreaking = true;
                vase.BreakTimer = 0f;
                return true;
            }
        }

        return false;
    }

    public void Update(float dt)
    {
        foreach (var vase in vases)
        {
            if (!vase.IsBreaking || vase.IsBroken)
                continue;

            vase.BreakTimer += dt;

            if (vase.BreakTimer >= 0.15f)
            {
                vase.IsBroken = true;

                if (vase.StoredPlant.HasValue)
                    OnPlantCollected?.Invoke(vase.StoredPlant.Value);

                if (vase.StoredZombie.HasValue)
                    OnZombieSpawned?.Invoke(vase.Position, vase.StoredZombie.Value);
            }
        }
    }

    public void UpdatePlanternReveal(List<Plant> plants)
    {
        foreach (var vase in vases)
        {
            vase.RevealedByPlantern = false;

            foreach (var plant in plants)
            {
                if (plant.Type != PlantType.Plantern)
                    continue;

                if (Vector2.Distance(plant.Position, vase.Position) <= PlanternVisionRadius)
                {
                    vase.RevealedByPlantern = true;
                    break;
                }
            }
        }
    }

    public bool AllDestroyed()
    {
        return vases.All(v => v.IsBroken);
    }
}