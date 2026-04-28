using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

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
    
    // ===== VISUAL (some unused) =====
    static NativeAtlasLoader Loader;
    static Texture2D Texture;
    
    private const float Scale = 0.8f;
    public FrameDefinition Head;
    public FrameDefinition Body;
    public FrameDefinition Tail;
    public FrameDefinition Extra;
    public FrameDefinition Extra2;
    public float rot1, rot2;

    // ===== TUNABLE CONSTANTS =====
    private const float SquashArmTime = 1.5f;

    private const float SquashTriggerRadius = 85f;
    private const float SquashSplashRadius = 95f;

    private const float PotatoMineArmTime = 14f;

    private const float PotatoMineTriggerRadius = 80f;
    private const float PotatoMineSplashRadius = 85f;
    // =============================

    // special constructor to instantiate static atlas
    // MUST be called before all other Plant Class instances
    public Plant(Texture2D tex, NativeAtlasLoader loader)
    {
        Texture = tex;
        Loader = loader;
    }

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
        LoadTextures();
    }

    private void LoadTextures()
    {
        switch (Type)
        {
            case PlantType.Peashooter:
                Head = Loader.Frames["bevo_head"].Clone();
                Tail = Loader.Frames["bevo_tail"].Clone();
                Body = Loader.Frames["bevo_body"].Clone();
                // define offsets & origins (pivot)
                Head.Pivot = new Vector2(Head.SourceRectangle.Width * Head.Pivot.X, Head.SourceRectangle.Height * Head.Pivot.Y);
                Tail.Pivot = new Vector2(Tail.SourceRectangle.Width*Tail.Pivot.X, Tail.SourceRectangle.Height*Tail.Pivot.Y);
                Body.Pivot = new Vector2(Body.SourceRectangle.Width/2, Body.SourceRectangle.Height);

                Body.Offset = new Vector2(0, 80);
                Head.Offset = new Vector2(-10, 15);
                Tail.Offset =  new Vector2(-45, 50);
                break;
            case PlantType.SnowPea:
                Head = Loader.Frames["bevo_head_b"].Clone();
                Tail = Loader.Frames["bevo_tail_b"].Clone();
                Body = Loader.Frames["bevo_body_b"].Clone();
                Extra = Loader.Frames["bevo_scarf"].Clone();
                // define offsets & origins (pivot)
                Head.Pivot = new Vector2(Head.SourceRectangle.Width * Head.Pivot.X, Head.SourceRectangle.Height * Head.Pivot.Y);
                Tail.Pivot = new Vector2(Tail.SourceRectangle.Width*Tail.Pivot.X, Tail.SourceRectangle.Height*Tail.Pivot.Y);
                Body.Pivot = new Vector2(Body.SourceRectangle.Width/2, Body.SourceRectangle.Height);
                Extra.Pivot = new Vector2(Extra.SourceRectangle.Width/2, 0);

                Body.Offset = new Vector2(0, 80);
                Head.Offset = new Vector2(-10, 15);
                Tail.Offset =  new Vector2(-45, 50);
                Extra.Offset = new Vector2(10, 5);
                break;
            case PlantType.Repeater:
                Head = Loader.Frames["bevo_head"].Clone();
                Tail = Loader.Frames["bevo_tail"].Clone();
                Body = Loader.Frames["bevo_body"].Clone();
                Extra = Loader.Frames["bevo_bandana"].Clone();
                // define offsets & origins (pivot)
                Head.Pivot = new Vector2(Head.SourceRectangle.Width * Head.Pivot.X, Head.SourceRectangle.Height * Head.Pivot.Y);
                Tail.Pivot = new Vector2(Tail.SourceRectangle.Width*Tail.Pivot.X, Tail.SourceRectangle.Height*Tail.Pivot.Y);
                Body.Pivot = new Vector2(Body.SourceRectangle.Width/2, Body.SourceRectangle.Height);
                Extra.Pivot = new Vector2(Extra.SourceRectangle.Width/2, 0);

                Body.Offset = new Vector2(0, 80);
                Head.Offset = new Vector2(0, 10);
                Tail.Offset =  new Vector2(75, 50);
                Extra.Offset = new Vector2(0, 0);
                break;
            case PlantType.Walnut:
                Head = Loader.Frames["helmet"].Clone();
                Head.Pivot = new Vector2(Head.SourceRectangle.Width * Head.Pivot.X, Head.SourceRectangle.Height * Head.Pivot.Y);
                Head.Offset = new Vector2(0, 80);
                break;
            case PlantType.Plantern:
                Head = Loader.Frames["light_bevo_head"].Clone();
                Tail = Loader.Frames["light_bevo_tail"].Clone();
                Body = Loader.Frames["light_bevo_body"].Clone();
                Extra = Loader.Frames["light_bevo_arm"].Clone();
                Extra2 = Loader.Frames["light_bevo_light"].Clone();
                // define offsets & origins (pivot)
                Head.Pivot = new Vector2(Head.SourceRectangle.Width * Head.Pivot.X, Head.SourceRectangle.Height * Head.Pivot.Y);
                Tail.Pivot = new Vector2(Tail.SourceRectangle.Width*Tail.Pivot.X, Tail.SourceRectangle.Height*Tail.Pivot.Y);
                Body.Pivot = new Vector2(Body.SourceRectangle.Width/2, Body.SourceRectangle.Height);
                Extra.Pivot = new Vector2(Extra.SourceRectangle.Width*Extra.Pivot.X, Extra.SourceRectangle.Height*Extra.Pivot.Y);
                Extra2.Pivot = new Vector2(Extra2.SourceRectangle.Width*Extra2.Pivot.X, Extra2.SourceRectangle.Height*Extra2.Pivot.Y);
                
                Body.Offset = new Vector2(0, 80);
                Head.Offset = new Vector2(-10, -25);
                Tail.Offset =  new Vector2(-45, 50);
                Extra.Offset = new Vector2(-45, 0);
                Extra2.Offset = new Vector2(-55, -25);
                break;
            case PlantType.PotatoMine:
                Body = Loader.Frames["mine_1"].Clone();
                Extra = Loader.Frames["mine_dirt"].Clone();
                // define offsets & origins (pivot)
                Body.Pivot = new Vector2(Body.SourceRectangle.Width/2, Body.SourceRectangle.Height);
                Extra.Pivot = new Vector2(Extra.SourceRectangle.Width/2, Extra.SourceRectangle.Height);

                Body.Offset = new Vector2(0, 80);
                Extra.Offset = new Vector2(0, 80);
                break;
            case PlantType.Squash:
                Body = Loader.Frames["squash"].Clone();
                // define offsets & origins (pivot)
                Body.Pivot = new Vector2(Body.SourceRectangle.Width/2, Body.SourceRectangle.Height);
                Body.Offset = new Vector2(0, 80);
                break;
        }
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

    public void Draw(SpriteBatch sb)
    {
        // arrange sprite positions w/ null handling
        Vector2 posBody = (Body != null) ? Position + Body.Offset : Position;
        Vector2 posHead = (Head != null) ? Position + Head.Offset : Position;
        Vector2 posTail = (Tail != null) ? Position + Tail.Offset : Position;
        Vector2 posExtra = (Extra != null) ? Position + Extra.Offset : Position;
        Vector2 posExtra2 = (Extra2 != null) ? Position + Extra2.Offset : Position;
        
        switch (Type)
        {
            case PlantType.Peashooter:
                sb.Draw(Texture, posTail, Tail.SourceRectangle, Color.White,
                    rot2, Tail.Pivot, Scale, SpriteEffects.None, 0.0f);
                sb.Draw(Texture, posBody, Body.SourceRectangle, Color.White,
                    0.0f, Body.Pivot, Scale, SpriteEffects.None, 0.0f);
                sb.Draw(Texture, posHead, Head.SourceRectangle, Color.White,
                    rot1, Head.Pivot, Scale, SpriteEffects.None, 0.0f);
                break;
            case PlantType.SnowPea:
                sb.Draw(Texture, posTail, Tail.SourceRectangle, Color.White,
                    rot2, Tail.Pivot, Scale, SpriteEffects.None, 0.0f);
                sb.Draw(Texture, posBody, Body.SourceRectangle, Color.White,
                    0.0f, Body.Pivot, Scale, SpriteEffects.None, 0.0f);
                sb.Draw(Texture, posExtra, Extra.SourceRectangle, Color.White,
                    0.0f, Extra.Pivot, Scale, SpriteEffects.None, 0.0f);
                sb.Draw(Texture, posHead, Head.SourceRectangle, Color.White,
                    rot1, Head.Pivot, Scale, SpriteEffects.None, 0.0f);
                break;
            case PlantType.Repeater:
                sb.Draw(Texture, posTail, Tail.SourceRectangle, Color.White,
                    rot2, Tail.Pivot, Scale, SpriteEffects.FlipHorizontally, 0.0f);
                sb.Draw(Texture, posBody, Body.SourceRectangle, Color.White,
                    0.0f, Body.Pivot, Scale, SpriteEffects.FlipHorizontally, 0.0f);
                sb.Draw(Texture, posExtra, Extra.SourceRectangle, Color.White,
                    0.0f, Extra.Pivot, Scale, SpriteEffects.FlipHorizontally, 0.0f);
                sb.Draw(Texture, posHead, Head.SourceRectangle, Color.White,
                    rot1, Head.Pivot, Scale, SpriteEffects.FlipHorizontally, 0.0f);
                break;
            case PlantType.Walnut:
                if (HealthRemaining > 28f)
                    Body = Loader.Frames["helmet"].Clone();
                else if (HealthRemaining > 14f)
                    Body = Loader.Frames["helmet_2"].Clone();
                else
                    Body = Loader.Frames["helmet_3"].Clone();
                // update pivots and offsets
                Body.Pivot = new Vector2(Body.SourceRectangle.Width / 2, Body.SourceRectangle.Height);
                Body.Offset = new Vector2(0, 80);
                sb.Draw(Texture, posHead, Body.SourceRectangle, Color.White,
                    rot1, Body.Pivot, Scale, SpriteEffects.None, 0.0f);
                break;
            case PlantType.Plantern:
                sb.Draw(Texture, posTail, Tail.SourceRectangle, Color.White,
                    rot2, Tail.Pivot, Scale, SpriteEffects.FlipHorizontally, 0.0f);
                sb.Draw(Texture, posBody, Body.SourceRectangle, Color.White,
                    0.0f, Body.Pivot, Scale, SpriteEffects.FlipHorizontally, 0.0f);
                sb.Draw(Texture, posExtra, Extra.SourceRectangle, Color.White,
                    rot1, Extra.Pivot, Scale, SpriteEffects.FlipHorizontally, 0.0f);
                sb.Draw(Texture, posHead, Head.SourceRectangle, Color.White,
                    rot1, Head.Pivot, Scale, SpriteEffects.FlipHorizontally, 0.0f);
                sb.Draw(Texture, posExtra2, Extra2.SourceRectangle, Color.White,
                    rot1, Extra2.Pivot, Scale, SpriteEffects.FlipHorizontally, 0.0f);
                break;
            case PlantType.PotatoMine:
                if (armTimer < PotatoMineArmTime / 2f)
                    Body = Loader.Frames["mine_1"].Clone();
                else if (armTimer < PotatoMineArmTime)
                    Body = Loader.Frames["mine_2"].Clone();
                else
                    Body = Loader.Frames["mine_3"].Clone();
                // update pivots and offsets
                Body.Pivot = new Vector2(Body.SourceRectangle.Width / 2, Body.SourceRectangle.Height);
                Body.Offset = new Vector2(0, 80);
                sb.Draw(Texture, posBody, Body.SourceRectangle, Color.White,
                    0.0f, Body.Pivot, Scale, SpriteEffects.None, 0.0f);
                sb.Draw(Texture, posExtra, Extra.SourceRectangle, Color.White,
                    0.0f, Extra.Pivot, Scale, SpriteEffects.None, 0.0f);
                break;
            case PlantType.Squash:
                sb.Draw(Texture, posBody, Body.SourceRectangle, Color.White,
                    rot1, Body.Pivot, Scale, SpriteEffects.None, 0.0f);
                break;
        }
    }

    public void Animate(GameTime gameTime)
    {
        double t = gameTime.TotalGameTime.TotalSeconds;
        rot1 = (float)(Math.Sin(t * 2) * (Math.PI / 25) + (2 * Math.PI));
        rot2 = (float)(Math.Cos(t * 2) * (Math.PI / 8) + (2 * Math.PI));
    }

    // legacy code
    /*
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
    */
}