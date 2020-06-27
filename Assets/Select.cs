using System;
using UnityEngine;

public class Select : MonoBehaviour
{
    public static string selectedObject;
    public string internalObject;
    public static GameObject ThisSelected;
    private Outline _outline;

    // Update is called once per frame
    

    /*void Update()
    {
        
        
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out theObject))
        {
            if (theObject.transform.gameObject.CompareTag("Selected"))
            {
                ThisSelected = theObject.transform.gameObject;
                selectedObject = ThisSelected.name;
                internalObject = ThisSelected.name; 
            
                if (ThisSelected != null) _outline = ThisSelected.GetComponent<Outline>();

                if (_outline != null) _outline.OutlineWidth = 10;
            }
            
            
           
        }
    }*/

    private void OnCollisionEnter(Collision col)
    {    
        if (ThisSelected != null)
        {
            var selectionRenderer = ThisSelected.GetComponent<Renderer>();
            if (ThisSelected != null) _outline = ThisSelected.GetComponent<Outline>();
            if (_outline != null) _outline.OutlineWidth = 0;
            ThisSelected = null;
           
        }
        
        if (col.gameObject.name == "Contact Fingerbone")
        {
            ThisSelected = this.gameObject;
            selectedObject = ThisSelected.name;
            internalObject = ThisSelected.name;

            if (ThisSelected != null)
            {
                _outline = ThisSelected.GetComponent<Outline>();
            }

            if (_outline !=null)
            {
                _outline.OutlineWidth = 10;
                _outline.OutlineColor = Color.green;
            }
        }
    }
}
