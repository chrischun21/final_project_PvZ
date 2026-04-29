using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace final_project_PvZ;

public class Zombie
{
    public Vector2 Position;

    public int Health;
    public float Speed;

    public bool IsEating = false;

    public ZombieType Type;

    private static Random rand = new Random();

    private float slowTimer = 0f;
    private float slowMultiplier = 1f;
    
    // ===== VISUAL (some unused) =====
    static NativeAtlasLoader Loader;
    static Texture2D Texture;
    
    public float Scale;
    public FrameDefinition Head;
    public FrameDefinition Torso;
    public FrameDefinition ArmL;
    public FrameDefinition ArmR;
    public FrameDefinition Hand;
    public FrameDefinition LegL;
    public FrameDefinition LegR;
    public FrameDefinition Extra;
    public float rot1, rot2,rot3;

    // GARGANTUAR SMASH
    private const float SmashDuration = 1.2f;
    public bool IsPreparingSmash => IsSmashing;

    public bool IsSmashing = false;
    private float smashTimer = 0f;
    private Plant smashTarget = null;
    
    // special constructor to instantiate static atlas
    // MUST be called before all other Zombie Class instances
    public Zombie(Texture2D tex, NativeAtlasLoader loader)
    {
        Texture = tex;
        Loader = loader;
    }

    public Zombie(Vector2 position, ZombieType type)
    {
        Position = position;
        Type = type;

        switch (type)
        {
            case ZombieType.Normal:
                Health = rand.Next(10, 16);
                Speed = rand.Next(25, 36);
                Scale = 1f;
                break;

            case ZombieType.Buckethead:
                Health = rand.Next(50, 66);
                Speed = rand.Next(25, 36);
                Scale = 1f;
                break;

            case ZombieType.Gargantuar:
                Health = 150;
                Speed = rand.Next(16, 27);
                Scale = 0.75f;
                break;
        }

        LoadTextures();
    }

    private void LoadTextures()
    {
        switch (Type)
        {
            case ZombieType.Gargantuar:
                Head = Loader.Frames["zomb_g_head"].Clone();
                Torso = Loader.Frames["zomb_g_torso"].Clone();
                ArmL = Loader.Frames["zomb_g_arm"].Clone(); 
                Hand = Loader.Frames["zomb_g_hand"].Clone();
                LegL = Loader.Frames["zomb_g_leg_l"].Clone();
                LegR = Loader.Frames["zomb_g_leg_r"].Clone();
                Extra = Loader.Frames["zomb_g_sign"].Clone();
                
                Scale = 0.75f;
                
                // define offsets & origins (pivot)
                Hand.Pivot = new Vector2(Hand.SourceRectangle.Width * Hand.Pivot.X, Hand.SourceRectangle.Height * Hand.Pivot.Y);

                Head.Offset = new Vector2(-5, 0);
                Torso.Offset = new Vector2(5, -60);
                ArmL.Offset = new Vector2(25, 0);
                Hand.Offset = new Vector2(0, 80);
                LegL.Offset = new Vector2(30, 50);
                LegR.Offset = new Vector2(35, 50);
                Extra.Offset = new Vector2(-90, 20);
                break;
            default:
                // zombie + buckethead
                Head = Loader.Frames["zomb_head"].Clone();
                Torso = Loader.Frames["zomb_torso"].Clone();
                ArmL = Loader.Frames["zomb_arm_l"].Clone(); 
                ArmR = Loader.Frames["zomb_arm_r"].Clone();
                LegL = Loader.Frames["zomb_leg_l"].Clone();
                LegR = Loader.Frames["zomb_leg_r"].Clone();
                Extra = Loader.Frames["zomb_bucket"].Clone();

                Scale = 1f;
        
                ArmR.Pivot = new Vector2(ArmR.SourceRectangle.Width * ArmR.Pivot.X, ArmR.SourceRectangle.Height * ArmR.Pivot.Y);

                Head.Offset = new Vector2(-5, 0);
                Torso.Offset = new Vector2(5, -50);
                ArmL.Offset = new Vector2(10, 0);
                ArmR.Offset = new Vector2(-5, 0);
                LegL.Offset = new Vector2(10, 50);
                LegR.Offset = new Vector2(10, 50);
                Extra.Offset = new Vector2(-5, -20);
                break;
        }
        // define offsets & origins (pivot)
        Head.Pivot = new Vector2(Head.SourceRectangle.Width * Head.Pivot.X, Head.SourceRectangle.Height * Head.Pivot.Y);
        Torso.Pivot = new Vector2(Torso.SourceRectangle.Width * Torso.Pivot.X, Torso.SourceRectangle.Height * Torso.Pivot.Y);
        ArmL.Pivot = new Vector2(ArmL.SourceRectangle.Width * ArmL.Pivot.X, ArmL.SourceRectangle.Height * ArmL.Pivot.Y);
        LegL.Pivot = new Vector2(LegL.SourceRectangle.Width * LegL.Pivot.X, LegL.SourceRectangle.Height * LegL.Pivot.Y);
        LegR.Pivot = new Vector2(LegR.SourceRectangle.Width * LegR.Pivot.X, LegR.SourceRectangle.Height * LegR.Pivot.Y);
        Extra.Pivot = new Vector2(Extra.SourceRectangle.Width * Extra.Pivot.X, Extra.SourceRectangle.Height * Extra.Pivot.Y);
    }

