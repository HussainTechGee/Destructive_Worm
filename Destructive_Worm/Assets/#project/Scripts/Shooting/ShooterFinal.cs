using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterFinal : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;


    [SerializeField] private float shootRate;
    [SerializeField] private float projectileMaxMoveSpeed;
    [SerializeField] private float projectileMaxHeight;

    [SerializeField] private AnimationCurve trajectoryAnimationCurve;
    [SerializeField] private AnimationCurve axisCorrectionAnimationCurve;
    [SerializeField] private AnimationCurve projectileSpeedAnimationCurve;

    private float shootTimer;

    private void Update() {
        shootTimer -= Time.deltaTime;

        if(shootTimer <= 0 ) {
            shootTimer = shootRate;
            
        }
    }
    public void BulletFire(Vector3 targetPos)
    {
            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();

            projectile.InitializeProjectile(targetPos, projectileMaxMoveSpeed, projectileMaxHeight);
            projectile.InitializeAnimationCurves(trajectoryAnimationCurve, axisCorrectionAnimationCurve, projectileSpeedAnimationCurve);
    }

}
