using UnityEngine;

public class Rotate : MonoBehaviour {

    public float speed =1.0f;
	
	// Update is called once per frame
	void Update () {

        transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));

	}
}
