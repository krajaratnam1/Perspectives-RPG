using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ColliderInteractionController : MonoBehaviour
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
    }

    void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyUp(KeyInput))
        {
            GameObject player;
            if (playerSwapSystem.isBigPlayer)
            {
                player = playerSwapSystem.bigPlayer;
            }
            else
            {
                player = playerSwapSystem.smallPlayer;
            }

            Collider playerCollider = player.GetComponent<SphereCollider>();

            if (other.transform.name == playerCollider.transform.name)
            {
                Debug.Log("Activated Collider");
                MethodToCall();
            }
        }
    }

}