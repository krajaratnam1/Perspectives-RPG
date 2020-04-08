using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimestampManager : MonoBehaviour
{
    public GameObject window, chapterSelect;
    public PlayerSwitch playerSwapSystem;
    public Text timeText; 
    public float[] times;
    public int level = 0;
    public string timeString = "";
    
    // Start is called before the first frame update
    void Start()
    {
        times = new float[] { 0, 0, 0, 0, 0 };
        playerSwapSystem = GameObject.Find("PlayerSwitch").GetComponent<PlayerSwitch>();
        chapterSelect = GameObject.Find("Chapter Select");
    }


    public void ShowWindow()
    {
        window.SetActive(true);
        playerSwapSystem.bigPlayer.GetComponent<MovementController>().enabled = false;
        playerSwapSystem.smallPlayer.GetComponent<MovementController>().enabled = false;
        playerSwapSystem.LockMouse();
        Cursor.visible = true;
        chapterSelect.SetActive(false);

        timeString = "";
        for(int i = 0; i<5; i++)
        {
            timeString += "Level " + (i + 1).ToString() + " : " + times[i] + "\n";
        }

        timeText.text = timeString;
    }


    public void IncrementLevel()
    {
        level++;
        if(level >= 5)
        {
            ShowWindow();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(window.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;


            if(Input.GetKeyDown(KeyCode.Return))
            {
                window.SetActive(false);
                (playerSwapSystem.isBigPlayer ? playerSwapSystem.bigPlayer : playerSwapSystem.smallPlayer).GetComponent<MovementController>().enabled = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                playerSwapSystem.UnlockMouse();
                chapterSelect.SetActive(true);
                return;
            }

            if(Input.GetKeyDown(KeyCode.C))
            {
                TextEditor te = new TextEditor();
                te.text = timeString;
                te.SelectAll();
                te.Copy();
            }

        } else
        {
            if(level < 5)
            {
                times[level] += Time.deltaTime;
            }
        }
    }
}
