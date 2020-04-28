using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class FungusTrigger : MonoBehaviour
{
    public TimestampManager timeManager; 
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
        timeManager = GameObject.Find("Timer").GetComponent<TimestampManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Fungus Trigger - OnTriggerEnter - " + this.name);

        Debug.Log("playerSwapSystem.isBigPlayer - " + playerSwapSystem.isBigPlayer);

        Debug.Log("other.name - " + other.name);

        if (other.GetComponent<MovementController>() != null) // is a player
        {
            Debug.Log("Fungus Trigger - OnTriggerEnter - Player");

            if (playerSwapSystem.isBigPlayer && other.name == "Big Player")
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
                if(invisWall.isExit && other.name == "Big Player")
                {
                    timeManager.IncrementLevel();
                }
                invisWall.gameObject.SetActive(false);
            }

            return;
        }

        if (other.GetComponent<PushableObject>() != null) // is small statue
        {
            Debug.Log("Fungus Trigger - OnTriggerEnter - Small Statue");
            if (other.GetComponent<PushableObject>().carried)
            {
                if (pushingBlock != "")
                {
                    print("Pushing Block Executed - " + this.name);
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
