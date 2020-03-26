using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
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
    public Dictionary<EnemyType, Material> EnemyMaterialMap = new Dictionary<EnemyType, Material>();
    public Dictionary<EnemyType, Material> ShieldMaterialMap = new Dictionary<EnemyType, Material>();
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
            InputTest.text = "Rotation Rate: " + (int)InputManager.Instance.CursorMovement.x + ", " + (int)InputManager.Instance.CursorMovement.y + ", " + (int)InputManager.Instance.CursorMovement.z;
        }

    }

    public EnemyType GetRandomizedEnemyType()
    {
        return AllEnemyTypes[Random.Range(0, AllEnemyTypes.Count)];
    }

}
