using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickCallMoveController : ClickCallMethodController
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        MethodToCall += MoveUp;
    }

    public void MoveUp()
    {
        Debug.Log("Called Move Up");
        //this.transform.Translate(Vector3.up * 1, Space.World);
    }
}
