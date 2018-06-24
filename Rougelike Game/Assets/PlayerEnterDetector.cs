using UnityEngine;

public class PlayerEnterDetector : MonoBehaviour {

    //When the player walks on top of the bag
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerEnter();
        }
    }

    //When the player walks off the bag
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerExit();
        }
    }

    public virtual void PlayerEnter()
    {

    }

    public virtual void PlayerExit()
    {

    }
}
