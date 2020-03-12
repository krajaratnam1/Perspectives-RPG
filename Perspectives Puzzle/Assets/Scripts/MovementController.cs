using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    public bool playerCanMove = true, canMove = true, isPushing = false;

    private float InputX, InputZ, Speed, gravity;

    Vector3 lastGroundedPos;

    [SerializeField] Camera cam;
    public CharacterController characterController;

    private Vector3 desiredMoveDirection;
    float ungroundedTimer = 0, groundedTimer = 0;


    [SerializeField] float rotationSpeed = 0.3f;
    [SerializeField] float allowRotation = 0.1f;
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float gravityMultipler;

    public bool gravityEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        lastGroundedPos = this.transform.position + Vector3.down * 5;
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

        if(!canMove || !gravityEnabled)
        {
            return;
        }

        if(characterController.isGrounded)
        {
            groundedTimer += Time.deltaTime;
            ungroundedTimer = 0;
            if (groundedTimer >= 0.5f)
            {
                lastGroundedPos = transform.position;
            }
        } else
        {
            ungroundedTimer += Time.deltaTime;
            groundedTimer = 0;
            if(ungroundedTimer >= 1f)
            {
                print("Repositioning");
                transform.position = lastGroundedPos + Vector3.up*5;
                ungroundedTimer = 0;
            }
        }

    }


    void InputDecider()
    {
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        if (Speed > allowRotation)
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
        gravity = gravityEnabled ? (gravity * gravityMultipler) : 0;

        Vector3 moveDirection = (playerCanMove ? desiredMoveDirection : Vector3.zero) * (movementSpeed * Time.deltaTime);
        moveDirection = new Vector3(moveDirection.x, gravity, moveDirection.z);
        characterController.Move(moveDirection);

        if (characterController.isGrounded)
        {
            gravity = 0;
        }
    }



}
