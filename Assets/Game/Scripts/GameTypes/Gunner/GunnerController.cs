using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static EnemyBase;

public class GunnerController : Singleton<GunnerController>
{
    public enum PlayerType
    {
        Friend, Game
    }

    public PlayerType playerType = PlayerType.Game;

    public bool GameStatus = true;
    public GameObject PauseMenu;
    public Vector2 ScreenSize;
    public Text InputTest;
    public LayerMask PlayerProjectileMask, EnemyLayerMask;
    public List<EnemyColour> AllEnemyColours = new List<EnemyColour>() { EnemyColour.A, EnemyColour.B, EnemyColour.C };
    public GameObject PickupsShellPrefab;
    public Dictionary<float, GameObject> PickupsMap = new Dictionary<float, GameObject>();
    public string PlayerMovementSoundEffectName, PlayerRotationSoundEffectName, PickupSoundEffectName;

    public List<Transform> FactoryMarkers = new List<Transform>();
    public GameObject ProjectilesCollection, EffectsContainer;
    public GameObject PlayerObject;

    private PlayerShootingBehaviour shootingBehaviour;
    private bool isMoving = false;

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        GameStatus = false;
    }

    public void Start()
    {
        isMoving = false;
        switch (playerType)
        {
            case PlayerType.Friend:
                PlayerObject = FindObjectOfType<FriendController>().gameObject;
                
                break;
            case PlayerType.Game:
                PlayerObject = FindObjectOfType<PlayerBehaviour>().gameObject;
                shootingBehaviour = PlayerObject.GetComponent<PlayerBehaviour>().shootBehaviour;
                break;
        }
    }

    public void Update()
    {
        GameUpdate();
    }

    void GameUpdate()
    {
        if (PauseMenu)
        {
            if (InputManager.Instance.Escape)
            {
                if (GameStatus)
                {
                    Time.timeScale = 0.0f;
                    GameStatus = false;
                    PauseMenu.SetActive(true);
                }
                else
                {
                    QuitGame();
                }
            }
            else if (GameStatus)
            {
                PauseMenu.SetActive(false);
                Time.timeScale = 1.0f;
            }
        }

        if (InputTest)
        {
            //InputTest.text = "Rotation Rate: " + (int)InputManager.Instance.CursorMovement.x + ", " + (int)InputManager.Instance.CursorMovement.y + ", " + (int)InputManager.Instance.CursorMovement.z;
        }

    }

    public void UpdatePlayer(string[] messageSegments)
    {
        float x = float.Parse(messageSegments[2]), y = float.Parse(messageSegments[3]), z = float.Parse(messageSegments[4]);
        float rotation = float.Parse(messageSegments[5]);
        Vector3 newPosition = new Vector3(x, y, z);

        Debug.Log((newPosition - PlayerObject.transform.position).magnitude);
        if((newPosition - PlayerObject.transform.position).magnitude > 0.25f && !isMoving)
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
        GameObject newPickupShell = Instantiate(PickupsShellPrefab, pickupPosition, Quaternion.identity);
        newPickupShell.GetComponent<Shell>().ChangeColour(ammoColour);
        PickupsMap.Add(float.Parse(messageSegments[3]), newPickupShell);
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

}
