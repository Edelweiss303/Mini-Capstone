
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{


    public PlayerShootingBehaviour shootBehaviour;

    public GameObject DamageVisionPanel;

    public float MaxHealth = 100.0f;

    public float DamageVisionEffectTime = 1.0f;

    public float RotationRate = 0.01f;
    public bool Alive = true;

    public AudioSource TakeDamageAudioSource;
    public HealthBarBehaviour healthBarBehaviour;

    private float damageVisionEffectTimer = 0.0f;
    private float currentHealth;

    private Vector3 currentRotation;

    // Start is called before the first frame update
    void Start()
    {
        healthBarBehaviour = FindObjectOfType<HealthBarBehaviour>();
        
        currentHealth = MaxHealth;
        
        currentRotation = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Alive)
        {
            PlayerUpdate();
            CameraUpdate();
        }
    }

    void PlayerUpdate()
    {
        if (shootBehaviour)
        {
            shootBehaviour.ShootUpdate();
        }

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
        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    currentRotation.y = -RotationRate;
        //}
        //else if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    currentRotation.y = RotationRate;
        //}
        //else
        //{
        //    currentRotation.y = 0.0f;
        //}

        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    currentRotation.x = RotationRate;
        //}
        //else if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    currentRotation.x = -RotationRate;
        //}
        //else
        //{
        //    currentRotation.x = 0.0f;
        //}

        ////Apply the rotation
        //transform.Rotate(currentRotation);

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
        GunnerController.Instance.QuitGame();
    }
}
