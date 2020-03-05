using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    float gravity;
    PlayerSwitch playerSwapSystem;

    Vector3 lastGroundedPos;

    [SerializeField] Camera cam;
    public CharacterController characterController;

    private Vector3 desiredMoveDirection;
    float ungroundedTimer = 0, groundedTimer = 0;


    [SerializeField] float rotationSpeed = 0.3f;
    [SerializeField] float allowRotation = 0.1f;
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float gravityMultipler;

    public bool gravityEnabled = true, big = false;
    // Start is called before the first frame update
    void Start()
    {
        playerSwapSystem = GameObject.Find("PlayerSwitch").GetComponent<PlayerSwitch>();
        characterController = GetComponent<CharacterController>();
        lastGroundedPos = this.transform.position + Vector3.down * 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSwapSystem.isBigPlayer != big)
        {
            gravity -= 9.8f * Time.deltaTime;
            gravity = gravityEnabled ? (gravity * gravityMultipler) : 0;
            Vector3 direction = Vector3.up * gravity;
            characterController.Move(direction);
            print(direction);
            print(characterController.isGrounded);
        }

        
        if (characterController.isGrounded || playerSwapSystem.isBigPlayer == big)
        {
            gravity = 0;
            groundedTimer += Time.deltaTime;
            ungroundedTimer = 0;
            if (groundedTimer >= 0.5f)
            {
                lastGroundedPos = transform.position;
            }
        }
        else
        {
            ungroundedTimer += Time.deltaTime;
            groundedTimer = 0;
            if (ungroundedTimer >= 1f)
            {
                print("Repositioning");
                transform.position = lastGroundedPos + Vector3.up * 5;
                ungroundedTimer = 0;
            }
        }
    }
}
