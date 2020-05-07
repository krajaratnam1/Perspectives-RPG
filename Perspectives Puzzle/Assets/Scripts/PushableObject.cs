using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushableObject : MonoBehaviour
{
    public bool carried = false, prompting = false;
    public Vector3 pushDirection;
    MovementController movement;
    CharacterController player;
    CharacterController pushable;
    public float carryTimer = -1, carryTime = 0.5f;
    public float speed = 1;
    public KeyCode pushKey = KeyCode.F;
    public GameObject prompt;

    // Start is called before the first frame update
    void Start()
    {
        pushable = gameObject.GetComponent<CharacterController>();
        //prompt = GameObject.Find("Push Prompt");
    }

    // Update is called once per frame
    void Update()
    {

        if (carried)
        {
            if(carryTimer > 0)
            {
                carryTimer -= Time.deltaTime;
                transform.localPosition += Vector3.up * 0.25f * Time.deltaTime / carryTime;
                if(transform.localPosition.y >= 0)
                {
                    carryTimer = -1;
                }
            } else
            {
                movement.canMove = true;
            }
        } else
        {
            if (carryTimer > 0)
            {
                carryTimer -= Time.deltaTime;
                transform.localPosition += Vector3.down * 0.25f * Time.deltaTime / carryTime;
                if(transform.localPosition.y <= -0.25f)
                {
                    carryTimer = -1;
                }
            } else
            {
                if (movement != null)
                {
                    movement.canMove = true;
                    movement.GetComponent<CharacterController>().radius = 0.7f;
                }
                transform.SetParent(null);
                

            }
        }

        if(prompting || carried) 
        {
            prompt.GetComponent<Text>().text = carried ? " " : "F to Carry.";
            prompt.SetActive(true);
            movement.carrying = carried;
            if(Input.GetKeyDown(pushKey))
            {
                if (carried)
                {
                    movement.canMove = false;
                    carried = false;
                    carryTimer = carryTime;
                }
                else
                {
                    movement.canMove = false;
                    carried = true;
                    carryTimer = carryTime;
                    transform.SetParent(movement.transform);
                    transform.localPosition = new Vector3(0, -0.25f, 0.87f);
                    transform.localEulerAngles = Vector3.zero;
                    //movement.GetComponent<CharacterController>().radius = 1.15f;
                }
            }
        } else
        {
            prompt.SetActive(false);
            if (movement != null)
            {
                movement.canMove = true;
				movement.carrying = false;
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.GetComponent<MovementController>() != null)
        {

            movement = other.gameObject.GetComponent<MovementController>();
            player = other.gameObject.GetComponent<CharacterController>();

            if (movement.enabled)
            { 
                prompting = true;
            } 
            
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(player != null)
        {
            if (other.name == player.name)
            {
                prompting = false;
            }
        }
        
    }
}
