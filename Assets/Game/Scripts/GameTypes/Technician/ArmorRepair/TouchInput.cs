using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour
{
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
                    //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 3.0f);

                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        GameObject obj = hit.collider.gameObject;
                        Material material = hit.collider.gameObject.GetComponent<Renderer>().sharedMaterial;
                        Vector4 tiling = material.GetVector("Polygon_Tiling");
                        Vector4 offset = material.GetVector("Polygon_Offset");
                        Vector2 vec2Offset = new Vector2(offset.x + 0.5f, offset.y + 0.5f);
                        Vector2 texCoord = new Vector2(
                            Mathf.Round(hit.textureCoord.x * 10) / 10,
                            Mathf.Round(hit.textureCoord.y * 10) / 10
                            );

                        if(texCoord == vec2Offset)
                        {
                            material.SetVector("Polygon_Tiling", Vector4.zero);
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
