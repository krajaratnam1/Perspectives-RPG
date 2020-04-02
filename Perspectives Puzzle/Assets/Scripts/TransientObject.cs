using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransientObject : MonoBehaviour
{
    public PlayerSwitch playerSwapSystem;

    // Start is called before the first frame update
    void Start()
    {
        playerSwapSystem = GameObject.Find("PlayerSwitch").GetComponent<PlayerSwitch>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovementController>() != null)
        {
            print("Player inside of object");
            if (other.name == "Big Player")
            {
                playerSwapSystem.cancelNextSwapBig = true;
            } else
            {
                playerSwapSystem.cancelNextSwapSmall = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MovementController>() != null)
        {
            print("Player outside of object");
            if (other.name == "Big Player")
            {
                playerSwapSystem.cancelNextSwapBig = false;
            }
            else
            {
                playerSwapSystem.cancelNextSwapSmall = false;
            }
        }
    }
}
