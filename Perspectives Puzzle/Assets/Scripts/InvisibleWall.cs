using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class InvisibleWall : MonoBehaviour
{
    public Flowchart flowchart;
    public bool isExit = true, blockPlayer = true;
    public bool scene2 = false, entered = false; // scene 2 variables
    public float rephaseWaitingTime = 0; // scene 2 variables
    public InvisibleWall clone;
    public PlayerSwitch playerSwapSystem;
    public int level = 1;
    public FungusTrigger blocking, otherSide;
    float alphaMultiplier = 1;
    public float visibilityDistance = 1, alpha;
    public MovementController movement;
    public Material rippleMat;
    public GameObject invisWall, ripples;
    // Start is called before the first frame update
    void Start()
    {
        flowchart = GameObject.Find("Flowchart").GetComponent<Flowchart>();
        playerSwapSystem = GameObject.Find("PlayerSwitch").GetComponent<PlayerSwitch>();
        rippleMat = Resources.Load<Material>("Ripple");
        if(isExit)
        {
            blocking.bigPlayerBlock = "Level " + level.ToString() + " Big Exit";
            blocking.smallPlayerBlock = "Level " + level.ToString() + " Small Exit";
            blocking.pushingBlock = "";
            otherSide.bigPlayerBlock = "";
            otherSide.smallPlayerBlock = "";
            otherSide.pushingBlock = "Level " + level.ToString() + " Pushing Exit";
        } else
        {
            blocking.bigPlayerBlock = "";
            blocking.smallPlayerBlock = "";
            blocking.pushingBlock = "";
            otherSide.bigPlayerBlock = "";
            otherSide.smallPlayerBlock = "";
            otherSide.pushingBlock = "Level " + level.ToString() + " Enter";
        }
    }

    // Update is called once per frame
    void Update()
    {

            if (movement != null)
            {
                if (!movement.enabled) // have to manually link if scene 2 is active.
                {
                    return;
                }
                if (!scene2)
                {
                    if (movement.carrying || !blockPlayer)
                    {
                        blocking.gameObject.SetActive(false);
                        invisWall.SetActive(false);
                        if (alphaMultiplier > 0)
                        {
                            alphaMultiplier -= Time.deltaTime;
                        }
                    }
                    else
                    {
                        blocking.gameObject.SetActive(true);
                        invisWall.SetActive(true);
                        if (alphaMultiplier < 1)
                        {
                            alphaMultiplier += Time.deltaTime;
                        }

                   

                    }
                } 

                float dist = Vector3.Distance(transform.position, movement.transform.position);
                if (dist < visibilityDistance)
                {
                    alpha = 1 - (dist / visibilityDistance);
                    alpha *= alphaMultiplier;
                }
                else
                {
                    alpha = 0;
                }


                if (movement.isBig == playerSwapSystem.isBigPlayer)
                {
                    Color col = rippleMat.GetColor("_Color");
                    rippleMat.SetColor("_Color", new Color(col.r, col.g, col.b, alpha));

                    //print(level.ToString() + " " + (isExit ? "Exit" : "Entrance"));
                }

            }


        if (scene2 && clone != null)
        {
            if (entered && !clone.entered)
            {
                if (rephaseWaitingTime < 10)
                {
                    rephaseWaitingTime += Time.deltaTime;
                    if (rephaseWaitingTime >= 0.5f)
                    {
                        flowchart.ExecuteBlock("Out of Phase");
                        rephaseWaitingTime = 11;
                    }

                }
            }

            if (entered && clone.entered) // can only leave when in phase with reflection
            {
                blocking.gameObject.SetActive(false);
                invisWall.SetActive(false);
                if (alphaMultiplier > 0)
                {
                    alphaMultiplier -= Time.deltaTime;
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (scene2)
        {
            if (other.GetComponent<CharacterController>() != null)
            {
                entered = true;
            }
        }
        else
        {

            if (other.GetComponent<MovementController>() != null)
            {
                movement = other.GetComponent<MovementController>();
                ripples.SetActive(true);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (scene2)
        {
            
            if (other.GetComponent<CharacterController>() != null)
            {
                rephaseWaitingTime = 0;
                entered = false;
            }
        }
        else
        {
            if (other.GetComponent<MovementController>() != null)
            {
                movement = null;
                ripples.SetActive(false);

            }
        }
    }
}
