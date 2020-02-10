using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryDroneBehaviour : MonoBehaviour
{
    enum SentryState
    {
        Idle, Approach, Wait, Charge, Strafe, Retreat
    }
    private SentryState sState;

    public List<GameObject> DroneComponents;
    public Material LaserMaterial;
    public float RotationRate = 1.0f;

    public float Speed = 1.0f;
    public float Health = 3.0f;
    public float Damage = 1.0f;
    public float DeathTime = 4.0f;

    public float RechargeTime = 4.0f;
    public float FiringTime = 1.0f;
    public float LaserLifespan = 0.3f;
    

    public bool Alive = true;
    public float ShootRange = 25.0f;
    public float AggroRange = 28.0f;

    public GameObject ExplosionPrefab;
    public AudioSource DeathAudioSource;

    private Transform pTransform;
    private PlayerBehaviour pBehaviour;
    private float totalDyingTime = 0.0f;
    private float totalRechargeTime = 0.0f;
    private float totalFiringTime = 0.0f;
    private float strafeRate = 0.0f;
    private ComponentBehaviour selectedDrone;
    private LineRenderer laser;

    // Start is called before the first frame update
    void Start()
    {
        pBehaviour = FindObjectOfType<PlayerBehaviour>();
        pTransform = pBehaviour.gameObject.transform;
        
        int droneSelectionIndex = Random.Range(0, DroneComponents.Count - 1);
        selectedDrone = DroneComponents[droneSelectionIndex].GetComponent<ComponentBehaviour>();

        if (selectedDrone)
        {
            selectedDrone.Select();
        }
        sState = SentryState.Idle;   
    }

    // Update is called once per frame
    void Update()
    {
        SentryUpdate();
       

    }

    private void SentryUpdate()
    {
        if (Alive)
        {
            SetSentryState();

            if(sState == SentryState.Approach)
            {
                ApproachTarget(pTransform);
            }
            else if(sState == SentryState.Charge)
            {
                Shoot();
            }
            else if(sState == SentryState.Strafe)
            {
                Strafe();
            }
        }
        else
        {
            totalDyingTime += Time.deltaTime;

            if (totalDyingTime > DeathTime)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void SetSentryState()
    {
        float distanceFromPlayer = (pTransform.position - transform.position).magnitude;

        if (distanceFromPlayer > AggroRange)
        {
            sState = SentryState.Idle;
            totalFiringTime = 0.0f;
        }
        else if (distanceFromPlayer > ShootRange)
        {
            sState = SentryState.Approach;
            totalFiringTime = 0.0f;
        }
        else
        {
            if(totalRechargeTime > 0.0f)
            {
                sState = SentryState.Strafe;
            }
            else
            {
                sState = SentryState.Charge;
            }
        }
    }

    private void ApproachTarget(Transform target)
    {
        if (target)
        {
            transform.LookAt(target);

            transform.position += transform.forward * Speed * Time.deltaTime;
        }
    }

    private void Shoot()
    {
        transform.Rotate(0, 0, RotationRate);

        totalFiringTime += Time.deltaTime;

        if (FiringTime + LaserLifespan < totalFiringTime)
        {
            if (laser)
            {
                Destroy(laser);
                totalFiringTime = 0.0f;
                totalRechargeTime += Time.deltaTime;
                strafeRate = Random.Range(-0.2f, 0.2f);
            }
        }
        else if (FiringTime <= totalFiringTime)
        {
            //Show a laser effect from sentry to the player
            if (!laser)
            {
                laser = gameObject.AddComponent<LineRenderer>();

                if (laser)
                {
                    float xOffset = Random.Range(-1.0f, 1.0f);
                    float yOffset = Random.Range(-1.0f, 1.0f);
                    laser.material = LaserMaterial;
                    laser.SetPositions(new Vector3[] { selectedDrone.gameObject.transform.position, new Vector3(pTransform.position.x + xOffset, pTransform.position.y + yOffset, pTransform.position.z) });
                    laser.startWidth = 0.08f;
                    laser.endWidth = 1.0f;
                }

                pBehaviour.TakeDamage(Damage);
            }
        }
        
    }

    private void Strafe()
    {
        transform.LookAt(pTransform);
        transform.Translate(transform.right * Time.deltaTime * strafeRate);
        totalRechargeTime += Time.deltaTime;

        if(totalRechargeTime >= RechargeTime)
        {
            totalRechargeTime = 0.0f;
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Alive = false;

        foreach(GameObject component in DroneComponents)
        {
            component.SetActive(false);
        }

        //Create a death explosion effect and sound
        if (DeathAudioSource)
        {
            DeathAudioSource.PlayOneShot(DeathAudioSource.clip);
        }

        if (ExplosionPrefab)
        {
            Instantiate(ExplosionPrefab, this.transform.position, Quaternion.identity);
        }
    }
}
