using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderPanelSwitchController : ColliderInteractionController
{
    public GameObject pSwitchToMove;
    private Vector3 originalPosition;
    public Vector3 newPosition;
    public bool newPositionIsOffset = true;

    public ColliderPanelSwitchController friendToSyncWith;
    public bool isUp = false;
    public float lerpSpeed = 1;

    // Start is called before the first frame update
    public void Start()
    {
        base.Start();
        MethodToCall += ActivatePanelSwitch;
        isUp = false;
        originalPosition = pSwitchToMove.transform.position;
        if(newPositionIsOffset)
        {
            newPosition += originalPosition;
        }
    }


    void ActivatePanelSwitch()
    {
        Debug.Log("Activate Panel Switch");

        isUp = !isUp;
        friendToSyncWith.isUp = isUp;
        MoveSwitch();

        friendToSyncWith.MoveSwitch();
    }

    public void MoveSwitch()
    {
        //lerp it!
        StartCoroutine(LerpThing(pSwitchToMove.transform, lerpSpeed, pSwitchToMove.transform.position, isUp ? newPosition : originalPosition));

        //pSwitchToMove.transform.SetPositionAndRotation(isUp ? newPosition : originalPosition, pSwitchToMove.transform.rotation);
    }

    IEnumerator LerpThing(Transform thing, float waitTime, Vector3 start, Vector3 target){
         float elapsedTime = 0;
         
         while (elapsedTime < waitTime)
        {
            thing.position = Vector3.Lerp(start, target, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
        
            // Yield here
            yield return null;
        }  
        // Make sure we got there
        thing.position = target;

        yield return null;
    }
}
