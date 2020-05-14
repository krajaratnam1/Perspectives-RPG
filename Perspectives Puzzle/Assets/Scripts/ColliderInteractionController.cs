using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ColliderInteractionController : MonoBehaviour
{
    public UnityAction MethodToCall;
    public KeyCode KeyInput;
    public PlayerSwitch playerSwapSystem;
    //public Color baseColor;
    //public Color highlightColor;
    public float highlightTimeout = 0.3f;
    public string selected = "";
    float timer = 0; // time out to dehighlight button

    //Animator
    Animator anim;

    // Start is called before the first frame update
    public void Start()
    {
        //playerSwapSystem = GameObject.FindWithTag("PlayerSwap").GetComponent<PlayerSwitch>();
        //MeshRenderer objectHitRenderer = this.GetComponent<MeshRenderer>();
        //baseColor = objectHitRenderer.material.GetColor("_Color");
        playerSwapSystem = GameObject.Find("PlayerSwitch").GetComponent<PlayerSwitch>();

        //Animator
        anim = gameObject.GetComponentInChildren<Animator>();

        /*
        if (highlightColor.a <= 0)
        {
            highlightColor = new Color(1, 1, 1, 1); // default highlight color will be white
        }
        */

        if (KeyInput == KeyCode.F) // this is for climbing; can't be the same as buttons!
        {
            KeyInput = KeyCode.E;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > highlightTimeout) // timed out
        {
            //MeshRenderer objectHitRenderer = this.GetComponent<MeshRenderer>();
            //objectHitRenderer.material.SetColor("_Color", baseColor);
            selected = "";
        }
        else
        {
            timer += Time.deltaTime;
        }

        if (selected == (playerSwapSystem.isBigPlayer ? playerSwapSystem.bigPlayer.name : playerSwapSystem.smallPlayer.name))
        {
            if (Input.GetKeyUp(KeyInput) && playerSwapSystem.fadeDir == 0)
            {
                Debug.Log("Activated Collider");
                MethodToCall();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        /*GameObject player;
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
        Debug.Log("other: " + other);*/

        if (CheckColliderIsPlayer(other))
        {
            //MeshRenderer objectHitRenderer = this.GetComponent<MeshRenderer>();
            //objectHitRenderer.material.SetColor("_Color", highlightColor);
            selected = other.name;

            //Animator
            if(anim != null){
                anim.SetBool("Play", true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        /*GameObject player;
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
        Debug.Log("other: " + other);*/

        if (CheckColliderIsPlayer(other))
        {
            //MeshRenderer objectHitRenderer = this.GetComponent<MeshRenderer>();
            //objectHitRenderer.material.SetColor("_Color", baseColor);
            selected = "";

            //Animator
            if(anim != null){
                anim.SetBool("Play", false);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (CheckColliderIsPlayable(other))
        {
            timer = 0;
            if (CheckColliderIsPlayer(other))
            {
                if (selected != other.name)
                {
                    selected = other.name;
                    //MeshRenderer objectHitRenderer = this.GetComponent<MeshRenderer>();
                    //objectHitRenderer.material.SetColor("_Color", highlightColor);
                }
            }
            /*if (Input.GetKeyUp(KeyInput) && playerSwapSystem.fadeDir == 0)
            {
                if (CheckColliderIsPlayer(other))
                {

                    Debug.Log("Activated Collider");
                    MethodToCall();
                }
            }*/
        }
    }

    public bool CheckColliderIsPlayable(Collider other)
    {
        return other.name == playerSwapSystem.bigPlayer.name || other.name == playerSwapSystem.smallPlayer.name;
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

        return other.name == player.name;
    }


}

