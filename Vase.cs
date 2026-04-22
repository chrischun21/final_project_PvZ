namespace final_project_PvZ;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

public class Vase
{

    public Vector2 Position;

    public VaseType Type;

    public bool IsBreaking;
    public bool IsBroken;

    public float BreakTimer;

    public Vase(Vector2 position, VaseType type)
    {
        Position = position;
        Type = type;

        IsBreaking = false;
        IsBroken = false;
        BreakTimer = 0f;
    }
}