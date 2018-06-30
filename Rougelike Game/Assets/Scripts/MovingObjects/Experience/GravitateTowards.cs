using UnityEngine;

/// <summary>
/// Makes the game object gravitate towards the player
/// </summary>
public class GravitateTowards : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private Transform target;
    public float acceleration;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        target = GameObject.Find("Player").GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        //Get direction need to face
        Vector3 desiredDirection = target.position - transform.position;

        desiredDirection.Normalize();

        rigidbody.AddForce(desiredDirection * acceleration);
    }

}
