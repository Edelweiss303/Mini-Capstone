using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public Material AmmoMaterial;
    public Color LightColor;
    public EnemyBase.EnemyColour EnemyColour;
    public List<GameObject> Canisters;
    public int AmmoAmount = 4;

    private Light ammoLight;
    private List<MeshRenderer> canisterRenderers = new List<MeshRenderer>();
    private float gameID;

    private void Start()
    {
        ammoLight = GetComponent<Light>();

        foreach(GameObject canister in Canisters)
        {
            canisterRenderers.Add(canister.GetComponent<MeshRenderer>());
        }

        ChangeEnemyType(ColourManager.Instance.AllEnemyTypes[Random.Range(0, ColourManager.Instance.AllEnemyTypes.Count)]);
        gameID = gameObject.GetInstanceID();
        GameNetwork.Instance.ToPlayerQueue.Add("g:GunnerCreateAmmo:" + EnemyColour.ToString() + ":" + gameID + ":" + transform.position.x  + ":" + transform.position.y + ":" + transform.position.z);
    }

    public void ChangeEnemyType(EnemyBase.EnemyColour inEnemyColour)
    {
        EnemyColour = inEnemyColour;

        LightColor = ColourManager.Instance.AmmoColorMap[inEnemyColour];
        ammoLight.color = LightColor;

        AmmoMaterial = ColourManager.Instance.EnemyMaterialMap[inEnemyColour];
        foreach(MeshRenderer canisterRenderer in canisterRenderers)
        {
            canisterRenderer.material = AmmoMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PilotPlayerController>())
        {
            GameNetwork.Instance.ToPlayerQueue.Add("g:GunnerGetAmmo:" + EnemyColour.ToString() + ":" + gameID + ":" + AmmoAmount);
            Destroy(gameObject);
        }

    }
}
