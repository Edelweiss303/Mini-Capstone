
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
    public string TakeDamageSoundEffectName;
    public HealthBarBehaviour healthBarBehaviour;

    private float damageVisionEffectTimer = 0.0f;
    private float currentHealth;
    private Vector3 currentRotation;

    void Start()
    {
        healthBarBehaviour = FindObjectOfType<HealthBarBehaviour>();
        
        currentHealth = MaxHealth;
        
        currentRotation = new Vector3(0, 0, 0);
    }

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
        Camera.main.transform.position = transform.position;
        Camera.main.transform.rotation = transform.rotation;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (DamageVisionPanel)
        {
            damageVisionEffectTimer = Time.deltaTime;
            DamageVisionPanel.SetActive(true);
        }

        AudioManager.Instance.PlaySound(TakeDamageSoundEffectName);
        
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
