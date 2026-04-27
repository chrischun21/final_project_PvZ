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

    // GARGANTUAR SMASH
    private const float SmashDuration = 1.2f;
    public bool IsPreparingSmash => IsSmashing;

    public bool IsSmashing = false;
    private float smashTimer = 0f;
    private Plant smashTarget = null;

    public Zombie(Vector2 position, ZombieType type)
    {
        Position = position;
        Type = type;

        switch (type)
        {
            case ZombieType.Normal:
                Health = rand.Next(10, 16);
                Speed = rand.Next(25, 36);
                break;

            case ZombieType.Buckethead:
                Health = rand.Next(50, 66);
                Speed = rand.Next(25, 36);
                break;

            case ZombieType.Gargantuar:
                Health = 150;
                Speed = rand.Next(16, 27);
                break;
        }
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