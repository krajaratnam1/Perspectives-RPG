using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    public bool isExit = true;
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
            if(!movement.enabled)
            {
                return;
            }

            if(movement.carrying)
            {
                blocking.gameObject.SetActive(false);
                invisWall.SetActive(false);
                if(alphaMultiplier > 0)
                {
                    alphaMultiplier -= Time.deltaTime;
                }
            } else
            {
                blocking.gameObject.SetActive(true);
                invisWall.SetActive(true);
                if(alphaMultiplier < 1)
                {
                    alphaMultiplier += Time.deltaTime;
                }
            }

            float dist = Vector3.Distance(transform.position, movement.transform.position);
            if(dist < visibilityDistance)
            {
                alpha = 1 - (dist / visibilityDistance);
                alpha *= alphaMultiplier;
            } else
            {
                alpha = 0;
            }


            if (movement.isBig == playerSwapSystem.isBigPlayer)
            {
                Color col = rippleMat.GetColor("_Color");
                rippleMat.SetColor("_Color", new Color(col.r, col.g, col.b, alpha));

                print(level.ToString() + " " + (isExit?"Exit":"Entrance"));
            }

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MovementController>() != null)
        {
            movement = other.GetComponent<MovementController>();
            ripples.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MovementController>() != null)
        {
            movement = null;
            ripples.SetActive(false);
        }
    }
}
