using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogScript : MonoBehaviour
{

    PlayerSwitch playerSwitch;
    Camera smallCam;
    Camera bigCam;

    public bool isClosed = false;

    public float fogCloseDistance = 40;
    public float fogFarDistance = 150;

    public float lerpSpeed = 2;

    void Awake()
    {
        playerSwitch = GameObject.Find("PlayerSwitch").GetComponent<PlayerSwitch>();
        smallCam = playerSwitch.smallCam.GetComponent<Camera>();
        bigCam = playerSwitch.bigCam.GetComponent<Camera>();

    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other){

        //if they're entering
        if(other.gameObject.name == "Big Player" && playerSwitch.isBigPlayer && !playerSwitch.fogIsClose || other.gameObject.name == "Small Player" && !playerSwitch.isBigPlayer && !playerSwitch.fogIsClose){
            playerSwitch.fogIsClose = true;
            StartCoroutine(LerpFog(lerpSpeed, RenderSettings.fogEndDistance, fogCloseDistance));
        //if they're exiting
        } else if (other.gameObject.name == "Big Player" && playerSwitch.isBigPlayer && playerSwitch.fogIsClose || other.gameObject.name == "Small Player" && !playerSwitch.isBigPlayer && playerSwitch.fogIsClose){
            playerSwitch.fogIsClose = false;
            StartCoroutine(LerpFog(lerpSpeed, fogCloseDistance, fogFarDistance));
        }
    }

    IEnumerator LerpFog(float waitTime, float currentDistance, float target){
         float elapsedTime = 0;
         
         while (elapsedTime < waitTime)
        {
            RenderSettings.fogEndDistance = Mathf.Lerp(currentDistance, target, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
        
            // Yield here
            yield return null;
        }  
        // Make sure we got there
        //RenderSettings.fogEndDistance = target;
        yield return null;
    }
}
