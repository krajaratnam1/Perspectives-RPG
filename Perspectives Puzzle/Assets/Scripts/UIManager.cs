using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Text controlText;
    bool textVisible = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W) && textVisible){
            controlText.gameObject.SetActive(false);
            textVisible = false;
        }

        if(Input.GetKeyDown(KeyCode.Backslash)){
            if(textVisible){
                controlText.gameObject.SetActive(false);
                textVisible = !textVisible;
            } else{
                controlText.gameObject.SetActive(true);
                textVisible = !textVisible;
            }
        }
    }
}
