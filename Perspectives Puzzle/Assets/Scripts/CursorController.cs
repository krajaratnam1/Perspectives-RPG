using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    bool cursorLock;
    void Start()
    {
        //hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Tab)){
            if(cursorLock){
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                cursorLock = false;
            }else{
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                cursorLock = true;
            }
        }
    }
}
