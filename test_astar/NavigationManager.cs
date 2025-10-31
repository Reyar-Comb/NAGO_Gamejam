using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class NavigationManager : Node
{
    [Export] public TileMapLayer GroundLayer;
    private AStarGrid2D astar;

    public override void _Ready()
    {
        astar = new AStarGrid2D();
        astar.Region = GroundLayer.GetUsedRect();
        astar.CellSize = GroundLayer.TileSet.TileSize;
        astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
        astar.Update();
        
        foreach (Vector2I cell in GroundLayer.GetUsedCells())
        {
            astar.SetPointSolid(cell);
        }
    }
    
    public Vector2[] GetPath(Vector2 from, Vector2 to)
    {
        Vector2I startCell = GroundLayer.LocalToMap(from);
        Vector2I targetCell = GroundLayer.LocalToMap(to);

        Vector2I[] pathCells = astar.GetIdPath(startCell, targetCell).ToArray();

        Vector2[] worldPath = new Vector2[pathCells.Length];
        for (int i = 0; i < pathCells.Length; i++)
        {
            worldPath[i] = GroundLayer.MapToLocal(pathCells[i]);
        }
        
        return worldPath;

    }
}
