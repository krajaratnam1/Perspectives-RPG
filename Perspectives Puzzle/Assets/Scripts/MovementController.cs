using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    public bool canMove = true;

    private float InputX, InputZ, Speed, gravity;

    [SerializeField] Camera cam;
    private CharacterController characterController;

    private Vector3 desiredMoveDirection;


    [SerializeField] float rotationSpeed = 0.3f;
    [SerializeField] float allowRotation = 0.1f;
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float gravityMultipler;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        InputDecider();
        if (canMove)
        {
            MovementManager();
        }

    }


    void InputDecider() 
    {
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        if(Speed > allowRotation)
        {
            if (canMove)
            {
                RotationManager();
            }
        }
        else
        {
            desiredMoveDirection = Vector3.zero;
        }

    }



    void RotationManager()
    {
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * InputZ + right * InputX;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), rotationSpeed);

    }

    void MovementManager()
    {
        gravity -= 9.8f * Time.deltaTime;
        gravity = gravity * gravityMultipler;

        Vector3 moveDirection = desiredMoveDirection * (movementSpeed * Time.deltaTime);
        moveDirection = new Vector3(moveDirection.x, gravity, moveDirection.z);
        characterController.Move(moveDirection); 

        if(characterController.isGrounded)
        {
            gravity = 0;
        }
    }



}
