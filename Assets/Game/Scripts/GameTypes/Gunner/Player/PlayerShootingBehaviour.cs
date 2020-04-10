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
    public int MaxAmmo = 24;
    public float ReloadingTime = 1.5f;
    public GameObject BulletImpactEffectPrefab;
    public AudioSource ShotAudioSource, ReloadAudioSource;
    public float Damage = 1.0f;
    public float DeadZone = 0.1f;
    public float AutoAimWeight = 0.75f;
    public float CrosshairSpeed = 500.0f;
    public float MaximumAutoAimAdjustment = 3.0f;

    private Vector3 originalCrosshairPositionReference;
    private Vector3 totalAutoAimMovement;

    private GameObject effectsContainer;

    private GunnerController.EnemyType currentEnemyType;
    private Dictionary<GunnerController.EnemyType, int> ammoTypes = new Dictionary<GunnerController.EnemyType, int>();
    private List<GunnerController.EnemyType> enemyTypes = new List<GunnerController.EnemyType>()
                                                        { GunnerController.EnemyType.A, GunnerController.EnemyType.B, GunnerController.EnemyType.C };
    private int currentEnemyIndex;
    private float reloadingTimer;
    private bool isReloading = false;
    private LineRenderer currentAimTrajectory;

    public void Start()
    {
        effectsContainer = GunnerController.Instance.EffectsContainer;
        ammoBarBehaviour = FindObjectOfType<AmmoBarBehaviour>();
        currentEnemyType = GunnerController.EnemyType.A;
        currentEnemyIndex = 0;
        ammoTypes.Add(GunnerController.EnemyType.A, MaxAmmo);
        ammoTypes.Add(GunnerController.EnemyType.B, MaxAmmo);
        ammoTypes.Add(GunnerController.EnemyType.C, MaxAmmo);

        currentAimTrajectory = GetComponent<LineRenderer>();
        originalCrosshairPositionReference = Crosshairs.transform.position;
        totalAutoAimMovement = originalCrosshairPositionReference;
        ammoBarBehaviour.ChangeAmmoType(currentEnemyType);
        ammoBarBehaviour.SetAmmo(ammoTypes[currentEnemyType]);
    }

    public void ShootUpdate()
    {
        if (Crosshairs)
        {
            Shoot();
            if (InputManager.Instance.DoubleCenterTap)
            {
                Crosshairs.transform.position = originalCrosshairPositionReference;
            }
            if (InputManager.Instance.Swiping)
            {
                currentEnemyIndex++;
                if(currentEnemyIndex >= enemyTypes.Count)
                {
                    currentEnemyIndex = 0;
                }
                currentEnemyType = enemyTypes[currentEnemyIndex];
                ammoBarBehaviour.ChangeAmmoType(currentEnemyType);
                ammoBarBehaviour.SetAmmo(ammoTypes[currentEnemyType]);
            }
            MoveCursor();
        }
        //Reload();

    }

    private void Shoot()
    {
        RaycastHit hit;
        Ray rayFromCursor = GameCamera.ScreenPointToRay(Crosshairs.transform.position);

        if (InputManager.Instance.FireInput)
        {
            if (ammoTypes[currentEnemyType] > 0)
            {
                ammoTypes[currentEnemyType]--;
                if (ammoBarBehaviour)
                {
                    ammoBarBehaviour.SetAmmo(ammoTypes[currentEnemyType]);
                }

                if (Physics.Raycast(rayFromCursor, out hit, Range))
                {

                    EnemyBase hitEnemy;

                    if (ShotAudioSource)
                    {
                        ShotAudioSource.PlayOneShot(ShotAudioSource.clip);
                    }
                    GameObject effect = Instantiate(BulletImpactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    effect.GetComponent<Renderer>().material = GunnerController.Instance.EnemyMaterialMap[currentEnemyType];
                    if (effectsContainer)
                    {
                        effect.transform.parent = effectsContainer.transform;
                    }

                    hitEnemy = hit.collider.GetComponent<EnemyBase>();

                    if (hitEnemy != null && hitEnemy.IsAlive() && currentEnemyType == hitEnemy.Type)
                    {
                        hitEnemy.TakeDamage(Damage);
                    }

                }

            }

        }


    }

    private void MoveCursor()
    {
        Vector3 movementToApply = new Vector3(InputManager.Instance.CursorMovement.x * Screen.width * CrosshairSpeed, InputManager.Instance.CursorMovement.y * Screen.height * CrosshairSpeed, 0);
        if (movementToApply.magnitude < DeadZone)
        {
            movementToApply = Vector3.zero;
        }
        else
        {
            if (Crosshairs.transform.position.x > Screen.width && movementToApply.x > 0)
            {
                movementToApply.x = 0;
            }
            else if (Crosshairs.transform.position.x < 0 && movementToApply.x < 0)
            {
                movementToApply.x = 0;
            }

            if (Crosshairs.transform.position.y > Screen.height && movementToApply.y > 0)
            {
                movementToApply.y = 0;
            }
            else if (Crosshairs.transform.position.y < 0 && movementToApply.y < 0)
            {
                movementToApply.y = 0;
            }
        }

        Crosshairs.transform.position += movementToApply * Time.deltaTime * CrosshairSpeed;
    }

    private void Reload()
    {
        if (ammoTypes[currentEnemyType] == 0)
        {
            isReloading = true;
            ammoTypes[currentEnemyType] = 0;
            ammoBarBehaviour.SetAmmo(0);
        }
        if (isReloading)
        {
            reloadingTimer += Time.deltaTime;
            if (reloadingTimer > ReloadingTime)
            {
                if (ReloadAudioSource)
                {
                    ReloadAudioSource.PlayOneShot(ReloadAudioSource.clip);
                }

                ammoTypes[currentEnemyType] = MaxAmmo;
                reloadingTimer = 0;
                isReloading = false;

                //Reload
                ammoBarBehaviour.SetAmmo(ammoTypes[currentEnemyType]);
            }
        }

    }

    public void increaseAmmo(GunnerController.EnemyType enemyType, int amount)
    {
        ammoTypes[enemyType] += amount;
        if(ammoTypes[enemyType] > MaxAmmo)
        {
            ammoTypes[enemyType] = MaxAmmo;
        }

        if(enemyType == currentEnemyType)
        {
            ammoBarBehaviour.SetAmmo(ammoTypes[currentEnemyType]);
        }
    }
}
