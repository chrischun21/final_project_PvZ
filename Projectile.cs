using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace final_project_PvZ;

public class Projectile
{
    public Vector2 Position;
    public float Speed = 450f;
    public bool IsAlive = true;
    public float Direction;
    public bool IsSnow;

    public Projectile(Vector2 pos, float direction, bool isSnow)
    {
        Position = pos;
        Direction = direction;
        IsSnow = isSnow;
    }

    public void Update(float dt)
    {
        Position.X += Speed * Direction * dt;

        // kill if off screen
        if (Position.X > 1500)
            IsAlive = false;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        spriteBatch.Draw(
            pixel,
            new Rectangle((int)Position.X, (int)Position.Y, 10, 10),
            Color.LimeGreen
        );
    }

    public Rectangle GetHitbox()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, 10, 10);
    }
}