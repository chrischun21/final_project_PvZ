using Microsoft.Xna.Framework;

namespace final_project_PvZ;

public enum VaseAppearance
{
    Normal,
    PlantHint,
    ZombieHint
}

public class Vase
{
    public Vector2 Position;

    public VaseAppearance Appearance;

    public PlantType? StoredPlant;
    public ZombieType? StoredZombie;

    public bool RevealedByPlantern = false;

    public bool IsBreaking;
    public bool IsBroken;

    public float BreakTimer;

    public Vase(
        Vector2 position,
        PlantType? storedPlant = null,
        ZombieType? storedZombie = null)
    {
        Position = position;

        StoredPlant = storedPlant;
        StoredZombie = storedZombie;

        Appearance = VaseAppearance.Normal;

        IsBreaking = false;
        IsBroken = false;
        BreakTimer = 0f;
    }

    public bool ContainsPlant => StoredPlant.HasValue;
    public bool ContainsZombie => StoredZombie.HasValue;
}