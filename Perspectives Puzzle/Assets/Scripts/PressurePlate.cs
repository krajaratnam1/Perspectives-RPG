using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class PressurePlate : MonoBehaviour
{
    public Color baseColor;
    public Color highlightColor;
    public GameObject toDisappear;
    Flowchart flowchart;
    public string optionalFungusBlock;
    MeshRenderer mr;
    public int weight = 0;

    // Start is called before the first frame update
    void Start()
    {
        mr = gameObject.GetComponent<MeshRenderer>();
        if (highlightColor.a <= 0) // if highlight color not specified
        {
            highlightColor = Color.red; // default to red
        }
        if(baseColor.a <= 0)
        {
            baseColor = mr.material.GetColor("_Color");
        } else
        {
            mr.material.SetColor("_Color", baseColor);
        }
        flowchart = GameObject.Find("Flowchart").GetComponent<Flowchart>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (weight > 0)
        {
            mr.material.SetColor("_Color", highlightColor);

            
            if (toDisappear != null)
            {
                toDisappear.SetActive(false);
            }

            if (optionalFungusBlock != "")
            {
                flowchart.ExecuteBlock(optionalFungusBlock);
            }
        } else
        {
            mr.material.SetColor("_Color", baseColor);
            /*if (toDisappear != null)
            {
                toDisappear.SetActive(true);
            }*/

            if (optionalFungusBlock != "")
            {
                flowchart.ExecuteBlock(optionalFungusBlock);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MovementController>() != null || other.GetComponent<ClimbableObject>() != null || other.GetComponent<PushableObject>() != null || other.GetComponent<CharacterController>() != null)
        {
            weight++;
            if (toDisappear != null)
            {
                toDisappear.SetActive(false);
            }
        } else
        {
            print("Not Playable");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MovementController>() != null || other.GetComponent<ClimbableObject>() != null || other.GetComponent<PushableObject>() != null || other.GetComponent<CharacterController>() != null)
        {
            weight--;
            if(weight <= 0)
            {
                if (toDisappear != null)
                {
                    toDisappear.SetActive(true);
                }
            }
        } else
        {
            print("Not playable");
        }
    }
}
