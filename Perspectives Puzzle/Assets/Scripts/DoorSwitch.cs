using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : ColliderInteractionController
{
    public GameObject door, doorCopy;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        MethodToCall += Pressed;
    }


    void Pressed()
    {
        door.SetActive(!door.activeInHierarchy);
        doorCopy.SetActive(!doorCopy.activeInHierarchy);
    }
}
