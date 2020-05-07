using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

//[ExecuteInEditMode]
public class OutlineEffect : MonoBehaviour {

    private const string LINE_COLOR = "_OutLineColor",
                         LINE_ONLY = "_OutLineOnly";

    RenderTexture rt;
    public Transform Cam_2;

    public Material Post;

    private void Start()
    {
        if (Cam_2 != null) {
            rt = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            Shader.SetGlobalTexture("_Replace", rt);
            Cam_2.GetComponent<Camera>().targetTexture = rt;
        }
    }

    public void Update()
    {
        
        //AnimateEffect.AnimateEff(Post);
    }
    
    private void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, Post);
    }

}
