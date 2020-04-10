using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public Material AmmoMaterial;
    public Color LightColor;
    public GunnerController.EnemyType EnemyType;
    public List<GameObject> Canisters;
    public int AmmoAmount = 4;

    private Light ammoLight;
    private List<MeshRenderer> canisterRenderers = new List<MeshRenderer>();

    private void Start()
    {
        ammoLight = GetComponent<Light>();

        foreach(GameObject canister in Canisters)
        {
            canisterRenderers.Add(canister.GetComponent<MeshRenderer>());
        }

        ChangeEnemyType(PilotController.Instance.AllEnemyTypes[Random.Range(0, PilotController.Instance.AllEnemyTypes.Count + 1)]);
    }

    public void ChangeEnemyType(GunnerController.EnemyType inEnemyType)
    {
        EnemyType = inEnemyType;

        LightColor = PilotController.Instance.AmmoColorMap[EnemyType];
        ammoLight.color = LightColor;

        AmmoMaterial = PilotController.Instance.EnemyMaterialMap[EnemyType];
        foreach(MeshRenderer canisterRenderer in canisterRenderers)
        {
            canisterRenderer.material = AmmoMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameNetwork.Instance.ToPlayerQueue.Add("g:GunnerGetAmmo:" + EnemyType.ToString() + ":" + AmmoAmount);
        Destroy(gameObject);
    }
}
