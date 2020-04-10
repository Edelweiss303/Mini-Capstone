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

    private EnemyBase.EnemyColour currentEnemyColour;
    private Dictionary<EnemyBase.EnemyColour, int> ammoTypes = new Dictionary<EnemyBase.EnemyColour, int>();
    private List<EnemyBase.EnemyColour> enemyColours = new List<EnemyBase.EnemyColour>()
                                                        { EnemyBase.EnemyColour.A, EnemyBase.EnemyColour.B, EnemyBase.EnemyColour.C };
    private int currentEnemyIndex;
    private float reloadingTimer;
    private bool isReloading = false;
    private LineRenderer currentAimTrajectory;

    public void Start()
    {
        effectsContainer = GunnerController.Instance.EffectsContainer;
        ammoBarBehaviour = FindObjectOfType<AmmoBarBehaviour>();
        currentEnemyColour = EnemyBase.EnemyColour.A;
        currentEnemyIndex = 0;
        ammoTypes.Add(EnemyBase.EnemyColour.A, MaxAmmo);
        ammoTypes.Add(EnemyBase.EnemyColour.B, MaxAmmo);
        ammoTypes.Add(EnemyBase.EnemyColour.C, MaxAmmo);

        currentAimTrajectory = GetComponent<LineRenderer>();
        originalCrosshairPositionReference = Crosshairs.transform.position;
        totalAutoAimMovement = originalCrosshairPositionReference;
        ammoBarBehaviour.ChangeAmmoType(currentEnemyColour);
        ammoBarBehaviour.SetAmmo(ammoTypes[currentEnemyColour]);
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
                if(currentEnemyIndex >= enemyColours.Count)
                {
                    currentEnemyIndex = 0;
                }
                currentEnemyColour = enemyColours[currentEnemyIndex];
                ammoBarBehaviour.ChangeAmmoType(currentEnemyColour);
                ammoBarBehaviour.SetAmmo(ammoTypes[currentEnemyColour]);
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
            if (ammoTypes[currentEnemyColour] > 0)
            {
                ammoTypes[currentEnemyColour]--;
                if (ammoBarBehaviour)
                {
                    ammoBarBehaviour.SetAmmo(ammoTypes[currentEnemyColour]);
                }

                if (Physics.Raycast(rayFromCursor, out hit, Range))
                {

                    EnemyBase hitEnemy;
                    if (ShotAudioSource)
                    {
                        ShotAudioSource.PlayOneShot(ShotAudioSource.clip);
                    }
                    GameObject effect = Instantiate(BulletImpactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    effect.GetComponent<Renderer>().material = ColourManager.Instance.EnemyMaterialMap[currentEnemyColour];
                    if (effectsContainer)
                    {
                        effect.transform.parent = effectsContainer.transform;
                    }

                    hitEnemy = hit.collider.GetComponent<EnemyBase>();

                    if (hitEnemy != null && hitEnemy.IsAlive() && currentEnemyColour == hitEnemy.Colour)
                    {
                        hitEnemy.TakeDamage(Damage);
                        return;
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
        if (ammoTypes[currentEnemyColour] == 0)
        {
            isReloading = true;
            ammoTypes[currentEnemyColour] = 0;
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

                ammoTypes[currentEnemyColour] = MaxAmmo;
                reloadingTimer = 0;
                isReloading = false;

                //Reload
                ammoBarBehaviour.SetAmmo(ammoTypes[currentEnemyColour]);
            }
        }

    }

    public void increaseAmmo(EnemyBase.EnemyColour enemyColour, int amount)
    {
        ammoTypes[enemyColour] += amount;
        if(ammoTypes[enemyColour] > MaxAmmo)
        {
            ammoTypes[enemyColour] = MaxAmmo;
        }

        if(enemyColour == currentEnemyColour)
        {
            ammoBarBehaviour.SetAmmo(ammoTypes[currentEnemyColour]);
        }
    }
}
