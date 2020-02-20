using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootingBehaviour : MonoBehaviour
{
    public GameObject Crosshairs;
    public float Range = 100.0f;
    public float AutoAimRange = 25.0f;
    public Camera GameCamera;

    public AmmoBarBehaviour ammoBarBehaviour;
    public float MaxAmmo = 24.0f;
    public float ReloadingTime = 1.5f;
    public GameObject BulletImpactEffectPrefab;
    public AudioSource ShotAudioSource, ReloadAudioSource;
    public float Damage = 1.0f;
    public float AutoAimStrength = 3.0f;
    public float AutoAimWeight = 0.75f;

    private GameObject effectsContainer;
    private float currentAmmo;
    private float reloadingTimer;
    private bool isReloading = false;
    private LineRenderer currentAimTrajectory;

    public void Start()
    {
        effectsContainer = FindObjectOfType<EffectsContainerBehaviour>().gameObject;
        ammoBarBehaviour = FindObjectOfType<AmmoBarBehaviour>();
        currentAmmo = MaxAmmo;
        currentAimTrajectory = GetComponent<LineRenderer>();
    }

    public void ShootUpdate()
    {
        RaycastHit hit;
        Ray rayFromCursor;
        Vector3 autoAimDirection = Vector3.zero;
        EnemyBehaviour eBehaviour = null;
        if (Crosshairs)
        {
            rayFromCursor = GameCamera.ScreenPointToRay(Crosshairs.transform.position);

            if (Physics.Raycast(rayFromCursor, out hit, Range))
            {
                Ray line = new Ray(rayFromCursor.origin, hit.point - rayFromCursor.origin);
                eBehaviour = EnemiesManager.Instance.GetClosestEnemy(line, AutoAimRange, out autoAimDirection);

                if (InputManager.Instance.FireInput)
                {
                    if (currentAmmo > 0)
                    {
                        currentAmmo--;
                        if (ammoBarBehaviour)
                        {
                            ammoBarBehaviour.SetAmmo(currentAmmo);
                        }

                        EnemyBehaviour hitEnemy;

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

            if (eBehaviour)
            {
                //Do a ray to the enemy?
                Vector3 autoAimTarget = Crosshairs.transform.position + autoAimDirection * AutoAimStrength;
                Crosshairs.transform.position = Vector3.Lerp(Crosshairs.transform.position, InputManager.Instance.CursorLocation, 0.2f);
                Crosshairs.transform.position = Vector3.Lerp(Crosshairs.transform.position, autoAimTarget, AutoAimWeight);
            }
            else
            {
                Crosshairs.transform.position = InputManager.Instance.CursorLocation;
            }
        }

        if ((InputManager.Instance.Reloading && !isReloading) || currentAmmo == 0)
        {
            isReloading = true;
            currentAmmo = 0;
            ammoBarBehaviour.SetAmmo(0);
        }
        if(isReloading)
        {
            reloadingTimer += Time.deltaTime;
            if (reloadingTimer > ReloadingTime && !InputManager.Instance.Reloading)
            {
                if (ReloadAudioSource)
                {
                    ReloadAudioSource.PlayOneShot(ReloadAudioSource.clip);
                }

                currentAmmo = MaxAmmo;
                reloadingTimer = 0;
                isReloading = false;

                //Reload
                ammoBarBehaviour.SetAmmo(currentAmmo);
            }

        }
    }
}
