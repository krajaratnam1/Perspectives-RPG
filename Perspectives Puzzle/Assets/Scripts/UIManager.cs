using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject titleCard;
    bool textVisible = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)){
            if(textVisible){
                titleCard.SetActive(false);
                textVisible = !textVisible;
            } else{
                titleCard.SetActive(true);
                textVisible = !textVisible;
            }
        }
    }
}
