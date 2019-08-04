using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //ScaleAnim(new Vector3(0, 0, 0), 300f);
    }
    float progress = 0;
    float duretion = 30f;
    Vector3 targetScale = new Vector3(0, 0, 0);
    void ScaleAnim(Vector3 targetScale, float duretion)
    {
      
        float progress = 0;
     //   while (progress <= duretion)
        {

         
        }
        
    }
    
    // Update is called once per frame
    void Update () {


        if (Input.GetKeyDown(KeyCode.Y))
        {

            progress = progress + Time.deltaTime;
            float percent = Mathf.Clamp01(progress / duretion);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, percent);
        }
    }

}
