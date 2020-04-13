using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public string ShootingSoundEffectName = "Player_Shoot", PlayerPowerupSoundEffectName;
    public int BaseDamage = 1;
    public float DeadZone = 0.1f;
    public float AutoAimWeight = 0.75f;
    public float CrosshairSpeed = 500.0f;
    public float MaximumAutoAimAdjustment = 3.0f;
    public float ShootHeatGeneration = 0.25f;
    public bool IsOverheated = false;
    public float PowerupTime = 45.0f;
    public Slider PowerupSlider;
    public Image PowerupFill;

    private Vector3 originalCrosshairPositionReference;
    private Vector3 totalAutoAimMovement;

    private GameObject effectsContainer;

    private EnemyBase.EnemyColour currentEnemyColour;
    private Dictionary<EnemyBase.EnemyColour, int> ammoAmounts = new Dictionary<EnemyBase.EnemyColour, int>();
    private List<EnemyBase.EnemyColour> enemyColours = new List<EnemyBase.EnemyColour>()
                                                        { EnemyBase.EnemyColour.A, EnemyBase.EnemyColour.B, EnemyBase.EnemyColour.C };
    private Dictionary<EnemyBase.EnemyColour, int> ammoDamage = new Dictionary<EnemyBase.EnemyColour, int>();

    private EnemyBase.EnemyColour powerupColour;
    private int currentEnemyIndex;
    private float reloadingTimer;
    private bool isReloading = false;
    private LineRenderer currentAimTrajectory;
    private float currentPowerupTimeRemaining = 0.0f;

    public void Start()
    {
        PowerupSlider.maxValue = 45.0f;
        PowerupSlider.value = 0.0f;
        effectsContainer = GunnerController.Instance.EffectsContainer;
        ammoBarBehaviour = FindObjectOfType<AmmoBarBehaviour>();
        currentEnemyColour = EnemyBase.EnemyColour.A;
        currentEnemyIndex = 0;
        ammoAmounts.Add(EnemyBase.EnemyColour.A, MaxAmmo);
        ammoAmounts.Add(EnemyBase.EnemyColour.B, MaxAmmo);
        ammoAmounts.Add(EnemyBase.EnemyColour.C, MaxAmmo);
        ammoDamage.Add(EnemyBase.EnemyColour.A, BaseDamage);
        ammoDamage.Add(EnemyBase.EnemyColour.B, BaseDamage);
        ammoDamage.Add(EnemyBase.EnemyColour.C, BaseDamage);

        currentAimTrajectory = GetComponent<LineRenderer>();
        originalCrosshairPositionReference = Crosshairs.transform.position;
        totalAutoAimMovement = originalCrosshairPositionReference;
        ammoBarBehaviour.ChangeAmmoType(currentEnemyColour);
        ammoBarBehaviour.SetAmmo(ammoAmounts[currentEnemyColour]);
    }

    public void SetPowerup(EnemyBase.EnemyColour colourToPowerup, int powerupDamage)
    {
        foreach(EnemyBase.EnemyColour colour in enemyColours)
        {
            ammoDamage[colour] = BaseDamage;
        }

        AudioManager.Instance.PlaySound(PlayerPowerupSoundEffectName);
        PowerupFill.color = ColourManager.Instance.AmmoColorMap[colourToPowerup];
        currentPowerupTimeRemaining = PowerupTime;
        ammoDamage[colourToPowerup] = powerupDamage;
        powerupColour = colourToPowerup;

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
                ammoBarBehaviour.SetAmmo(ammoAmounts[currentEnemyColour]);
            }
            MoveCursor();

            if(currentPowerupTimeRemaining > 0.0f)
            {
                PowerupUpdate();
            }
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        Ray rayFromCursor = GameCamera.ScreenPointToRay(Crosshairs.transform.position);

        if (InputManager.Instance.FireInput && !IsOverheated)
        {
            if (ammoAmounts[currentEnemyColour] > 0)
            {
                GameNetwork.Instance.ToPlayerQueue.Add("t:TechAddHeat:" + ShootHeatGeneration);
                AudioManager.Instance.PlaySound(ShootingSoundEffectName);
                ammoAmounts[currentEnemyColour]--;
                if (ammoBarBehaviour)
                {
                    ammoBarBehaviour.SetAmmo(ammoAmounts[currentEnemyColour]);
                }

                if (Physics.Raycast(rayFromCursor, out hit, Range))
                {

                    EnemyBase hitEnemy;
                    
                    GameObject effect = Instantiate(BulletImpactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    effect.GetComponent<Renderer>().material = ColourManager.Instance.EnemyMaterialMap[currentEnemyColour];
                    if (effectsContainer)
                    {
                        effect.transform.parent = effectsContainer.transform;
                    }

                    hitEnemy = hit.collider.GetComponent<EnemyBase>();

                    if (hitEnemy != null && hitEnemy.IsAlive() && currentEnemyColour == hitEnemy.Colour)
                    {
                        hitEnemy.TakeDamage(ammoDamage[currentEnemyColour]);
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

    public void increaseAmmo(EnemyBase.EnemyColour enemyColour, int amount)
    {
        ammoAmounts[enemyColour] += amount;
        if(ammoAmounts[enemyColour] > MaxAmmo)
        {
            ammoAmounts[enemyColour] = MaxAmmo;
        }

        if(enemyColour == currentEnemyColour)
        {
            ammoBarBehaviour.SetAmmo(ammoAmounts[currentEnemyColour]);
        }
    }

    private void PowerupUpdate()
    {
        PowerupSlider.value = currentPowerupTimeRemaining;
        currentPowerupTimeRemaining -= Time.deltaTime;
        if (currentPowerupTimeRemaining <= 0.0f)
        {
            currentPowerupTimeRemaining = 0.0f;
            ammoDamage[powerupColour] = BaseDamage;
        }
    }
}
