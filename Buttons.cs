using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace final_project_PvZ;

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    None
}

public class Buttons
{
    private Texture2D _easyTexture;
    private Texture2D _mediumTexture;
    private Texture2D _hardTexture;

    private Rectangle _easyRect;
    private Rectangle _mediumRect;
    private Rectangle _hardRect;

    private float _easyClickTimer = 0f;
    private float _mediumClickTimer = 0f;
    private float _hardClickTimer = 0f;

    private const float ClickDuration = 0.1f;

    public Buttons(Texture2D easy, Texture2D medium, Texture2D hard)
    {
        _easyTexture = easy;
        _mediumTexture = medium;
        _hardTexture = hard;

        int buttonWidth = 400;
        int buttonHeight = 120;

        _easyRect = new Rectangle(550, 200, buttonWidth, buttonHeight);
        _mediumRect = new Rectangle(550, 400, buttonWidth, buttonHeight);
        _hardRect = new Rectangle(550, 600, buttonWidth, buttonHeight);
    }

    public Difficulty HandleClick(Point mousePoint)
    {
        if (_easyRect.Contains(mousePoint))
        {
            _easyClickTimer = ClickDuration;
            return Difficulty.Easy;
        }

        if (_mediumRect.Contains(mousePoint))
        {
            _mediumClickTimer = ClickDuration;
            return Difficulty.Medium;
        }

        if (_hardRect.Contains(mousePoint))
        {
            _hardClickTimer = ClickDuration;
            return Difficulty.Hard;
        }

        return Difficulty.None;
    }

    public void Update(float dt)
    {
        _easyClickTimer = Math.Max(0, _easyClickTimer - dt);
        _mediumClickTimer = Math.Max(0, _mediumClickTimer - dt);
        _hardClickTimer = Math.Max(0, _hardClickTimer - dt);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var mouse = Mouse.GetState();
        Point mousePoint = new Point(mouse.X, mouse.Y);

        DrawSingleButton(spriteBatch, _easyTexture, _easyRect, _easyClickTimer, mousePoint);
        DrawSingleButton(spriteBatch, _mediumTexture, _mediumRect, _mediumClickTimer, mousePoint);
        DrawSingleButton(spriteBatch, _hardTexture, _hardRect, _hardClickTimer, mousePoint);
    }

    private void DrawSingleButton(SpriteBatch spriteBatch, Texture2D texture, Rectangle rect, float clickTimer, Point mousePoint)
    {
        float scale = 1.0f;

        if (clickTimer > 0)
            scale = 0.9f;
        else if (rect.Contains(mousePoint))
            scale = 1.1f;

        Vector2 center = new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);

        spriteBatch.Draw(
            texture,
            center,
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