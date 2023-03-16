using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCam : MonoBehaviour
{


    public Camera cam;
    public Material minimapMaterial;
    public RenderTexture minimapTexture;



    public void Start()
    {
        cam = GetComponent<Camera>();
        minimapTexture = new RenderTexture(512, 512, 1);
        cam.targetTexture = minimapTexture;
        minimapMaterial.mainTexture =  minimapTexture;
    }

    public void PositionCam(Rect rect)
    {
        cam.orthographicSize = rect.height;
        cam.transform.position = new Vector3(rect.center.x, 1000, rect.center.y);

        minimapMaterial.SetVector("_Anchor", rect.position);
        minimapMaterial.SetFloat("_Size", rect.width);

    }

    public void OnDestroy()
    {
        minimapTexture.Release();
    }

}
