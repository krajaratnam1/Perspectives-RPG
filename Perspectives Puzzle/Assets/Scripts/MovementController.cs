using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    public PlayerSwitch playerSwapSystem;
    public Flowchart flowchart;
    public bool playerCanMove = true, canMove = true, carrying = false, climbing = false, isBig = false;

    private float InputX, InputZ, Speed, gravity, cloneGravity;

    public string blockOnFall = "Fallen";

    public Vector3 lastGroundedPos, cloneGroundedPos;

    [SerializeField] Camera cam;
    public CharacterController characterController;

    private Vector3 desiredMoveDirection;
    public float ungroundedTimer = 0, groundedTimer = 0, cloneUngroundedTimer = 0, cloneGroundedTimer = 0;

    Animator animator;


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
        
        playerSwapSystem = GameObject.Find("PlayerSwitch").GetComponent<PlayerSwitch>();
        flowchart = GameObject.Find("Flowchart").GetComponent<Flowchart>();
        animator = GetComponent<Animator>();

        if (playerSwapSystem.mirroring)
        {
            cloneGroundedPos = (playerSwapSystem.isBigPlayer ? playerSwapSystem.smallStatue : playerSwapSystem.bigStatue).transform.position + Vector3.down * 5;
        }
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

        // the below code is for repositioning the player after falling

        if(characterController.isGrounded) // keeping track of the position when they're not falling
        {
            groundedTimer += Time.deltaTime;
            ungroundedTimer = 0;
            if (groundedTimer >= 0.5f) // to make sure that we don't place the player ridiculously close to just before they fell
            {
                lastGroundedPos = transform.position;
                groundedTimer = 0;
            }
        } else
        {
            ungroundedTimer += Time.deltaTime;
            groundedTimer = 0;
            if(ungroundedTimer >= 1f) 
            {
                flowchart.ExecuteBlock(blockOnFall);
                print("Repositioning after fall");
                transform.position = lastGroundedPos + Vector3.up*5;
                ungroundedTimer = 0;
            }
        }


        // repositioning clone after falling

        if(playerSwapSystem.mirroring)
        {
            if((playerSwapSystem.isBigPlayer ? playerSwapSystem.smallStatue : playerSwapSystem.bigStatue).GetComponent<CharacterController>().isGrounded)
            {
                cloneGroundedTimer += Time.deltaTime;
                cloneUngroundedTimer = 0;
                if(cloneGroundedTimer >= 0.5f)
                {
                    cloneGroundedPos = (playerSwapSystem.isBigPlayer ? playerSwapSystem.smallStatue : playerSwapSystem.bigStatue).transform.position;
                    cloneGroundedTimer = 0;
                }
            } else
            {
                cloneUngroundedTimer += Time.deltaTime;
                cloneGroundedTimer = 0;
                if(cloneUngroundedTimer >= 1f)
                {
                    print("Repositioning clone after fall.");
                    (playerSwapSystem.isBigPlayer ? playerSwapSystem.smallStatue : playerSwapSystem.bigStatue).transform.position = cloneGroundedPos + Vector3.up * 5;
                }
            }
        }

        //Animator Stuff
        if(animator != null){
            animator.SetFloat("MoveSpeed", Speed);
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


        if (playerSwapSystem.mirroring) // move the statue accordingly
        {
            cloneGravity -= 9.8f * Time.deltaTime;
            cloneGravity = gravityEnabled ? (cloneGravity * gravityMultipler) : 0;
            (playerSwapSystem.isBigPlayer ? playerSwapSystem.smallStatue : playerSwapSystem.bigStatue).GetComponent<CharacterController>().Move(new Vector3(moveDirection.x, cloneGravity, -moveDirection.z));
            if((playerSwapSystem.isBigPlayer ? playerSwapSystem.smallStatue : playerSwapSystem.bigStatue).GetComponent<CharacterController>().isGrounded)
            {
                cloneGravity = 0;
            }
        }
    }



}
