using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;
    public float maxSpeed = 10.0f;
    public Transform gunTransform;
    public ShooterFinal ShooterScript;
    private Rigidbody2D rb;
    private bool isGrounded;
    public bool isPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(!isPlayer)
        {
            return;
        }
        if(BattleSystem.instance.state!=BattleState.PLAYERTURN)
        {
            return;
        }
        Move();
        Jump();
        Aim();
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
    void Aim()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - gunTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gunTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        //Flip the gun
        Vector3 localScale = new Vector3(1, 1, 1);
        if(angle > 90 || angle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = 1f;
        }
        gunTransform.transform.localScale = localScale;

    }
    void Fire()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;  // Ensure the z position is 0 since it's a 2D game
        Vector2 direction = (mousePosition - bulletSpawnPoint.position);
        Vector2 directionNormalize = direction.normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.name=transform.name+"Bullet";
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = directionNormalize * bulletSpeed;
        BattleSystem.instance.OtherPlayerTurn();
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        //BulletHit
        if(collision.gameObject.CompareTag("Bullet") )
        {
            Debug.Log(collision.gameObject.name + " : " + transform.name + "Bullet");
            if(!collision.gameObject.name.Equals(transform.name + "Bullet"))
            {
                Debug.Log("OtherPlayer Hit");
                BattleSystem.instance.PlayerBulletHit();
            }
            
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
