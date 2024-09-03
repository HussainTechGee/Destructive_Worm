using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BulletScript : MonoBehaviour
{
    Rigidbody2D rb;
    public int PlayerId=-1;
 
    public void Init(int id)

    {
        PlayerId = id;
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
   void FixedUpdate()
    {
        if (rb)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
