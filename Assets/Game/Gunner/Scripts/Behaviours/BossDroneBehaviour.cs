using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDroneBehaviour : EnemyBehaviour
{
    public float Speed = 2.0f;
    public float Health = 1.0f;
    public float Damage = 1.0f;
    public float DeathTime = 4.0f;
    public bool Alive = true;

    public GameObject ExplosionPrefab;
    public AudioSource DeathAudioSource;

    private Transform playerTransform;
    private float totalDyingTime = 0.0f;

    private MeshRenderer enemyRenderer;
    private BoxCollider enemyCollider;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = FindObjectOfType<PlayerBehaviour>().gameObject.transform;
        enemyRenderer = GetComponent<MeshRenderer>();
        enemyCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Alive)
        {
            EnemyMoveUpdate();
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

    private void EnemyMoveUpdate()
    {
        if (playerTransform)
        {
            transform.LookAt(playerTransform);

            Vector3 jitter = new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
            transform.position += transform.forward * Speed * Time.deltaTime + jitter;
        }
    }

    override public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Die();
        }
    }

    public override bool IsAlive()
    {
        return Alive;
    }

    public void Die()
    {
        Alive = false;
        enemyCollider.enabled = false;
        if (enemyRenderer)
        {
            enemyRenderer.enabled = false;
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

    public void OnTriggerEnter(Collider collider)
    {
        PlayerBehaviour pBehaviour = collider.gameObject.GetComponent<PlayerBehaviour>();
        if (pBehaviour)
        {
            pBehaviour.TakeDamage(Damage);
            Die();
        }
    }
}
