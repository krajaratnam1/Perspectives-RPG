using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Fungus;

public class PlayerSwitch : MonoBehaviour
{
	public Flowchart flowchart;
    public GameObject bigPlayer, bigStatue, smallPlayer, smallStatue, bigAnchor, smallAnchor,
        smallCam, bigCam;
    public bool isBigPlayer = true;
	public string SwapToBigBlock, SwapToSmallBlock;
    public CinemachineFreeLook bigCameraFreeLook, smallCameraFreeLook;
    public CinemachineBrain smallBrain, bigBrain;
    public GameObject smallStatueLook, bigStatueLook;

    public RawImage bigView;
    public float fadeTime = 1;
    public int fadeDir = 0; // -1 for out, 1 for in
    public bool simultaneousFadingAndBlending = false;

    public float cooldown = 1f, cdTimer = 0;

    float bigFLYAxis, bigFLXAxis, smallFLYAxis, smallFLXAxis;

    Vector3 oldCamPos, oldCamAngles;
    public bool tracking = false;

    public float angleEpsilon = 0.0025f, positionEpsilon = 0.1f;




    public void Start()
    {
        flowchart = GameObject.Find("Flowchart").GetComponent<Flowchart>();
        SetPlayer(isBigPlayer);
        if (isBigPlayer)
        {
            bigView.color = new Color(bigView.color.r, bigView.color.g, bigView.color.b, 1);
        }
        else
        {
            bigView.color = new Color(bigView.color.r, bigView.color.g, bigView.color.b, 0);
        }
    }

    public void LockMouse()
    {
        bigFLYAxis = bigCameraFreeLook.m_YAxis.m_MaxSpeed;
        bigFLXAxis = bigCameraFreeLook.m_XAxis.m_MaxSpeed;
        bigCameraFreeLook.m_YAxis.m_MaxSpeed = 0;
        bigCameraFreeLook.m_XAxis.m_MaxSpeed = 0;

        smallFLYAxis = smallCameraFreeLook.m_YAxis.m_MaxSpeed;
        smallFLXAxis = smallCameraFreeLook.m_XAxis.m_MaxSpeed;
        smallCameraFreeLook.m_YAxis.m_MaxSpeed = 0;
        smallCameraFreeLook.m_XAxis.m_MaxSpeed = 0;
    }

    public void UnlockMouse()
    {
        bigCameraFreeLook.m_YAxis.m_MaxSpeed = bigFLYAxis;
        bigCameraFreeLook.m_XAxis.m_MaxSpeed = bigFLXAxis;
        smallCameraFreeLook.m_YAxis.m_MaxSpeed = smallFLYAxis;
        smallCameraFreeLook.m_XAxis.m_MaxSpeed = smallFLXAxis;
    }

    public void SetPlayer(bool isBig)
    {
        if (cdTimer > 0)
        {
            return;
        }

        cdTimer = cooldown;
        LockMouse();
        Synchronize();
        Fade();
        isBigPlayer = isBig;
        fadeDir = isBigPlayer ? 1 : -1;
        if (simultaneousFadingAndBlending)
        {
            if (!isBigPlayer)
            {
                smallStatueLook.SetActive(true);
                bigStatueLook.SetActive(false);
                bigCameraFreeLook.enabled = true;
                smallCameraFreeLook.enabled = true;
            }
            else
            {
                bigStatueLook.SetActive(true);
                smallStatueLook.SetActive(false);
                bigCameraFreeLook.enabled = true;
                smallCameraFreeLook.enabled = true;
            }
        }
        smallPlayer.GetComponent<MovementController>().enabled = false;
        bigPlayer.GetComponent<MovementController>().enabled = false;
    }

    public void SwitchPlayers()
    {
        SetPlayer(!isBigPlayer);
		flowchart.ExecuteBlock(isBigPlayer?SwapToBigBlock:SwapToSmallBlock);

    }

