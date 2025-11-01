using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class NavigationManager : Node
{
	public TileMapLayer ObstacleLayer = null;
	private AStarGrid2D astar;

	public TileMapLayer FindObstacleLayer()
	{
		var layer = GetTree().CurrentScene.GetNodeOrNull<TileMapLayer>("ObstacleLayer");
		if (layer == null)
		{
			CallDeferred("FindObstacleLayer");
			return null;
		}
		return layer;
	}
	public override void _Ready()
	{
		ObstacleLayer = FindObstacleLayer();
		astar = new AStarGrid2D();
		
		astar.Region = ObstacleLayer.GetUsedRect();
		astar.CellSize = ObstacleLayer.TileSet.TileSize;
		astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
		astar.Update();
		
		foreach (Vector2I cell in ObstacleLayer.GetUsedCells())
		{
			astar.SetPointSolid(cell);
		}
	}
	
	public Vector2[] GetPath(Vector2 from, Vector2 to)
	{
		Vector2I startCell = ObstacleLayer.LocalToMap(from);
		Vector2I targetCell = ObstacleLayer.LocalToMap(to);

		Vector2I[] pathCells = astar.GetIdPath(startCell, targetCell).ToArray();

		Vector2[] worldPath = new Vector2[pathCells.Length];
		for (int i = 0; i < pathCells.Length; i++)
		{
			worldPath[i] = ObstacleLayer.MapToLocal(pathCells[i]);
			worldPath[i] = new Vector2I((int)worldPath[i].X, (int)worldPath[i].Y);
		}
		// foreach (var cell in pathCells)
		// {
		// 	GD.Print("Path Cell: " + cell);
		// }
		return worldPath;

	}
}
