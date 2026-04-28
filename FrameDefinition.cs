using Microsoft.Xna.Framework;
namespace final_project_PvZ;

public class FrameDefinition
{
    public Rectangle SourceRectangle { get; set; }
    public Vector2 Pivot { get; set; } = Vector2.Zero;
    public Vector2 Offset { get; set; } = Vector2.Zero;
    public Point Size { get; set; }
    
    // Add this method
    public FrameDefinition Clone()
    {
        return new FrameDefinition {
            SourceRectangle = this.SourceRectangle,
            Pivot = this.Pivot,
            Size = this.Size,
            Offset = this.Offset
        };
    }
}