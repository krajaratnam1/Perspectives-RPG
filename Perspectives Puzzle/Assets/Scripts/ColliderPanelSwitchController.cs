using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderPanelSwitchController : ColliderInteractionController
{
    public GameObject pSwitchToMove;
    private Vector3 originalPosition;
    public Vector3 newPosition;

    public ColliderPanelSwitchController friendToSyncWith;
    public bool isUp = false;

    // Start is called before the first frame update
    public void Start()
    {
        base.Start();
        MethodToCall += ActivatePanelSwitch;
        isUp = false;
        originalPosition = this.transform.position;
    }


    void ActivatePanelSwitch()
    {
        Debug.Log("Activate Panel Switch");

        isUp = !isUp;
        friendToSyncWith.isUp = isUp;
        MoveSwitch();

        friendToSyncWith.MoveSwitch();
    }

    public void MoveSwitch()
    {
        pSwitchToMove.transform.SetPositionAndRotation(isUp ? newPosition : originalPosition, pSwitchToMove.transform.rotation);
    }
}