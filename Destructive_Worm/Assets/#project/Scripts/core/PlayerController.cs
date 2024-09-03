using System.Collections;
using UnityEngine;
using Fusion;
public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform bulletSpawnPoint,bulletDirection;
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
    public bool isFired;
    float deviation;// how much anngle of gun will deviate
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
        Vector2 shooterPosition = new Vector2(transform.position.x, transform.position.y);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;  // Ensure the z position is 0 since it's a 2D game
        Vector2 targetPosition = new Vector2(mousePosition.x, mousePosition.y);

        // Initial speed of the bullet, replace with actual speed
        float initialSpeed = bulletSpeed;

        // Call the function to aim at the target
        AimAtTarget(shooterPosition, targetPosition, gunTransform, initialSpeed);


    }
    void AimAtTarget(Vector2 shooterPosition, Vector2 targetPosition, Transform gunTransform, float initialSpeed)
    {
        // Calculate the launch angle to reach the target
        float launchAngle = CalculateLaunchAngle(shooterPosition, targetPosition, initialSpeed);

        // Check if the launch angle is valid
        if (!float.IsNaN(launchAngle))
        {
            // Aim the gun using the calculated launch angle
            AimGun(gunTransform, launchAngle);
        }
        else
        {
            Debug.Log("Target is unreachable with the given initial speed.");
        }
    }
    void AimGun(Transform gunTransform, float launchAngle)
    {
        // Set the rotation of the gun based on the launch angle
        gunTransform.rotation = Quaternion.Euler(0, 0, launchAngle);
        Debug.Log("Angle: " + launchAngle);

        // Flip the gun if the angle is greater than 90 degrees or less than -90 degrees
        Vector3 localScale = gunTransform.localScale;
        if (launchAngle > 90 && launchAngle < 270)
        {
            localScale.y = -1.3f;
        }
        else
        {
            localScale.y = 1.3f;
        }
        gunTransform.localScale = localScale;
    }
    float CalculateLaunchAngle(Vector2 shooterPosition, Vector2 targetPosition, float initialSpeed)
    {
        float g = Mathf.Abs(Physics2D.gravity.y);

        // Calculate distances
        float horizontalDistance = targetPosition.x - shooterPosition.x;
        float verticalDistance = targetPosition.y - shooterPosition.y;
        float speedSquared = initialSpeed * initialSpeed;

        // Calculate discriminant
        float discriminant = (speedSquared * speedSquared) - (g * (g * horizontalDistance * horizontalDistance + 2 * verticalDistance * speedSquared));

        if (discriminant < 0)
        {
            // Target is unreachable with the given initial speed
            return float.NaN;
        }

        // Compute the two possible launch angles (in radians)
        float root = Mathf.Sqrt(discriminant);
        float lowAngle = Mathf.Atan((speedSquared - root) / (g * horizontalDistance));
        float highAngle = Mathf.Atan((speedSquared + root) / (g * horizontalDistance));

        // Convert radians to degrees and choose the desired angle (high or low arc)
        float highAngleDeg = Mathf.Rad2Deg * highAngle;
        float lowAngleDeg = Mathf.Rad2Deg * lowAngle;

        if (horizontalDistance < 0)
        {
            // Invert both angles for aiming left
            lowAngleDeg = lowAngleDeg - 180;
            highAngleDeg = highAngleDeg - 180;
        }
        float chosenAngle = highAngleDeg; // You can choose lowAngleDeg or highAngleDeg based on your preference
       

        return chosenAngle;
    }
 
    void Fire()
    {
        if(!isFired)
        {
            //isFired = true;
            Vector2 direction = (bulletDirection.position - bulletSpawnPoint.position);
            Vector2 directionNormalize = direction.normalized;
            RPC_Fire(directionNormalize, PlayerId);
        }
       
        

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Fire(Vector2 directionNormalize, int pId)
    {
         Debug.Log("Fire RpC");


        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.name = transform.name + "Bullet";
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = directionNormalize * bulletSpeed;
        bullet.GetComponent<BulletScript>().Init(pId);

        //GameObject bulletObj = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        //Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
        //// Calculate the launch direction based on the angle
        //Vector2 launchDirection = new Vector2(Mathf.Cos(launchAngle * Mathf.Deg2Rad), Mathf.Sin(launchAngle * Mathf.Deg2Rad));

        //// Apply the velocity to the bullet
        //bulletRb.velocity = launchDirection * bulletSpeed;
        //bulletObj.GetComponent<BulletScript>().Init( pId);
        //  BattleSystem.instance.NextTurn(4);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_NextTurn()
    {
        BattleSystem.instance.NextTurn(.5f);
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
            if (collision.GetComponent<BulletScript>() && PlayerId!=collision.GetComponent<BulletScript>().PlayerId)
            {
                Debug.Log("OtherPlayer Hit");
                transform.GetComponent<Unit>().RPC_TakeDamage(8);
            }
           else if (collision.GetComponent<BotBulletScript>() && PlayerId != collision.GetComponent<BotBulletScript>().PlayerId)
            {
                Debug.Log("OtherBot Hit");
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
