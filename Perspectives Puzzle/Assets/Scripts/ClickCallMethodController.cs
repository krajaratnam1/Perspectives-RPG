using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class ClickCallMethodController : MonoBehaviour
{
    public float RayCastDistance = 3.0f;
    public UnityAction MethodToCall;
	public KeyCode KeyInput;
    public PlayerSwitch playerSwapSystem;

    // Start is called before the first frame update
    public void Start()
    {
        playerSwapSystem = GameObject.FindWithTag("PlayerSwap").GetComponent<PlayerSwitch>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyInput))
        {
            GameObject camera;
            if (playerSwapSystem.isBigPlayer)
            {
                camera = playerSwapSystem.bigCam;
            }
            else
            {
                camera = playerSwapSystem.smallCam;
            }

            Ray ray = camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, RayCastDistance))
            {
                if (hit.transform == this.transform)
                {
					MethodToCall();
                }
            }
        }
    }
}
