using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace final_project_PvZ;

public class Plant
{
    public Vector2 Position;
    public PlantType Type;

    public float MaxHealth;
    public float HealthRemaining;

    public bool IsAlive = true;

    private float shootTimer = 0f;
    public float armTimer = 0f;

    // ===== TUNABLE CONSTANTS =====
    private const float SquashArmTime = 1.5f;

    private const float SquashTriggerRadius = 85f;
    private const float SquashSplashRadius = 95f;

    private const float PotatoMineArmTime = 14f;

    private const float PotatoMineTriggerRadius = 80f;
    private const float PotatoMineSplashRadius = 85f;
    // =============================

    public Plant(Vector2 pos, PlantType type)
    {
        Position = pos;
        Type = type;

        switch (type)
        {
            case PlantType.Walnut:
                MaxHealth = 42f;
                break;

            default:
                MaxHealth = 3f;
                break;
        }

        HealthRemaining = MaxHealth;
    }

    public void TakeEatingDamage(float amount)
    {
        // Armed potato mine cannot be eaten
        if (Type == PlantType.PotatoMine && armTimer >= PotatoMineArmTime)
            return;

        // Squash invincible while arming
        if (Type == PlantType.Squash && armTimer < SquashArmTime)
            return;

        HealthRemaining -= amount;

        if (HealthRemaining <= 0)
            IsAlive = false;
    }
    
    public void TakeInstantKill()
    {
        HealthRemaining = 0;
        IsAlive = false;
    }

    public List<Projectile> Update(float dt, List<Zombie> zombies)
    {
        List<Projectile> spawned = new();

        shootTimer += dt;

        if (Type == PlantType.PotatoMine || Type == PlantType.Squash)
            armTimer += dt;

        //-----------------------------------
        // PEASHOOTER
        //-----------------------------------
        if (Type == PlantType.Peashooter)
        {
            if (CanShoot(zombies, true) && shootTimer >= 1.2f)
            {
                shootTimer = 0f;

                spawned.Add(new Projectile(
                    Position + new Vector2(0, 40),
                    1f,
                    false
                ));
            }
        }

        //-----------------------------------
        // SNOW PEA
        //-----------------------------------
        if (Type == PlantType.SnowPea)
        {
            if (CanShoot(zombies, true) && shootTimer >= 1.2f)
            {
                shootTimer = 0f;

                spawned.Add(new Projectile(
                    Position + new Vector2(0, 40),
                    1f,
                    true
                ));
            }
        }

        //-----------------------------------
        // REPEATER
        //-----------------------------------
        if (Type == PlantType.Repeater)
        {
            if (CanShoot(zombies, false) && shootTimer >= 0.6f)
            {
                shootTimer = 0f;

                spawned.Add(new Projectile(
                    Position + new Vector2(0, 40),
                    -1f,
                    false
                ));
            }
        }

        //-----------------------------------
        // POTATO MINE
        //-----------------------------------
        if (Type == PlantType.PotatoMine && armTimer >= PotatoMineArmTime)
        {
            foreach (var z in zombies)
            {
                bool sameLane = Math.Abs(z.Position.Y - Position.Y) < 40f;
                bool close = Math.Abs(z.Position.X - Position.X) < PotatoMineTriggerRadius;

                if (sameLane && close)
                {
                    foreach (var other in zombies)
                    {
                        bool nearby =
                            Math.Abs(other.Position.X - Position.X) < PotatoMineSplashRadius &&
                            Math.Abs(other.Position.Y - Position.Y) < 40f;

                        if (nearby)
                            other.TakeDamage(85);
                    }

                    IsAlive = false;
                    break;
                }
            }
        }

        //-----------------------------------
        // SQUASH
        //-----------------------------------
        if (Type == PlantType.Squash && armTimer >= SquashArmTime)
        {
            foreach (var z in zombies)
            {
                bool sameLane = Math.Abs(z.Position.Y - Position.Y) < 40f;
                bool close = Math.Abs(z.Position.X - Position.X) < SquashTriggerRadius;

                if (sameLane && close)
                {
                    foreach (var other in zombies)
                    {
                        bool nearby =
                            Math.Abs(other.Position.X - Position.X) < SquashSplashRadius &&
                            Math.Abs(other.Position.Y - Position.Y) < 50f;

                        if (nearby)
                            other.TakeDamage(85);
                    }

                    IsAlive = false;
                    break;
                }
            }
        }

        return spawned;
    }

    private bool CanShoot(List<Zombie> zombies, bool shootRight)
    {
        foreach (var z in zombies)
        {
            bool sameRow = Math.Abs(z.Position.Y - Position.Y) < 40f;

            bool inFront = shootRight
                ? z.Position.X > Position.X
                : z.Position.X < Position.X;

            if (sameRow && inFront)
                return true;
        }

        return false;
    }

    public void Draw(
        SpriteBatch spriteBatch,
        Texture2D peashooterTex,
        Texture2D snowTex,
        Texture2D repeaterTex,
        Texture2D helmet1,
        Texture2D helmet2,
        Texture2D helmet3,
        Texture2D planternTex,
        Texture2D mine1,
        Texture2D mine2,
        Texture2D mine3,
        Texture2D squashTex)
    {
        Texture2D tex = peashooterTex;
        SpriteEffects effects = SpriteEffects.None;

        switch (Type)
        {
            case PlantType.Peashooter:
                tex = peashooterTex;
                break;

            case PlantType.SnowPea:
                tex = snowTex;
                break;

            case PlantType.Repeater:
                tex = repeaterTex;
                effects = SpriteEffects.FlipHorizontally;
                break;

            case PlantType.Walnut:
                if (HealthRemaining > 28f)
                    tex = helmet1;
                else if (HealthRemaining > 14f)
                    tex = helmet2;
                else
                    tex = helmet3;
                break;

            case PlantType.Plantern:
                tex = planternTex;
                break;

            case PlantType.PotatoMine:
                if (armTimer < PotatoMineArmTime / 2f)
                    tex = mine1;
                else if (armTimer < PotatoMineArmTime)
                    tex = mine2;
                else
                    tex = mine3;
                break;

            case PlantType.Squash:
                tex = squashTex;
                break;
        }

        Vector2 drawPos = Position + new Vector2(0, 80f);

        spriteBatch.Draw(
            tex,
            drawPos,
            null,
            Color.White,
            0f,
            new Vector2(tex.Width / 2f, tex.Height),
            0.8f,
            effects,
            0f
        );
    }
}