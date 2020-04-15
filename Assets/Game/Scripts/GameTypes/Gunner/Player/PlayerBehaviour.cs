
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


    [SerializeField]
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
        PlayerViewScreen.SetDamageScreen();

        GameNetwork.Instance.BroadcastQueue.Add("GunnerTakeDamage:" + currentHealth);

        if(currentHealth <= 0)
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
