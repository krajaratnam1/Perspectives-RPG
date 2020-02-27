using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveSwitchButtonController : ColliderInteractionController
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        MethodToCall += SwitchPerspectives;
    }

    void SwitchPerspectives()
    {
        playerSwapSystem.SwitchPlayers();
    }
}