    void Synchronize()
    {

        if (cdTimer > 0)
        {
            cdTimer -= Time.deltaTime;
            bigStatueLook.transform.position = (bigCam.transform.position - bigAnchor.transform.position)
                    + smallAnchor.transform.position;
            bigStatueLook.transform.eulerAngles = bigCam.transform.eulerAngles;

            smallStatueLook.transform.position = (smallCam.transform.position - smallAnchor.transform.position)
                      + bigAnchor.transform.position;
            smallStatueLook.transform.eulerAngles = smallCam.transform.eulerAngles;

            if (cdTimer <= 0)
            {
                if (isBigPlayer)
                {
                    if (bigPlayer.GetComponent<MovementController>().characterController.isGrounded)
                    {
                        bigPlayer.GetComponent<MovementController>().enabled = true;
                        UnlockMouse();
                    }
                }
                else
                {
                    if (smallPlayer.GetComponent<MovementController>().characterController.isGrounded)
                    {
                        smallPlayer.GetComponent<MovementController>().enabled = true;
                        UnlockMouse();
                    }
                }
            }
        }



        if (isBigPlayer)
        {
            bigStatue.transform.position = (bigPlayer.transform.position - bigAnchor.transform.position)
                   + smallAnchor.transform.position;
            bigStatue.transform.eulerAngles = bigPlayer.transform.eulerAngles;
            smallPlayer.transform.position = (smallStatue.transform.position - bigAnchor.transform.position)
                   + smallAnchor.transform.position;
            smallPlayer.transform.eulerAngles = smallStatue.transform.eulerAngles;
            if (fadeDir == 0)
            {
                bigStatueLook.transform.position = (bigCam.transform.position - bigAnchor.transform.position)
                    + smallAnchor.transform.position;
                bigStatueLook.transform.eulerAngles = bigCam.transform.eulerAngles;
            }
        }
        else
        {
            smallStatue.transform.position = (smallPlayer.transform.position - smallAnchor.transform.position)
                    + bigAnchor.transform.position;
            smallStatue.transform.eulerAngles = smallPlayer.transform.eulerAngles;
            bigPlayer.transform.position = (bigStatue.transform.position - smallAnchor.transform.position)
                    + bigAnchor.transform.position;
            bigPlayer.transform.eulerAngles = bigStatue.transform.eulerAngles;
            if (fadeDir == 0)
            {
                smallStatueLook.transform.position = (smallCam.transform.position - smallAnchor.transform.position)
                      + bigAnchor.transform.position;
                smallStatueLook.transform.eulerAngles = smallCam.transform.eulerAngles;
            }
        }
    }

    void FinishFadeOut()
    {
        if (!simultaneousFadingAndBlending)
        {
            smallStatueLook.SetActive(true);
            bigStatueLook.SetActive(false);
            bigCameraFreeLook.enabled = true;
            smallCameraFreeLook.enabled = true;
        }
        //UnlockMouse();
        tracking = true;
        oldCamPos = smallCameraFreeLook.transform.position;
        oldCamAngles = smallCameraFreeLook.transform.eulerAngles;
        //smallPlayer.GetComponent<MovementController>().enabled = true;
    }

    void FinishFadeIn()
    {
        if (!simultaneousFadingAndBlending)
        {
            bigStatueLook.SetActive(true);
            smallStatueLook.SetActive(false);
            bigCameraFreeLook.enabled = true;
            smallCameraFreeLook.enabled = true;
        }
        //bigPlayer.GetComponent<MovementController>().enabled = true;
        //UnlockMouse();
        tracking = true;
        oldCamPos = bigCameraFreeLook.transform.position;
        oldCamAngles = bigCameraFreeLook.transform.eulerAngles;
    }

    void Fade()
    {
        if (fadeDir == -1)
        {
            if (!simultaneousFadingAndBlending)
            {
                bigCameraFreeLook.enabled = false;
                smallCameraFreeLook.enabled = false;
            }
            if (bigView.color.a <= 0)
            {
                fadeDir = 0;
                FinishFadeOut();

            }
            else
            {
                bigView.color -= new Color(0, 0, 0, Time.deltaTime / fadeTime);

            }
        }
        else if (fadeDir == 1)
        {
            if (!simultaneousFadingAndBlending)
            {
                bigCameraFreeLook.enabled = false;
                smallCameraFreeLook.enabled = false;
            }
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

    void Track()
    {
        if (cdTimer > 0)
        {
            return;
        }
        if (tracking)
        {
            Vector3 posDiff = (isBigPlayer ? (bigCameraFreeLook) : (smallCameraFreeLook)).transform.position - oldCamPos;
            Vector3 angleDiff = (isBigPlayer ? (bigCameraFreeLook) : (smallCameraFreeLook)).transform.eulerAngles - oldCamAngles;
            if (posDiff.magnitude <= positionEpsilon && angleDiff.magnitude <= angleEpsilon)
            {
                //print("Camera movement done!");
                UnlockMouse();
                tracking = false;
                (isBigPlayer ? bigPlayer : smallPlayer).GetComponent<MovementController>().enabled = true;
            }
            else
            {
                oldCamPos = (isBigPlayer ? (bigCameraFreeLook) : (smallCameraFreeLook)).transform.position;
                oldCamAngles = (isBigPlayer ? (bigCameraFreeLook) : (smallCameraFreeLook)).transform.eulerAngles;
            }
        }
    }

    public void Update()
    {
        Track();
        Synchronize();
        Fade();

        if (!smallPlayer.GetComponent<MovementController>().characterController.isGrounded)
        {
            //print("Not Grounded!");
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SwitchPlayers();
        }
    }
}

