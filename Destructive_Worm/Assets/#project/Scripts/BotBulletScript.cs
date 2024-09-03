
using UnityEngine;
public class BotBulletScript : MonoBehaviour
{
    public int PlayerId=-1;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(int id)

    {
        rb = GetComponent<Rigidbody2D>();
        PlayerId = id;
    }
    private void FixedUpdate()
    {
        if(rb)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
    }

}
