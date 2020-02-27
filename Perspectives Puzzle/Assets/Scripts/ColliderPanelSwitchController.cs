using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderPanelSwitchController : ColliderInteractionController
{
    // Start is called before the first frame update
    public void Start()
    {
        base.Start();
        MethodToCall += ActivatePanelSwitch;
    }


    void ActivatePanelSwitch()
    {
        Debug.Log("Activate Panel Switch");
    }
}