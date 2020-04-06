using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GunnerController : Singleton<GunnerController>
{
    public enum PlayerType
    {
        Friend, Game
    }

    public PlayerType playerType = PlayerType.Friend;

    public enum EnemyType
    {
        A, B, C
    }

    public bool GameStatus = true;
    public GameObject PauseMenu;
    public Vector2 ScreenSize;
    public Text InputTest;
    public LayerMask PlayerProjectileMask, EnemyLayerMask;
    public Material EnemyTypeAMaterial, EnemyTypeBMaterial, EnemyTypeCMaterial;
    public Material ShieldTypeAMaterial, ShieldTypeBMaterial, ShieldTypeCMaterial;

    public Color AmmoColorA, AmmoColorB, AmmoColorC;
    public Dictionary<EnemyType, Material> EnemyMaterialMap = new Dictionary<EnemyType, Material>();
    public Dictionary<EnemyType, Material> ShieldMaterialMap = new Dictionary<EnemyType, Material>();
    public Dictionary<EnemyType, Color> AmmoColorMap = new Dictionary<EnemyType, Color>();
    public List<EnemyType> AllEnemyTypes = new List<EnemyType>() { EnemyType.A, EnemyType.B, EnemyType.C };

    public List<Transform> FactoryMarkers = new List<Transform>();
    public GameObject ProjectilesCollection, EffectsContainer;
    public GameObject PlayerObject;

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        GameStatus = false;
    }

    public void Awake()
    {
        EnemyMaterialMap.Add(EnemyType.A, EnemyTypeAMaterial);
        EnemyMaterialMap.Add(EnemyType.B, EnemyTypeBMaterial);
        EnemyMaterialMap.Add(EnemyType.C, EnemyTypeCMaterial);

        ShieldMaterialMap.Add(EnemyType.A, ShieldTypeAMaterial);
        ShieldMaterialMap.Add(EnemyType.B, ShieldTypeBMaterial);
        ShieldMaterialMap.Add(EnemyType.C, ShieldTypeCMaterial);

        AmmoColorMap.Add(EnemyType.A, AmmoColorA);
        AmmoColorMap.Add(EnemyType.B, AmmoColorB);
        AmmoColorMap.Add(EnemyType.C, AmmoColorC);
    }
    public void Start()
    {
        switch (playerType)
        {
            case PlayerType.Friend:
                PlayerObject = FindObjectOfType<FriendController>().gameObject;
                break;
            case PlayerType.Game:
                PlayerObject = FindObjectOfType<PlayerBehaviour>().gameObject;
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
        PlayerObject.transform.position = new Vector3(x, y, z);
        PlayerObject.transform.eulerAngles = new Vector3(PlayerObject.transform.eulerAngles.x, rotation, PlayerObject.transform.eulerAngles.z);
    }

    public EnemyType GetRandomizedEnemyType()
    {
        return AllEnemyTypes[Random.Range(0, AllEnemyTypes.Count)];
    }

}
