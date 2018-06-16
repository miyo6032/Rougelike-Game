using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Coordinates movements between the player and the enemy to keep the from moving into the same square
/// </summary>
public class MovementTracker : MonoBehaviour
{
    private readonly List<Vector2Int> claimedSpots = new List<Vector2Int>();

    /// <summary>
    /// Claim a spot that the object will move into that no one else can claim
    /// </summary>
    /// <param name="pos"></param>
    public void ClaimSpot(Vector2Int pos)
    {
        claimedSpots.Add(pos);
    }

    /// <summary>
    /// After moving, remove the claim to that spot
    /// </summary>
    /// <param name="pos"></param>
    public void RemoveClaim(Vector2Int pos)
    {
        claimedSpots.Remove(pos);
    }

    /// <summary>
    /// If a certain position is claimed
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool SpotClaimed(Vector2Int pos)
    {
        return claimedSpots.Contains(pos);
    }
}
