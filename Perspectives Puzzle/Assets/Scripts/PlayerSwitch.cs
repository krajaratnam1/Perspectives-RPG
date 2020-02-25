using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitch : MonoBehaviour
{
    bool isPlayerA = true;

    public void SwitchPlayers(){
        isPlayerA = !isPlayerA;
    }
}
