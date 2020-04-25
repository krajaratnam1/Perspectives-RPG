using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateEffect : MonoBehaviour {

    public static Vector2 move = new Vector2(1, 1);


    // Update is called once per frame
    public static void AnimateEff (Material m) {

        move = new Vector2( move.x + Time.deltaTime*0.1f, move.y + Time.deltaTime*0.1f);

        m.SetTextureOffset("_Noise", -move);
        m.SetTextureOffset("_Noise2", move);

    }
}
