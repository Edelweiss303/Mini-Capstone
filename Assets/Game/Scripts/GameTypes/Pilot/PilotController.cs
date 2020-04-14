using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static EnemyBase;
using static GunnerController;

public class PilotController : Singleton<PilotController>
{
    public float PickupSpawnFrequency = 8.0f;
    public float PickupSpawnRange = 4.0f;
    public List<GameObject> PickupSpawnLocations;
    public GameObject PickupPrefab;
    public Transform PickupsContainerTransform;
    public PilotPlayerController PlayerController;
    public Camera RadarCamera;
    public Image RadarImage;
    public RawImage RadarRecording;
    public string RadioScanSoundEffectName;
    public HealthBarBehaviour HealthBarBehaviour;
    public GameObject GameOverPanel;
    public PlayerScreen PlayerViewScreen;
    public Text ScoreText;

    private float timeSinceLastPickupSpawn = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        PlayerController = FindObjectOfType<PilotPlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        PickupUpdate();
        RadarUpdate();
    }


    void PickupUpdate()
    {
        timeSinceLastPickupSpawn += Time.deltaTime;
        if(timeSinceLastPickupSpawn > PickupSpawnFrequency)
        {
            Vector3 point;

            //Choose a pickupspawnpoint to originate from
            Vector3 spawnPointPosition = PickupSpawnLocations[UnityEngine.Random.Range(0, PickupSpawnLocations.Count)].transform.position;
            point = new Vector3(UnityEngine.Random.Range(-PickupSpawnRange, PickupSpawnRange), 0, UnityEngine.Random.Range(-PickupSpawnRange, PickupSpawnRange)) + spawnPointPosition;

            //Spawn pickup
            Instantiate(PickupPrefab, point, Quaternion.identity, PickupsContainerTransform);

            timeSinceLastPickupSpawn = 0.0f;
        }
    }

    void RadarUpdate()
    {
        if (Input.GetKey(KeyCode.R))
        {
            RadarImage.gameObject.SetActive(true);
        }
        else
        {
            RadarImage.gameObject.SetActive(false);
        }
    }

    public void ScanRadar()
    {
        AudioManager.Instance.PlaySound(RadioScanSoundEffectName);
        RadarImage.sprite = Sprite.Create(TextureToTexture2D(RadarRecording.texture), RadarImage.sprite.rect, RadarImage.sprite.pivot);
    }

    private Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);
        return texture2D;
    }

    public void SetHealth(float newHealth)
    {
        HealthBarBehaviour.Health = newHealth/15.0f;
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        GameOverPanel.SetActive(true);
        AudioManager.Instance.StopAll();
    }

    public void SetScore(int scoreValue)
    {
        ScoreText.text = "SCORE: " + scoreValue;
    }

}