    public void Update(float dt)
    {
        if (slowTimer > 0)
        {
            slowTimer -= dt;
            slowMultiplier = 0.5f;
        }
        else
        {
            slowMultiplier = 1f;
        }

        if (IsSmashing)
        {
            smashTimer -= dt;

            if (smashTimer <= 0f)
            {
                if (smashTarget != null && smashTarget.IsAlive)
                    smashTarget.TakeInstantKill();

                IsSmashing = false;
                smashTarget = null;
            }
        }

        if (!IsEating && !IsSmashing)
            Position.X -= Speed * slowMultiplier * dt;
    }

    public void StartSmash(Plant target)
    {
        if (Type != ZombieType.Gargantuar || IsSmashing)
            return;

        IsSmashing = true;
        smashTimer = SmashDuration;
        smashTarget = target;
    }

    public void ApplySlow()
    {
        slowTimer = 10f;
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
    }

    public void DrawWalk(SpriteBatch sb, SpriteFont font)
    {
        Color tint = Color.White;

        if (slowTimer > 0)
            tint = Color.LightBlue;

        if (Type == ZombieType.Gargantuar && IsPreparingSmash)
            tint = Color.Red;
        
        // arrange sprite positions w/ null handling
        Vector2 posTorso = Position + Torso.Offset;
        Vector2 posHead = posTorso + Head.Offset;
        Vector2 posArmL = posTorso + ArmL.Offset;
        Vector2 posArmR = (ArmR != null) ? posTorso + ArmR.Offset : Position;
        Vector2 posHand = (Hand != null) ? posArmL + Hand.Offset : Position;
        Vector2 posLegL = posTorso + LegL.Offset;
        Vector2 posLegR = posTorso + LegR.Offset;
        Vector2 posExtra = (Type == ZombieType.Gargantuar) ? posHand + Extra.Offset : posHead + Extra.Offset;
        
        if (Type == ZombieType.Normal)
            sb.Draw(Texture, posArmR, ArmR.SourceRectangle, tint,
                rot2, ArmR.Pivot, Scale, SpriteEffects.None, 0.0f);
        
        sb.Draw(Texture, posLegR, LegR.SourceRectangle, tint,
            rot1, LegR.Pivot, Scale, SpriteEffects.None, 0.0f);
        sb.Draw(Texture, posLegL, LegL.SourceRectangle, tint,
            rot2, LegL.Pivot, Scale, SpriteEffects.None, 0.0f);
        sb.Draw(Texture, posTorso, Torso.SourceRectangle, tint,
            rot3, Torso.Pivot, Scale, SpriteEffects.None, 0.0f);
        sb.Draw(Texture, posArmL, ArmL.SourceRectangle, tint,
            rot1, ArmL.Pivot, Scale, SpriteEffects.None, 0.0f);
        sb.Draw(Texture, posHead, Head.SourceRectangle, tint,
            rot1, Head.Pivot, Scale, SpriteEffects.None, 0.0f);
        
        if (Type == ZombieType.Gargantuar)
        {
            sb.Draw(Texture, posExtra, Extra.SourceRectangle, tint,
                rot3, Head.Pivot, Scale, SpriteEffects.None, 0.0f);
            sb.Draw(Texture, posHand, Hand.SourceRectangle, tint,
                rot3, Hand.Pivot, Scale, SpriteEffects.None, 0.0f);
        }
        else if (Type == ZombieType.Buckethead)
            sb.Draw(Texture, posExtra, Extra.SourceRectangle, tint, 
                rot1, Head.Pivot, Scale, SpriteEffects.None, 0.0f);

        Vector2 HealthOffset = (Type == ZombieType.Gargantuar) ? 
            new Vector2(-10, -150) : new Vector2(-10, -120);
        
        // health display
        sb.DrawString(
            font,
            Health.ToString(),
            Position + HealthOffset,
            Color.White
        );
    }
    
    public void AnimateWalk(GameTime gameTime)
    {
        double t = gameTime.TotalGameTime.TotalSeconds;
        rot1 = (float)(Math.Sin(t * 2) * (Math.PI / 23) + (2 * Math.PI));
        rot2 = (float)(Math.Cos(t * 2) * (Math.PI / 10) + (2 * Math.PI));
        rot3 = (float)(Math.Sin(t * 2) * (Math.PI / 72) + (2 * Math.PI));
    }

    // legacy Draw method
    public void Draw(
        SpriteBatch spriteBatch,
        Texture2D normalTexture,
        Texture2D bucketTexture,
        Texture2D giantTexture,
        SpriteFont font)
    {
        Color tint = Color.White;

        if (slowTimer > 0)
            tint = Color.LightBlue;

        if (Type == ZombieType.Gargantuar && IsPreparingSmash)
            tint = Color.Red;

        Texture2D tex = Type switch
        {
            ZombieType.Normal => normalTexture,
            ZombieType.Buckethead => bucketTexture,
            ZombieType.Gargantuar => giantTexture,
            _ => normalTexture
        };

        float scale = Type == ZombieType.Gargantuar ? 0.75f : 1f;
        Vector2 drawPos = Position + new Vector2(0, 70f);

        spriteBatch.Draw(
            tex,
            drawPos,
            null,
            tint,
            0f,
            new Vector2(tex.Width / 2f, tex.Height),
            scale,
            SpriteEffects.None,
            0f
        );

        spriteBatch.DrawString(
            font,
            Health.ToString(),
            drawPos + new Vector2(-10, -170),
            Color.White
        );
    }

    public float GetEatingMultiplier()
    {
        return slowTimer > 0 ? 0.5f : 1f;
    }
}