using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Strategy/Pattern")]
public class PatternStrategy : SearchStrategy
{
    [Header("패턴 설정")]
    public List<Vector2Int> pattern;

    public override List<Vector2Int> GetCoords(InventoryItem source, InventoryGrid grid, int originX, int originY)
    {
        return GridSearcher.GetValidCoordinates(originX, originY, pattern, grid);
    }
}
