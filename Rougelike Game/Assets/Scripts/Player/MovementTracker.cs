using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Coordinates movements between the player and the enemy to keep the from moving into the same square
/// </summary>
public class MovementTracker : MonoBehaviour
{
    private readonly Dictionary<MovingObject, Vector2Int> claimedSpots = new Dictionary<MovingObject, Vector2Int>();

    /// <summary>
    /// Claim a spot that the object will move into that no one else can claim
    /// </summary>
    /// <param name="pos"></param>
    public void ClaimSpot(MovingObject source, Vector2Int pos)
    {
        claimedSpots.Add(source, pos);
    }

    /// <summary>
    /// After moving, remove the claim to that spot
    /// </summary>
    /// <param name="pos"></param>
    public void RemoveClaim(MovingObject source)
    {
        claimedSpots.Remove(source);
    }

    /// <summary>
    /// If a certain position is claimed
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool SpotClaimed(Vector2Int pos)
    {
        foreach (Vector2Int claimed in claimedSpots.Values)
        {
            if (pos == claimed)
            {
                return true;
            }
        }
        return false;
    }
}