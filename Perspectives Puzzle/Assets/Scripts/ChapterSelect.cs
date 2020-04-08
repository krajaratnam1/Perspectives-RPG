using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class ChapterSelect : MonoBehaviour
{
    public GameObject window,
        level1BigStatue, level1BigPlayer, level1SmallPlayer, level1SmallStatue,
        level2BigStatue, level2BigPlayer, level2SmallPlayer, level2SmallStatue,
        level3BigStatue, level3BigPlayer, level3SmallPlayer, level3SmallStatue,
        level4BigStatue, level4BigPlayer, level4SmallPlayer, level4SmallStatue,
        level5BigStatue, level5BigPlayer, level5SmallPlayer, level5SmallStatue;


    public int selection = 0;

    public Text selectionText;

    public PlayerSwitch playerSwapSystem;

    GameObject[] bigStatues, bigPlayers, smallStatues, smallPlayers;

    public Flowchart flowchart;
    // Start is called before the first frame update
    void Start()
    {
        playerSwapSystem = GameObject.Find("PlayerSwitch").GetComponent<PlayerSwitch>();

        bigStatues = new GameObject[] { level1BigStatue, level2BigStatue, level3BigStatue, level4BigStatue, level5BigStatue };
        bigPlayers = new GameObject[] { level1BigPlayer, level2BigPlayer, level3BigPlayer, level4BigPlayer, level5BigPlayer };
        smallStatues = new GameObject[] { level1SmallStatue, level2SmallStatue, level3SmallStatue, level4SmallStatue, level5SmallStatue };
        smallPlayers = new GameObject[] { level1SmallPlayer, level2SmallPlayer, level3SmallPlayer, level4SmallPlayer, level5SmallPlayer };

        flowchart = GameObject.Find("Flowchart").GetComponent<Flowchart>();


        for(int i = 0; i<5; i++)
        {
            bigStatues[i] = GameObject.Find("Level " + (i + 1).ToString() + " Big Statue Spawn");
            bigPlayers[i] = GameObject.Find("Level " + (i + 1).ToString() + " Big Player Spawn");
            smallStatues[i] = GameObject.Find("Level " + (i + 1).ToString() + " Small Statue Spawn");
            smallPlayers[i] = GameObject.Find("Level " + (i + 1).ToString() + " Small Player Spawn");

            if(bigStatues[i] == null)
            {
                print("did not find " + "Level " + (i + 1).ToString() + " Big Statue Spawn");
            }
        }


    }



    public void SetLevel(int level)
    {
        if(level < 0 || level > 4)
        {
            return;
        } else
        {
            print(smallStatues[level].name);
            playerSwapSystem.smallStatue.transform.position = smallStatues[level].transform.position;
            playerSwapSystem.bigStatue.transform.position = bigStatues[level].transform.position;
            playerSwapSystem.smallPlayer.transform.position = smallPlayers[level].transform.position;
            playerSwapSystem.bigPlayer.transform.position = bigPlayers[level].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {

        selectionText.text = "Level " + (selection + 1).ToString();

        if(window.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                selection--;
                if(selection < 0)
                {
                    selection = 0;
                }
            }

            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                selection++;
                if(selection > 4)
                {
                    selection = 4;
                }
            }

            if(Input.GetKeyDown(KeyCode.Return))
            {
                SetLevel(selection);
                window.SetActive(false);
                (playerSwapSystem.isBigPlayer ? playerSwapSystem.bigPlayer : playerSwapSystem.smallPlayer).GetComponent<MovementController>().enabled = true;
                return;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                window.SetActive(false);
                (playerSwapSystem.isBigPlayer ? playerSwapSystem.bigPlayer : playerSwapSystem.smallPlayer).GetComponent<MovementController>().enabled = true;
                return;
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                window.SetActive(true);
                playerSwapSystem.bigPlayer.GetComponent<MovementController>().enabled = false;
                playerSwapSystem.smallPlayer.GetComponent<MovementController>().enabled = false;

            }
        }
    }
}
