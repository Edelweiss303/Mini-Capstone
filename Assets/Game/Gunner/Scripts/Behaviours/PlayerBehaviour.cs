using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public GameObject Crosshairs;
    public GameObject BulletImpactEffectPrefab;
    public GameObject DamageVisionPanel;
    public float Damage = 1.0f;
    public float MaxHealth = 100.0f;
    public float MaxAmmo = 8.0f;
    public float Range = 100.0f;
    public float DamageVisionEffectTime = 1.0f;
    public float ReloadingTime = 1.5f;
    public float RotationRate = 0.01f;
    public bool Alive = true;

    public AudioSource ShotAudioSource;
    public AudioSource TakeDamageAudioSource;
    public AudioSource ReloadAudioSource;
    public Camera GameCamera;
    public WaveManager WManager;

    private GameObject effectsContainer;
    public HealthBarBehaviour healthBarBehaviour;
    public AmmoBarBehaviour ammoBarBehaviour;
    private float damageVisionEffectTimer = 0.0f;
    private float reloadingTimer;
    private float currentHealth;
    private float currentAmmo;
    private Vector3 currentRotation;

    // Start is called before the first frame update
    void Start()
    {
        effectsContainer = FindObjectOfType<EffectsContainerBehaviour>().gameObject;
        healthBarBehaviour = FindObjectOfType<HealthBarBehaviour>();
        ammoBarBehaviour = FindObjectOfType<AmmoBarBehaviour>();
        WManager = FindObjectOfType<WaveManager>();
        currentHealth = MaxHealth;
        currentAmmo = MaxAmmo;
        currentRotation = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Alive)
        {
            ShootUpdate();
            PlayerUpdate();
            CameraUpdate();
        }
    }

    void ShootUpdate()
    {
        if (Crosshairs)
        {
            Crosshairs.transform.position = InputManager.Instance.CursorLocation;
        }

        if (InputManager.Instance.FireInput)
        {
            if(currentAmmo > 0)
            {
                currentAmmo--;
                if (ammoBarBehaviour)
                {
                    ammoBarBehaviour.SetAmmo(currentAmmo);
                }
                
                RaycastHit hit;
                Ray rayFromCursor = GameCamera.ScreenPointToRay(InputManager.Instance.CursorLocation);
                EnemyBehaviour hitEnemy;
                if (Physics.Raycast(rayFromCursor, out hit, Range))
                {
                    if (ShotAudioSource)
                    {
                        ShotAudioSource.PlayOneShot(ShotAudioSource.clip);
                    }
                    GameObject effect = Instantiate(BulletImpactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    if (effectsContainer)
                    {
                        effect.transform.parent = effectsContainer.transform;
                    }


                    hitEnemy = hit.collider.GetComponent<EnemyBehaviour>();

                    if (hitEnemy != null && hitEnemy.IsAlive())
                    {
                        hitEnemy.TakeDamage(Damage);
                    }
                }
            }
        }

        if(currentAmmo <= 0)
        {
            reloadingTimer += Time.deltaTime;

            if (reloadingTimer > ReloadingTime)
            {
                if (ReloadAudioSource)
                {
                    ReloadAudioSource.PlayOneShot(ReloadAudioSource.clip);
                }

                currentAmmo = MaxAmmo;
                reloadingTimer = 0;
            }
            //Reload
            ammoBarBehaviour.SetAmmo(currentAmmo);
        }
    }

    void PlayerUpdate()
    {
        if(damageVisionEffectTimer != 0)
        {
            if(damageVisionEffectTimer > DamageVisionEffectTime)
            {
                damageVisionEffectTimer = 0.0f;
                DamageVisionPanel.SetActive(false);
            }
            else
            {
                damageVisionEffectTimer += Time.deltaTime;
            }
        }

        if (healthBarBehaviour)
        {
            healthBarBehaviour.Health = currentHealth / MaxHealth;
        }
    }

    void CameraUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentRotation.y = -RotationRate;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            currentRotation.y = RotationRate;
        }
        else
        {
            currentRotation.y = 0.0f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            currentRotation.x = RotationRate;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            currentRotation.x = -RotationRate;
        }
        else
        {
            currentRotation.x = 0.0f;
        }

        //Apply the rotation
        transform.Rotate(currentRotation);

    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (DamageVisionPanel)
        {
            damageVisionEffectTimer = Time.deltaTime;
            DamageVisionPanel.SetActive(true);
        }

        if (TakeDamageAudioSource)
        {
            TakeDamageAudioSource.PlayOneShot(TakeDamageAudioSource.clip);
        }
        
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Alive = false;
    }
}
