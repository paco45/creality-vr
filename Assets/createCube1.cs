using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class createCube1 : MonoBehaviour
{    
    
    private GameObject deletetext;

    public void ShapeBuilder()
    {
        GameObject shapeObj = Instantiate(gameObject);
        if (shapeObj.CompareTag("little"))
        {
            shapeObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else
        {
            shapeObj.transform.localScale = Vector3.one * Random.Range(0.1f, 0.1f);
        }

        shapeObj.transform.position = new Vector3(-1.388f, 2.249f, 0.935f);
        shapeObj.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        shapeObj.tag = "Selected";
        shapeObj.AddComponent<InteractionBehaviour>();
        shapeObj.GetComponent<Rigidbody>().isKinematic = true;
        shapeObj.AddComponent<Outline>();
        shapeObj.AddComponent<Select>();
        //shapeObj.AddComponent<SimpleInteractionGlow>();
    }

    public void ShapeKiller()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Selected");
        foreach(GameObject enemy in enemies)
            Destroy(enemy);
        
        deletetext = GameObject.Find ("delete_text");
        deletetext.GetComponent<TextMesh>().text = "Eliminado";
        deletetext.GetComponent<TextMesh>().color = Color.red;
        
        Invoke("resetText",4);
    }
    
    private void resetText()
        {
            deletetext.GetComponent<TextMesh>().text = "Eliminar";
            deletetext.GetComponent<TextMesh>().color = Color.white;
        }

}

