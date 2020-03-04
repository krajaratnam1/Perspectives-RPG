using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushableObject : MonoBehaviour
{
    public bool pushing = false, prompting = false;
    public Vector3 pushDirection;
    MovementController movement;
    CharacterController player;
    CharacterController pushable;
    public float speed = 1;
    public KeyCode pushKey = KeyCode.F;
    public GameObject prompt;

    // Start is called before the first frame update
    void Start()
    {
        pushable = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        if(pushing)
        {
            prompt.SetActive(false);
            if(Input.GetKey(pushKey))
            {
                player.Move(speed * pushDirection * Time.deltaTime);
                pushable.Move(speed * pushDirection * Time.deltaTime);
                return;
            } else
            {
                pushing = false;
            }
        }

        if(prompting)
        {
            prompt.SetActive(true);
            if(Input.GetKeyDown(pushKey))
            {
                movement.canMove = false;
				movement.isPushing = true;
                pushing = true;
                pushDirection = -movement.gameObject.transform.position + transform.position;
                pushDirection = new Vector3(pushDirection.x, 0, pushDirection.z);
                prompt.SetActive(false);
            } else
            {
                movement.canMove = true;
				movement.isPushing = false;
            }
        } else
        {
            prompt.SetActive(false);
            if (movement != null)
            {
                movement.canMove = true;
				movement.isPushing = false;
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
