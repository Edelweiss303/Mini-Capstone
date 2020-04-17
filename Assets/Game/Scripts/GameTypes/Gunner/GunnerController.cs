using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static EnemyBase;

public class GunnerController : Singleton<GunnerController>
{
    public Vector2 ScreenSize;
    public Text ScoreText;
    public LayerMask PlayerProjectileMask, EnemyLayerMask;
    public List<EnemyColour> AllEnemyColours = new List<EnemyColour>() { EnemyColour.A, EnemyColour.B, EnemyColour.C };
    public GameObject PickupsShellPrefab;
    public Dictionary<float, GameObject> PickupsMap = new Dictionary<float, GameObject>();
    public string PlayerMovementSoundEffectName, PlayerRotationSoundEffectName, PickupSoundEffectName;
    public int PlayerScore = 0;

    public List<Transform> FactoryMarkers = new List<Transform>();
    public GameObject ProjectilesCollection, EffectsContainer;
    public GameObject PlayerObject;
    public PlayerBehaviour PBehaviour;
    public GameObject GameOverPanel;
    

    private PlayerShootingBehaviour shootingBehaviour;
    private bool isMoving = false;

    public void Start()
    {
        isMoving = false;
        PBehaviour = FindObjectOfType<PlayerBehaviour>();
        PlayerObject = PBehaviour.gameObject;
        shootingBehaviour = PlayerObject.GetComponent<PlayerBehaviour>().shootBehaviour;
    }

    public void UpdatePlayer(string[] messageSegments)
    {
        float x = float.Parse(messageSegments[2]), y = float.Parse(messageSegments[3]), z = float.Parse(messageSegments[4]);
        float rotation = float.Parse(messageSegments[5]);
        Vector3 newPosition = new Vector3(x, y, z);
        PBehaviour.Velocity = newPosition - PlayerObject.transform.position;
        if ((newPosition - PlayerObject.transform.position).magnitude > 0.25f && !isMoving)
        {
            isMoving = true;
            AudioManager.Instance.PlaySound(PlayerMovementSoundEffectName);
        }
        else if(isMoving && (newPosition - PlayerObject.transform.position).magnitude == 0.0f)
        {
            isMoving = false;
            AudioManager.Instance.StopSound(PlayerMovementSoundEffectName);
        }
        PlayerObject.transform.position = newPosition;

        if(Mathf.Abs(PlayerObject.transform.eulerAngles.y - rotation) > 10)
        {
            AudioManager.Instance.PlaySound(PlayerRotationSoundEffectName);
        }
        PlayerObject.transform.eulerAngles = new Vector3(PlayerObject.transform.eulerAngles.x, rotation, PlayerObject.transform.eulerAngles.z);
    }

    public EnemyColour GetRandomizedEnemyColour()
    {
        return AllEnemyColours[Random.Range(0, AllEnemyColours.Count)];
    }

    public void UpdateAmmo(string[] messageSegments)
    {
        EnemyColour colourToIncrease;
        int amountToIncrease = int.Parse(messageSegments[4]);
        float pickupID = float.Parse(messageSegments[3]);
        switch (messageSegments[2])
        {
            case "B":
                colourToIncrease = EnemyColour.B;
                break;
            case "C":
                colourToIncrease = EnemyColour.C;
                break;
            default:
                colourToIncrease = EnemyColour.A;
                break;
        }
        shootingBehaviour.increaseAmmo(colourToIncrease, amountToIncrease);
        AudioManager.Instance.PlaySound(PickupSoundEffectName);
        if (PickupsMap.ContainsKey(pickupID))
        {
            Destroy(PickupsMap[pickupID]);
            PickupsMap.Remove(pickupID);
        }
    }

    public void AddAmmoPickup(string[] messageSegments)
    {
        EnemyColour ammoColour;
        switch (messageSegments[2])
        {
            case "B":
                ammoColour = EnemyColour.B;
                break;
            case "C":
                ammoColour = EnemyColour.C;
                break;
            default:
                ammoColour = EnemyColour.A;
                break;
        }

        Vector3 pickupPosition = new Vector3(float.Parse(messageSegments[4]), float.Parse(messageSegments[5]), float.Parse(messageSegments[6]));
        SpawnedAsset spawnDetails = new SpawnedAsset(pickupPosition, Quaternion.identity, null);
        spawnDetails.Tag = AddressablesManager.Addressable_Tag.ammo_pickup_shell;
        spawnDetails.Colour = ammoColour;
        spawnDetails.GameID = int.Parse(messageSegments[3]);
        AddressablesManager.Instance.Spawn(spawnDetails);

        //GameObject newPickupShell = Instantiate(PickupsShellPrefab, pickupPosition, Quaternion.identity);
        //newPickupShell.GetComponent<Shell>().ChangeColour(ammoColour);
        //PickupsMap.Add(float.Parse(messageSegments[3]), newPickupShell);
    }

    public void SetPowerup(string[] messageSegments)
    {
        EnemyBase.EnemyColour powerupColour;
        switch (messageSegments[2])
        {
            case "r":
                powerupColour = EnemyColour.A;
                break;
            case "b":
                powerupColour = EnemyColour.B;
                break;
            case "g":
                powerupColour = EnemyColour.C;
                break;
            default:
                return;
        }
        shootingBehaviour.SetPowerup(powerupColour, 2);
    }

    public void GameOver()
    {
        //Time.timeScale = 0.0f;
        GameOverPanel.SetActive(true);
        AudioManager.Instance.StopAll();
        GameNetwork.Instance.BroadcastQueue.Add("GunnerGameOver");
    }

    public void IncreaseScore(int inScoreIncrement)
    {
        PlayerScore += inScoreIncrement;
        GameNetwork.Instance.BroadcastQueue.Add("GunnerSetScore:" + PlayerScore);
        ScoreText.text = "SCORE: " + PlayerScore;
    }

    public void RepairDamage(int damageToRepair)
    {
        if((PBehaviour.CurrentHealth + damageToRepair) > PBehaviour.MaxHealth)
        {
            PBehaviour.CurrentHealth = PBehaviour.MaxHealth;
        }
        else
        {
            PBehaviour.CurrentHealth += damageToRepair;
        }

        PBehaviour.healthBarBehaviour.Health = PBehaviour.CurrentHealth / 15;
        AudioManager.Instance.PlaySound("Player_Repair");
        GameNetwork.Instance.BroadcastQueue.Add("GunnerRepairDamage:" + PBehaviour.CurrentHealth);
    }
}
