using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChanges : MonoBehaviour
{
    // Start is called before the first frame update

    public Material SkyBox;

    void Start()
    {
        RenderSettings.skybox = SkyBox;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
