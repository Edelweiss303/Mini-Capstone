using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShootingBehaviour : MonoBehaviour
{
    public GameObject Crosshairs;
    public float Range = 100.0f;
    public Camera GameCamera;

    public AmmoBarBehaviour ammoBarBehaviour;
    public int MaxAmmo = 24;

    public GameObject BulletImpactEffectPrefab;
    public string ShootingSoundEffectName = "Player_Shoot", PlayerPowerupSoundEffectName;
    public int BaseDamage = 1;
    public float DeadZone = 0.1f;
    public float CrosshairSpeed = 500.0f;
    public float ShootHeatGeneration = 0.25f;
    public float BeamFiringTimeLimit = 8.0f;
    public bool IsOverheated = false;
    public float PowerupTime = 45.0f;
    public Slider PowerupSlider;
    public Image PowerupFill;
    public int NumberOfEnemiesToKillForPowerBeam = 5;
    public LineRenderer PowerBeamLine;
    public GameObject PowerBeamImpactEffectPrefab;
    public float SwipingCooldownTime = 0.1f;

    public Text RedProgressText, GreenProgressText, BlueProgressText;

    private Vector3 originalCrosshairPositionReference;
    private Vector3 totalAutoAimMovement;

    private GameObject effectsContainer;

    private EnemyBase.EnemyColour currentEnemyColour;
    private Dictionary<EnemyBase.EnemyColour, int> ammoAmounts = new Dictionary<EnemyBase.EnemyColour, int>();
    private List<EnemyBase.EnemyColour> enemyColours = new List<EnemyBase.EnemyColour>()
                                                        { EnemyBase.EnemyColour.A, EnemyBase.EnemyColour.B, EnemyBase.EnemyColour.C };
    private Dictionary<EnemyBase.EnemyColour, int> ammoDamage = new Dictionary<EnemyBase.EnemyColour, int>();

    private Dictionary<EnemyBase.EnemyColour, int> powerBeamProgress = new Dictionary<EnemyBase.EnemyColour, int>() { { EnemyBase.EnemyColour.A, 0 }, { EnemyBase.EnemyColour.B, 0 }, { EnemyBase.EnemyColour.C, 0 } };

    private EnemyBase.EnemyColour powerupColour;
    private int currentEnemyIndex;
    private bool isPowerBeamActive = false, playerHasPowerBeam = true;
    private float currentPowerupTimeRemaining = 0.0f;
    private Vector3 beamOrigin;
    private float beamOscillationTimeThreshold = 0.05f;
    private float timeSinceLastBeamOscillation = 0.0f;
    private float timeBeamHasBeenActive = 0.0f;
    private bool isNewSwipe = true;
    private float timeSinceLastSwipe = 0.0f;
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
            if (isPowerBeamActive)
            {
                AimBeam();
            }
            else
            {
                Shoot();
                if (InputManager.Instance.DoubleCenterTap)
                {
                    //Crosshairs.transform.position = originalCrosshairPositionReference;
                }

                if (InputManager.Instance.Swiping && (isNewSwipe || InputManager.InputMode.KeyAndMouse == InputManager.Instance.inputMode))
                {
                    isNewSwipe = false;
                    currentEnemyIndex++;
                    if (currentEnemyIndex >= enemyColours.Count)
                    {
                        currentEnemyIndex = 0;
                    }
                    currentEnemyColour = enemyColours[currentEnemyIndex];
                    ammoBarBehaviour.ChangeAmmoType(currentEnemyColour);
                    ammoBarBehaviour.SetAmmo(ammoAmounts[currentEnemyColour]);
                }
                else if((Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended) || Input.touches.Length == 0)
                {
                    timeSinceLastSwipe = 0.0f;
                }
                else if (timeSinceLastSwipe > SwipingCooldownTime)
                {
                    isNewSwipe = true;
                }
                timeSinceLastSwipe += Time.deltaTime;

                

                if (currentPowerupTimeRemaining > 0.0f)
                {
                    PowerupUpdate();
                }

                if(InputManager.Instance.FireHeldSeconds > 2.5f && playerHasPowerBeam)
                {
                    PowerBeamLine.gameObject.SetActive(true);
                    isPowerBeamActive = true;
                    playerHasPowerBeam = false;
                }
            }
            MoveCursor();


        }
    }

    private void AimBeam()
    {
        RaycastHit hit;
        Ray rayFromCursor = GameCamera.ScreenPointToRay(Crosshairs.transform.position);
        if (Physics.Raycast(rayFromCursor, out hit, Range))
        {
            EnemyBase hitEnemy;

            hitEnemy = hit.collider.GetComponent<EnemyBase>();

            if (hitEnemy != null && hitEnemy.IsAlive())
            {
                hitEnemy.TakeDamage(ammoDamage[currentEnemyColour]);
                return;
            }
            else if(hitEnemy == null)
            {
                hitEnemy = hit.collider.GetComponentInParent<EnemyBase>();
                if (hitEnemy != null && hitEnemy.IsAlive())
                {
                    hitEnemy.TakeDamage(ammoDamage[currentEnemyColour]);
                    return;
                }
            }

            timeSinceLastBeamOscillation += Time.deltaTime;
            if (timeSinceLastBeamOscillation > beamOscillationTimeThreshold)
            {
                beamOrigin = rayFromCursor.origin + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                timeSinceLastBeamOscillation = 0.0f;
                SpawnedAsset spawnDetails = new SpawnedAsset(hit.point, Quaternion.identity, effectsContainer.transform);
                spawnDetails.Tag = AddressablesManager.Addressable_Tag.beam_impact_effect;
                AddressablesManager.Instance.Spawn(spawnDetails);
                //Instantiate(PowerBeamImpactEffectPrefab, hit.point, Quaternion.identity, effectsContainer.transform);
                AudioManager.Instance.PlaySound("Beam");
            }

            PowerBeamLine.SetPosition(0, beamOrigin);
            PowerBeamLine.SetPosition(1, Vector3.Lerp(beamOrigin, hit.point, 0.3f));
            PowerBeamLine.SetPosition(2, Vector3.Lerp(beamOrigin, hit.point, 0.6f));
            PowerBeamLine.SetPosition(3, hit.point);

            timeBeamHasBeenActive += Time.deltaTime;
            if(timeBeamHasBeenActive > BeamFiringTimeLimit)
            {
                timeBeamHasBeenActive = 0.0f;
                isPowerBeamActive = false;
                PowerBeamLine.gameObject.SetActive(false);
                AudioManager.Instance.StopSound("Beam");

                foreach(EnemyBase.EnemyColour enemyColour in enemyColours)
                {
                    powerBeamProgress[enemyColour] = 0;
                }
                RedProgressText.text = "0";
                BlueProgressText.text = "0";
                GreenProgressText.text = "0";
            }
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        Ray rayFromCursor = GameCamera.ScreenPointToRay(Crosshairs.transform.position);
        EnemyBase.EnemyColour hitEnemyColour;
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

                    SpawnedAsset spawnDetails = new SpawnedAsset(hit.point, Quaternion.LookRotation(hit.normal), effectsContainer.transform);
                    spawnDetails.Tag = AddressablesManager.Addressable_Tag.default_hit_effect;
                    spawnDetails.Colour = currentEnemyColour;
                    AddressablesManager.Instance.Spawn(spawnDetails);
                    //GameObject effect = Instantiate(BulletImpactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    //effect.GetComponent<Renderer>().material = ColourManager.Instance.EnemyMaterialMap[currentEnemyColour];
                    //if (effectsContainer)
                    //{
                    //    effect.transform.parent = effectsContainer.transform;
                    //}

                    hitEnemy = hit.collider.GetComponent<EnemyBase>();

                    if(hitEnemy == null)
                    {
                        hitEnemy = hit.collider.GetComponentInParent<EnemyBase>();
                    }

                    if (hitEnemy != null && hitEnemy.IsAlive() && currentEnemyColour == hitEnemy.Colour)
                    {
                        hitEnemyColour = hitEnemy.Colour;
                        hitEnemy.TakeDamage(ammoDamage[currentEnemyColour]);

                        if(hitEnemy == null || !hitEnemy.IsAlive())
                        {
                            powerBeamProgress[hitEnemyColour]++;
                            switch (hitEnemyColour)
                            {
                                case EnemyBase.EnemyColour.A:
                                    RedProgressText.text = powerBeamProgress[hitEnemyColour].ToString();
                                    break;
                                case EnemyBase.EnemyColour.B:
                                    BlueProgressText.text = powerBeamProgress[hitEnemyColour].ToString();
                                    break;
                                case EnemyBase.EnemyColour.C:
                                    GreenProgressText.text = powerBeamProgress[hitEnemyColour].ToString();
                                    break;
                            }

                            isPowerBeamActive = checkForPowerBeam();
                        }
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

    private bool checkForPowerBeam()
    {
        foreach(KeyValuePair<EnemyBase.EnemyColour,int> enemyTypeKilled in powerBeamProgress)
        {
            if(enemyTypeKilled.Value < NumberOfEnemiesToKillForPowerBeam)
            {
                return false;
            }
        }
        return true;
    }
}
