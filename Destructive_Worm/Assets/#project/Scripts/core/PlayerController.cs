using System.Collections;
using UnityEngine;
using Fusion;
public class PlayerController : NetworkBehaviour
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
   // public ShooterFinal ShooterScript;
    private Rigidbody2D rb;
    private bool isGrounded;
    public bool isPlayer;
    [Networked, OnChangedRender(nameof(IdChanged))]
    public int PlayerId { get; set; } = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void IdChanged()
    {
        Debug.Log("Id Update");
    }
    public override void FixedUpdateNetwork()
    {
        
        if(!HasInputAuthority)
        {
            return;
        }
        //if(!isPlayer)
        //{
        //    return;
        //}
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
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            // Set the PlayerID to the local player's ID
            PlayerId = Runner.LocalPlayer.PlayerId;
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
        RPC_Fire(mousePosition,PlayerId);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_Fire(Vector3 mousePosition, int pId)
    {
         Debug.Log("Fire RpC");
        GameObject bulletObj = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bulletObj.GetComponent<BulletScript>().Init(mousePosition, pId);
        BattleSystem.instance.NextTurn();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //BulletHit
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log(collision.gameObject.name + " : " + transform.name + "Bullet");

            //Check player should be the iffernt from fire player.
            if (PlayerId!=collision.GetComponent<BulletScript>().PlayerId)
            {
                Debug.Log("OtherPlayer Hit");
                transform.GetComponent<Unit>().RPC_TakeDamage(8);
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
