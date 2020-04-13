using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryShooting : MonoBehaviour
{
    public float RechargeTime = 4.0f;
    public float ChargingTime = 1.0f;
    public float LaserLifespan = 0.05f;
    public Material LaserMaterial;
    public float ShootRange = 25.0f;
    public float Damage = 1.0f;
    public GameObject CannonObject;
    public string Shoot_Sound;
    private PlayerBehaviour pBehaviour;
    public bool IsRecharged
    {
        get { return totalRechargeTime == 0.0f; }
    }

    private LineRenderer laser;
    private float totalRechargeTime = 0.0f;
    private float totalChargingTime = 0.0f;

    private void Start()
    {
        pBehaviour = GunnerController.Instance.PlayerObject.GetComponent<PlayerBehaviour>();
    }

    public void Charge(Transform target)
    {
        totalChargingTime += Time.deltaTime;

        if (ChargingTime + LaserLifespan < totalChargingTime)
        {
            if (laser)
            {
                Destroy(laser);
                totalChargingTime = 0.0f;
                totalRechargeTime += Time.deltaTime;
            }
        }
        else if (ChargingTime <= totalChargingTime)
        {
            Shoot(target);
        }
    }

    private void Shoot(Transform target)
    {
        if (!laser)
        {
            laser = gameObject.AddComponent<LineRenderer>();

            if (laser)
            {
                float xOffset = Random.Range(-1.0f, 1.0f);
                float yOffset = Random.Range(-1.0f, 1.0f);
                laser.material = LaserMaterial;
                laser.SetPositions(
                    new Vector3[] 
                    {
                        CannonObject.transform.position,
                        new Vector3(target.position.x + xOffset, target.position.y + yOffset, target.position.z)
                    }
                );
                laser.startWidth = 0.08f;
                laser.endWidth = 1.0f;
                AudioManager.Instance.PlaySound(Shoot_Sound);
            }

            pBehaviour.TakeDamage(Damage);
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
