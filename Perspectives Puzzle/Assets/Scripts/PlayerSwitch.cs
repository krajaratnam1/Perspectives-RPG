using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerSwitch : MonoBehaviour
{
    public GameObject bigPlayer, bigStatue, smallPlayer, smallStatue, bigAnchor, smallAnchor,
        smallCam, bigCam;
    public bool isBigPlayer = true;
    public CinemachineFreeLook bigCameraFreeLook, smallCameraFreeLook;
    public RawImage bigView;
    public float fadeTime = 1;
    public int fadeDir = 0; // -1 for out, 1 for in



    public void Start()
    {
        SetPlayer(isBigPlayer);
        bigStatue.GetComponent<MovementController>().enabled = false;
        smallStatue.GetComponent<MovementController>().enabled = false;
    }


    public void SetPlayer(bool isBig)
    {
        isBigPlayer = isBig;
        fadeDir = isBigPlayer ? 1 : -1;
        smallPlayer.GetComponent<MovementController>().enabled = false;
        bigPlayer.GetComponent<MovementController>().enabled = false;
        bigCameraFreeLook.enabled = false;
        smallCameraFreeLook.enabled = false;
    }

    public void SwitchPlayers()
    {
        SetPlayer(!isBigPlayer);
    }

    void Synchronize()
    {
        if (isBigPlayer)
        {
            bigStatue.transform.position = (bigPlayer.transform.position - bigAnchor.transform.position)
                   + smallAnchor.transform.position;
            bigStatue.transform.eulerAngles = bigPlayer.transform.eulerAngles;
            smallPlayer.transform.position = (smallStatue.transform.position - bigAnchor.transform.position)
                   + smallAnchor.transform.position;
            smallPlayer.transform.eulerAngles = smallStatue.transform.eulerAngles;
            smallCam.transform.position = (bigCam.transform.position - bigAnchor.transform.position)
                   + smallAnchor.transform.position;
        }
        else
        {
            smallStatue.transform.position = (smallPlayer.transform.position - smallAnchor.transform.position)
                    +  bigAnchor.transform.position;
            smallStatue.transform.eulerAngles = smallPlayer.transform.eulerAngles;
            bigPlayer.transform.position = (bigStatue.transform.position - smallAnchor.transform.position)
                    + bigAnchor.transform.position;
            bigPlayer.transform.eulerAngles = bigStatue.transform.eulerAngles;
            bigCam.transform.position = (smallCam.transform.position - smallAnchor.transform.position)
                  + bigAnchor.transform.position;
        }
    }

    void FinishFadeOut()
    {
        bigCameraFreeLook.LookAt = smallStatue.transform;
        bigCameraFreeLook.Follow = smallStatue.transform;
        smallCameraFreeLook.LookAt = smallPlayer.transform;
        smallCameraFreeLook.Follow = smallPlayer.transform;
        smallPlayer.GetComponent<MovementController>().enabled = true;
        bigCameraFreeLook.enabled = false;
        smallCameraFreeLook.enabled = true;
    }

    void FinishFadeIn()
    {
        print("Finished Fade In");
        bigCameraFreeLook.LookAt = bigPlayer.transform;
        bigCameraFreeLook.Follow = bigPlayer.transform;
        smallCameraFreeLook.LookAt = smallStatue.transform;
        smallCameraFreeLook.Follow = smallStatue.transform;
        bigPlayer.GetComponent<MovementController>().enabled = true;
        smallCameraFreeLook.enabled = false;
        bigCameraFreeLook.enabled = true;
    }

    void Fade()
    {
        if(fadeDir == -1)
        {
            if(bigView.color.a <= 0)
            {
                fadeDir = 0;
                FinishFadeOut();

            } else
            {
                bigView.color -= new Color(0, 0, 0, Time.deltaTime / fadeTime);
                
            }
        } else if(fadeDir == 1)
        {
            if (bigView.color.a >= 1)
            {
                fadeDir = 0;
                FinishFadeIn();
            }
            else
            {
                bigView.color += new Color(0, 0, 0, Time.deltaTime / fadeTime);
                
            }
        }
    }

    public void Update()
    {
        Synchronize();
        Fade();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            SwitchPlayers();
        }
    }
}

