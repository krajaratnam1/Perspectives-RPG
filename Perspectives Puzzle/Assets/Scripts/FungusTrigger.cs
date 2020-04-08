using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class FungusTrigger : MonoBehaviour
{
    public InvisibleWall invisWall = null;
    public PlayerSwitch playerSwapSystem;
    public Flowchart flowchart;
    public string bigPlayerBlock, smallPlayerBlock, pushingBlock;
    public bool Overriding = true;
    bool pushingBlockExecuted = false;
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
            

            if(playerSwapSystem.isBigPlayer && other.name == "Big Player")
            {
                if(bigPlayerBlock != "" && !pushingBlockExecuted)
                {
                    if (Overriding)
                    {
                        print("overriding");
                        flowchart.StopAllBlocks();
                    }
                    flowchart.ExecuteBlock(bigPlayerBlock);
                }
            } else if(!playerSwapSystem.isBigPlayer && other.name == "Small Player")
            {
                if(smallPlayerBlock != "" && !pushingBlockExecuted)
                {
                    if (Overriding)
                    {
                        print("small");
                        flowchart.StopAllBlocks();
                    }
                    print(smallPlayerBlock);
                    flowchart.ExecuteBlock(smallPlayerBlock);
                }
            }

            if (invisWall != null)
            {
                invisWall.gameObject.SetActive(false);
            }

            return;
        }

        if (other.GetComponent<PushableObject>() != null) // is small statue
        {
            if (other.GetComponent<PushableObject>().carried)
            {
                if (pushingBlock != "")
                {
                    print("Pushing Block Executed");
                    pushingBlockExecuted = true;
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
            if (other.GetComponent<PushableObject>().carried)
            {
                if (pushingBlock != "" && !pushingBlockExecuted)
                {
                    print("Pushing Block Executed");
                    pushingBlockExecuted = true;
                    flowchart.ExecuteBlock(pushingBlock);
                }
            }
            return;
        }
    }
}
