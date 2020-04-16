
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public PlayerShootingBehaviour shootBehaviour;
    public PlayerScreen PlayerViewScreen;
    public float MaxHealth = 100.0f;
    public float RotationRate = 0.01f;
    public bool Alive = true;
    public Vector3 Velocity;
    public HealthBarBehaviour healthBarBehaviour;

    public float CurrentHealth;
    private Vector3 currentRotation;

    void Start()
    {
        healthBarBehaviour = FindObjectOfType<HealthBarBehaviour>();
        
        CurrentHealth = MaxHealth;
        
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

        if (healthBarBehaviour)
        {
            healthBarBehaviour.Health = CurrentHealth / MaxHealth;
        }
    }

    void CameraUpdate()
    {
        Camera.main.transform.position = transform.position;
        Camera.main.transform.rotation = transform.rotation;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        PlayerViewScreen.SetDamageScreen();

        GameNetwork.Instance.BroadcastQueue.Add("GunnerTakeDamage:" + CurrentHealth);

        if(CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Alive = false;
        GunnerController.Instance.GameOver();
    }
}
