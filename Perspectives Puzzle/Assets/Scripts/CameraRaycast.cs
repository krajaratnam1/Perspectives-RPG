using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycast : MonoBehaviour
{
    Camera cam;
    LayerMask mask;

    public GameObject objectHit;
    GameObject lastHitByRay;
     
    public Color highlightColor;

    public RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        mask = LayerMask.GetMask("Terrain");   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, mask))
        {
            objectHit = hit.transform.gameObject;

            // highlight the hit object
            MeshRenderer objectHitRenderer = objectHit.GetComponent<MeshRenderer>();
            objectHitRenderer.material.SetColor("_Color", highlightColor);

            LastHit();
        }
    }

    //change the last hit game object back to white
    void LastHit()
    {
        GameObject nowHitByRay = objectHit;
        if (lastHitByRay && lastHitByRay != nowHitByRay)
        {
            lastHitByRay.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        }
        lastHitByRay = nowHitByRay;
    }
}
