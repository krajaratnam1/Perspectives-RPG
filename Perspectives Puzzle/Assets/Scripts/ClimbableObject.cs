using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableObject : MonoBehaviour
{
    public bool climbing = false, prompting = false, centering = false;
    public float height;
    public Vector3 centerDirection;
    MovementController movement;
    CharacterController player;
    CharacterController climbable;
    public float climbTime = 0.8f, centeringTime = 0.2f;
    float centeringTimer = 0;
    public KeyCode pushKey = KeyCode.F;
    public GameObject prompt;
    float origHeight = 0;

    // Start is called before the first frame update
    void Start()
    {
        climbable = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (climbing)
        {
            prompt.SetActive(false);
            player.Move(Vector3.up * height * Time.deltaTime / climbTime);
            if (player.gameObject.transform.position.y >= height + origHeight)
            {
                print(height + origHeight);
                climbing = false;
                player.Move(Vector3.down * (player.gameObject.transform.position.y - (height + origHeight)));
                // finished climbing
                centering = true;
                centeringTimer = 0;
                centerDirection = -movement.gameObject.transform.position + transform.position;
                centerDirection = new Vector3(centerDirection.x, 0, centerDirection.z);
                centerDirection /= centeringTime;
            }
            return;
        }

        if (centering)
        {
            centeringTimer += Time.deltaTime;
            player.Move(centerDirection * Time.deltaTime);
            if (centeringTimer >= centeringTime) // done centering
            {
                centering = false;
                movement.canMove = true;
            }
        }

        if (prompting)
        {
            prompt.SetActive(true);
            if (Input.GetKeyDown(pushKey))
            {
                movement.canMove = false;
                climbing = true;
                prompt.SetActive(false);
                prompting = false;
                origHeight = player.gameObject.transform.position.y;
            }
            else
            {
                movement.canMove = true;
            }
        }
        else
        {
            prompt.SetActive(false);
            if (movement != null)
            {
                movement.canMove = true;
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<MovementController>() != null)
        {

            movement = other.gameObject.GetComponent<MovementController>();
            player = other.gameObject.GetComponent<CharacterController>();

            if (movement.enabled && !climbing && !centering)
            {
                prompting = true;
            }

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (player != null)
        {
            if (other.name == player.name)
            {
                prompting = false;
            }
        }

    }
}
