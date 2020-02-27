using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ColliderInteractionController : MonoBehaviour
{
    public UnityAction MethodToCall;
    public KeyCode KeyInput;
    public PlayerSwitch playerSwapSystem;
    public Color baseColor;
    public Color highlightColor;

    // Start is called before the first frame update
    public void Start()
    {
        playerSwapSystem = GameObject.FindWithTag("PlayerSwap").GetComponent<PlayerSwitch>();
        MeshRenderer objectHitRenderer = this.GetComponent<MeshRenderer>();
        baseColor = objectHitRenderer.material.GetColor("_Color");
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject player;
        if (playerSwapSystem.isBigPlayer)
        {
            player = playerSwapSystem.bigPlayer;
        }
        else
        {
            player = playerSwapSystem.smallPlayer;
        }

        Collider playerCollider = player.GetComponent<SphereCollider>();

        Debug.Log("OnTriggerEnter");
        Debug.Log("Player: " + player);
        Debug.Log("other: " + other);

        if (other.transform.name == playerCollider.transform.name)
        {
            MeshRenderer objectHitRenderer = this.GetComponent<MeshRenderer>();
            objectHitRenderer.material.SetColor("_Color", highlightColor);
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject player;
        if (playerSwapSystem.isBigPlayer)
        {
            player = playerSwapSystem.bigPlayer;
        }
        else
        {
            player = playerSwapSystem.smallPlayer;
        }

        Collider playerCollider = player.GetComponent<SphereCollider>();

        Debug.Log("OnTriggerExit");
        Debug.Log("Player: " + player);
        Debug.Log("other: " + other);

        if (other.transform.name == playerCollider.transform.name)
        {
            MeshRenderer objectHitRenderer = this.GetComponent<MeshRenderer>();
            objectHitRenderer.material.SetColor("_Color", baseColor);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyUp(KeyInput))
        {
            GameObject player;
            if (playerSwapSystem.isBigPlayer)
            {
                player = playerSwapSystem.bigPlayer;
            }
            else
            {
                player = playerSwapSystem.smallPlayer;
            }

            Collider playerCollider = player.GetComponent<SphereCollider>();

            if (other.transform.name == playerCollider.transform.name)
            {
                Debug.Log("Activated Collider");
                MethodToCall();
            }
        }
    }

    public bool CheckColliderIsPlayer(Collider other)
    {
        GameObject player;
        if (playerSwapSystem.isBigPlayer)
        {
            player = playerSwapSystem.bigPlayer;
        }
        else
        {
            player = playerSwapSystem.smallPlayer;
        }

        Collider playerCollider = player.GetComponent<SphereCollider>();

        return other.transform.name == playerCollider.transform.name;
    }
}