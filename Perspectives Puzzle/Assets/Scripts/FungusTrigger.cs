using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class FungusTrigger : MonoBehaviour
{
    public PlayerSwitch playerSwapSystem;
    public Flowchart flowchart;
    public string bigPlayerBlock, smallPlayerBlock, pushingBlock;
    public bool Overriding = true;
    // Start is called before the first frame update
    void Start()
    {
        playerSwapSystem = GameObject.Find("PlayerSwitch").GetComponent<PlayerSwitch>();
        flowchart = GameObject.Find("Flowchart").GetComponent<Flowchart>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovementController>() != null) // is a player
        {
            if(playerSwapSystem.isBigPlayer)
            {
                if(bigPlayerBlock != "")
                {
                    if (Overriding)
                    {
                        flowchart.StopAllBlocks();
                    }
                    flowchart.ExecuteBlock(bigPlayerBlock);
                }
            } else
            {
                if(smallPlayerBlock != "")
                {
                    if (Overriding)
                    {
                        flowchart.StopAllBlocks();
                    }
                    flowchart.ExecuteBlock(smallPlayerBlock);
                }
            }
            return;
        }

        if (other.GetComponent<PushableObject>() != null) // is small statue
        {
            if (other.GetComponent<PushableObject>().pushing)
            {
                if (pushingBlock != "")
                {
                    flowchart.StopAllBlocks();
                    flowchart.ExecuteBlock(pushingBlock);
                }
            }
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PushableObject>() != null) // is small statue
        {
            if (other.GetComponent<PushableObject>().pushing)
            {
                if (pushingBlock != "")
                {
                    flowchart.ExecuteBlock(pushingBlock);
                }
            }
            return;
        }
    }
}
