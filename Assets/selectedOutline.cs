using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectedOutline : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject selectedObject;


    public bool lookingAtObject = false;

    public bool flashingIn = true;

    public bool startedFlshing = false;
    
    // Update is called once per frame
    void Update()
    {
        if (lookingAtObject == true)
        {
            selectedObject.GetComponent<Outline>().OutlineWidth = 5;
        }
    }
}
