using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using Fusion;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
public class BotController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform bulletSpawnPoint, directionOfFire;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float gravity = 9.8f;
    public Transform gunTransform;
    public ParticleSystem HitParticle;
    private Rigidbody2D rb;
    private bool isGrounded;
    public bool isMyTurn=false;
    [HideInInspector] public bool canMove;
    public AccuracyLevel accuracyLevel;
    SpriteRenderer spriteRenderer;
    float deviation;// how much anngle of gun will deviate
    float horizontalDistance, verticalDistance;
    [Networked, OnChangedRender(nameof(IdChanged))]
    public int PlayerId { get; set; } = 0;
    public enum AccuracyLevel {Beginner, Standard, Pro};

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void IdChanged()
    {
        Debug.Log("Id Update");
    }

    /* public override void FixedUpdateNetwork()
     {

         if(!HasInputAuthority)
         {
             return;
         }
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
     }*/
    /*private void FixedUpdate()
    {
        if(canMove)
        {
            Aim();
            canMove = false;
        }
    }*/

    public IEnumerator Move()
    {
        //canMove = true;
        //Invoke("StopMove", 3);
      //  this.target = target;
        if (isMyTurn)
        {
            if (Random.value > 0.5f)
            {
                int turn = Random.Range(-1, 2);
                float moveInput = turn;
                //while(canMove)
                {
                    rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
                }
                yield return new WaitForSeconds(3);
                moveInput = 0;
                rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
                canMove = true;
            }
            else
            {
                canMove = true;
            }
            
        }
        Invoke("StopMove", 0.5f);
        Invoke("GetMaxDaviation", 1f);
        Invoke("Fire", 2f);
    }
    void StopMove()
    {
        canMove = false;

    }

    void Jump()
    {
        if (isMyTurn)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
    }
    float CalculateLaunchAngle(Vector2 shooterPosition, Vector2 targetPosition, float initialSpeed, AccuracyLevel accuracyLevel)
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
        // Add accuracy adjustment based on skill level
        chosenAngle = ApplyAccuracyDeviation(chosenAngle, accuracyLevel);

        return chosenAngle; 
    }
    void GetMaxDaviation()
    {
        // Define the deviation ranges for different skill levels
        float maxDeviation;
        switch (accuracyLevel)
        {
            case AccuracyLevel.Beginner:
                maxDeviation = 15f;  // Example: 15 degrees of inaccuracy
                break;
            case AccuracyLevel.Standard:
                maxDeviation = 7f;  // Example: 15 degrees of inaccuracy
                break;
            case AccuracyLevel.Pro:
                maxDeviation = 2f;  // Example: 2 degrees of inaccuracy
                break;
            default:
                maxDeviation = 5f;  // Default value for any other cases
                break;
        }
        // Apply random deviation within the range [-maxDeviation, +maxDeviation]
        deviation = Random.Range(-maxDeviation, maxDeviation);
    }
    float ApplyAccuracyDeviation(float angle, AccuracyLevel accuracyLevel)
    {
        return angle + deviation;
    }
    void AimGun(Transform gunTransform, float launchAngle)
    {
        // Set the rotation of the gun based on the launch angle
        gunTransform.rotation = Quaternion.Euler(0, 0, launchAngle);
      //  Debug.Log("Angle: "+launchAngle);

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
    void AimAtTarget(Vector2 shooterPosition, Vector2 targetPosition, Transform gunTransform, float initialSpeed, AccuracyLevel accuracyLevel)
    {
        // Calculate the launch angle to reach the target
        float launchAngle = CalculateLaunchAngle(shooterPosition, targetPosition, initialSpeed, accuracyLevel);

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
    void Update()
    {
        if(canMove)
        {
            // Example positions for shooter and target, replace with actual positions
            Vector2 shooterPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 targetPosition = new Vector2(BotManager.instance.targetPlayer.position.x, BotManager.instance.targetPlayer.position.y);

            // Initial speed of the bullet, replace with actual speed
            float initialSpeed = bulletSpeed;

            // Call the function to aim at the target
            AimAtTarget(shooterPosition, targetPosition, gunTransform, initialSpeed, accuracyLevel);
        }
    }

    /*void Aim()
    {
        // Calculate direction to the opponent
        Debug.Log("Target: "+target);
        Vector3 targetPosition = BotManager.instance.botsList[target].transform.position ;
        Vector3 direction = (targetPosition - gunTransform.position).normalized;

        // Apply accuracy offset
        float accuracyOffset = Mathf.Lerp(0f, 1f, 1f - accuracy); // accuracy of 0 = beginner, 1 = Pro
        float offsetX = Random.Range(-accuracyOffset, accuracyOffset);
        float offsetY = Random.Range(-accuracyOffset, accuracyOffset);

        direction.x += offsetX;
        direction.y += offsetY;
        direction.Normalize();

        // Calculate angle and set gun rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Debug.Log("Angle: " + angle);
        gunTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Flip the gun if necessary
        Vector3 localScale = new Vector3(1.3f,1.3f,1);
        if (angle > 90 || angle < -90)
        {
            localScale.y = -1.3f;
        }
        else
        {
            localScale.y = 1.3f;
        }
        gunTransform.localScale = localScale;
        //Invoke("Fire",1);

    }*/
    void Fire()
    {
     if(isMyTurn)
        {
            Vector2 direction = (directionOfFire.position - bulletSpawnPoint.position);
            Vector2 directionNormalize = direction.normalized;
            if(Object.HasStateAuthority)
            {
                RPC_BotFire(directionNormalize, PlayerId);
            }
            
        }
        
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_BotFire(Vector3 directionNormalize, int pId)
    {
        isMyTurn = false;
        Debug.Log("Fire RpC");
        

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.name = transform.name + "Bullet";
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = directionNormalize * bulletSpeed;
        bullet.GetComponent<BulletScript>().Init( pId);
        BattleSystem.instance.NextTurn(4);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        //BulletHit
        //if(collision.gameObject.CompareTag("Bullet") )
        //{
        //    Debug.Log(collision.gameObject.name + " : " + transform.name + "Bullet");
        //    if(!collision.gameObject.name.Equals(transform.name + "Bullet"))
        //    {
        //        Debug.Log("OtherPlayer Hit");
        //        //BattleSystem.instance.PlayerBulletHit();
        //    }
            
        //}
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //BulletHit
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log(collision.gameObject.name + " : " + transform.name + "Bullet");

            //Check player should be the iffernt from fire player.
            if (collision.GetComponent<BulletScript>() && PlayerId != collision.GetComponent<BulletScript>().PlayerId)
            {
                TakeDamage();
                Debug.Log("OtherPlayer Hit");
                Destroy(collision.gameObject);
                if(Object.HasStateAuthority)
                {
                    transform.GetComponent<Unit>().TakeDamage(8, PlayerId);
                }
                
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
    void TakeDamage()
    {
        HitParticle.Play();
        spriteRenderer.color = Color.red;
        float punchScaleAmount = 1.2f;
        float punchDuration = 0.2f;
        Vector3 originalScale = transform.localScale;
        // Punch Scale Effect
        transform.DOScale(originalScale * punchScaleAmount, punchDuration)
                  .SetEase(Ease.InOutBounce)
                 .OnComplete(() =>
                 {
                     transform.DOScale(originalScale, punchDuration).SetEase(Ease.InBounce);
                     spriteRenderer.color = Color.white;
                 });

    }
}
