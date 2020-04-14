using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryShooting : MonoBehaviour
{
    public float RechargeTime = 4.0f;
    public float ChargingTime = 1.0f;
    public float ShootRange = 25.0f;
    public float Damage = 1.0f;

    public GameObject CannonObject, ProjectilePrefab;
    public string Shoot_Sound;
    private PlayerBehaviour pBehaviour;
    private SentryDrone sDrone;

    public bool IsRecharged
    {
        get { return totalRechargeTime == 0.0f; }
    }

    private float totalRechargeTime = 5.0f;
    private float totalChargingTime = 0.0f;

    private void Start()
    {
        sDrone = GetComponentInParent<SentryDrone>();
        

    }

    public void Charge(Transform target)
    {
        totalChargingTime += Time.deltaTime;

        if(totalChargingTime > ChargingTime)
        {
            totalChargingTime = 0.0f;
            Shoot(target);
        }
    }

    private void Shoot(Transform target)
    {
        if (!pBehaviour)
        {
            pBehaviour = GunnerController.Instance.PlayerObject.GetComponent<PlayerBehaviour>();
        }

        if (ProjectilePrefab)
        {
            AudioManager.Instance.PlaySound(Shoot_Sound);
            //Instantiate the projectile and send it in the direction of the player

            GameObject spawnedProjectile = Instantiate(ProjectilePrefab, CannonObject.transform.position, Quaternion.identity, GunnerController.Instance.ProjectilesCollection.transform);
            spawnedProjectile.transform.LookAt(pBehaviour.gameObject.transform.position);
        
            //Assign the colour and the velocity
            EnemyProjectile eProjectile = spawnedProjectile.GetComponent<EnemyProjectile>();
            eProjectile.SetColour(ColourManager.Instance.ProjectileMaterialMap[sDrone.Colour], sDrone.Colour);
        }

            
        
    }

    public void Recharge()
    {
        totalRechargeTime += Time.deltaTime;

        if (totalRechargeTime >= RechargeTime)
        {
            totalRechargeTime = 0.0f;
        }
    }

    public bool IsInRange(float distanceFromTarget)
    {
        return distanceFromTarget < ShootRange;
    }
}
