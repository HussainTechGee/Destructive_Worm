using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BulletScript : MonoBehaviour
{
    Rigidbody2D rb;
    Vector3 targetdirection, targetPosition;
    float moveSpeed=15, fixedYVelocity=1f;
    bool isReached;
    public int PlayerId=-1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(Vector3 pos,int id)

    {
        PlayerId = id;
        targetPosition = pos;
        Vector3 currentPosition = transform.position;
        targetdirection = (pos - currentPosition).normalized;
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
   void FixedUpdate()
    {
        if (rb)
        {
            Vector3 currentPosition = transform.position;
            // Direction towards the target

            // Set the velocity in the X direction to move towards the target at a specific speed
            

            // Check if the object has reached or passed the target in the X direction
            if (Vector2.Distance(currentPosition, targetPosition) > 0.2f && isReached==false)
            {
                // Calculate the velocity required to reach the target
                rb.velocity = targetdirection * moveSpeed;
                

            }
            else
            {
                isReached = true;
                float velocityX = targetdirection.x * moveSpeed;
                // After reaching the target, maintain the horizontal velocity and apply a fixed downward velocity
                //fixedYVelocity += .1f;
                rb.velocity = new Vector2(velocityX, -Mathf.Abs(fixedYVelocity));
            }
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        }
    }
}
