using UnityEngine;
using System.Collections.Generic;

//Coordinates movements between the player and the enemy to keep collisions from happening
//Basically, lets the everyone coordinate and communicate where they are going, so there are no
//disasterous collisions. Imagine if we had this in real life!
public class MovementTracker : MonoBehaviour {

    List<Vector2Int> claimedSpots = new List<Vector2Int>();

    public void ClaimSpot(Vector2Int pos)
    {
        claimedSpots.Add(pos);
    }

    public void RemoveClaim(Vector2Int pos)
    {
        claimedSpots.Remove(pos);
    }

    public List<Vector2Int> GetClaimedSpots()
    {
        return claimedSpots;
    }

    public bool SpotClaimed(Vector2Int pos)
    {
        return claimedSpots.Contains(pos);
    }
}
