using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour
{
    public float touchDistanceThreshold = 0.1f;
    public float repairAmount = 10.0f;

    private Camera camera;
    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        ScreenTap();
    }

    private void ScreenTap()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hit))
                {
                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
                        Material sharedMaterial = renderer.sharedMaterial;

                        Vector2 offset = (sharedMaterial.GetVector("Polygon_Offset")) * -1;
                        
                        Vector2 hitCoord = new Vector2(
                            (Mathf.Round(hit.textureCoord.x * 10) / 10) - 0.5f,
                            (Mathf.Round(hit.textureCoord.y * 10) / 10) - 0.5f
                            );

                        if (Vector2.Distance(hitCoord, offset) <= touchDistanceThreshold)
                        {
                            sharedMaterial.SetVector("Polygon_Tiling", Vector2.zero);
                        }

                        int index = Array.IndexOf(HullManager.Instance.armourRenderers, renderer);

                        if (!HullManager.Instance.armourIndexes.Contains(index))
                        {
                            HullManager.Instance.armourIndexes.Add(index);
                        }

                        //This health value needs to be replaced with whatever the networked one is
                        HullManager.Instance.health = HullManager.Instance.damageThresholds[HullManager.Instance.damageIndex] + repairAmount;
                        if(HullManager.Instance.damageIndex > 0)
                        {
                            HullManager.Instance.damageIndex--;
                        }
                        
                    }

                }
            }
        }
    }

    public void OnResetOrientation()
    {
        transform.rotation = Quaternion.identity;
    }
}
