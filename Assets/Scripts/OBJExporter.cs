using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;

/*=============================================================================
 |	    Project:  Unity3D Scene OBJ Exporter
 |
 |		  Notes: Only works with meshes + meshRenderers. No terrain yet
 |
 |       Author:  aaro4130
 |
 |     DO NOT USE PARTS OF THIS CODE, OR THIS CODE AS A WHOLE AND CLAIM IT
 |     AS YOUR OWN WORK. USE OF CODE IS ALLOWED IF I (aaro4130) AM CREDITED
 |     FOR THE USED PARTS OF THE CODE.
 |
 *===========================================================================*/

public class OBJExporter : MonoBehaviour
{
    public bool onlyCreatedObjects = true;
    private bool applyPosition = true;
    private bool applyRotation = true;
    private bool applyScale = true;
    private bool generateMaterials = true;
    private bool exportTextures = true;
    private bool splitObjects = true;
    private bool autoMarkTexReadable = false;
    private bool objNameAddIdNum = false;
    private GameObject exporttext;
    
    private GameObject maintext;


    private string versionString = "v2.0";

    

    Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }
    Vector3 MultiplyVec3s(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    
    public void brExportToOBj()
    {
        //get list of required export things
        MeshFilter[] sceneMeshes;
        if (onlyCreatedObjects)
        {
            List<MeshFilter> tempMFList = new List<MeshFilter>();
            GameObject[] gs = GameObject.FindGameObjectsWithTag("Selected");
            foreach (GameObject g in gs)
            {

                MeshFilter f = g.GetComponent<MeshFilter>();
                if (f != null)
                {
                    tempMFList.Add(f);
                }

            }
            sceneMeshes = tempMFList.ToArray();
        }
        else
        {
            sceneMeshes = FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];

        }

        if (Application.isPlaying)
        {
            foreach (MeshFilter mf in sceneMeshes)
            {
                MeshRenderer mr = mf.gameObject.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    if (mr.isPartOfStaticBatch)
                    {
                        
                        Debug.Log("Static batched object detected. Static batching is not compatible with this exporter. Please disable it before starting the player.");
                        return;
                    }
                }
            }
        }
        
        //work on export
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("# Export of " + Application.loadedLevelName);
        sb.AppendLine("# from Aaro4130 OBJ Exporter " + versionString);
        
        float maxExportProgress = (float)(sceneMeshes.Length + 1);
        int lastIndex = 0;
        for(int i = 0; i < sceneMeshes.Length; i++)
        {
            string meshName = sceneMeshes[i].gameObject.name;
            float progress = (float)(i + 1) / maxExportProgress;
            MeshFilter mf = sceneMeshes[i];
            MeshRenderer mr = sceneMeshes[i].gameObject.GetComponent<MeshRenderer>();

            if (splitObjects)
            {
                string exportName = meshName;
                if (objNameAddIdNum)
                {
                    exportName += "_" + i;
                }
                sb.AppendLine("g " + exportName);
            }

            //export the meshhh :3
            Mesh msh = mf.sharedMesh;
            int faceOrder = (int)Mathf.Clamp((mf.gameObject.transform.lossyScale.x * mf.gameObject.transform.lossyScale.z), -1, 1);
            
            //export vector data (FUN :D)!
            foreach (Vector3 vx in msh.vertices)
            {
                Vector3 v = vx;
                if (applyScale)
                {
                    v = MultiplyVec3s(v, mf.gameObject.transform.lossyScale);
                }
                
                if (applyRotation)
                {
  
                    v = RotateAroundPoint(v, Vector3.zero, mf.gameObject.transform.rotation);
                }

                if (applyPosition)
                {
                    v += mf.gameObject.transform.position;
                }
                v.x *= -1;
                sb.AppendLine("v " + v.x + " " + v.y + " " + v.z);
            }
            foreach (Vector3 vx in msh.normals)
            {
                Vector3 v = vx;
                
                if (applyScale)
                {
                    v = MultiplyVec3s(v, mf.gameObject.transform.lossyScale.normalized);
                }
                if (applyRotation)
                {
                    v = RotateAroundPoint(v, Vector3.zero, mf.gameObject.transform.rotation);
                }
                v.x *= -1;
                sb.AppendLine("vn " + v.x + " " + v.y + " " + v.z);

            }
            foreach (Vector2 v in msh.uv)
            {
                sb.AppendLine("vt " + v.x + " " + v.y);
            }

            for (int j=0; j < msh.subMeshCount; j++)
            {
                if(mr != null && j < mr.sharedMaterials.Length)
                {
                    string matName = mr.sharedMaterials[j].name;
                    sb.AppendLine("usemtl " + matName);
                }
                else
                {
                    sb.AppendLine("usemtl " + meshName + "_sm" + j);
                }

                int[] tris = msh.GetTriangles(j);
                for(int t = 0; t < tris.Length; t+= 3)
                {
                    int idx2 = tris[t] + 1 + lastIndex;
                    int idx1 = tris[t + 1] + 1 + lastIndex;
                    int idx0 = tris[t + 2] + 1 + lastIndex;
                    if(faceOrder < 0)
                    {
                        sb.AppendLine("f " + ConstructOBJString(idx2) + " " + ConstructOBJString(idx1) + " " + ConstructOBJString(idx0));
                    }
                    else
                    {
                        sb.AppendLine("f " + ConstructOBJString(idx0) + " " + ConstructOBJString(idx1) + " " + ConstructOBJString(idx2));
                    }
                    
                }
            }

            lastIndex += msh.vertices.Length;
        }

        //write to disk
        string path = Path.Combine(Application.persistentDataPath, "data");
        path = Path.Combine(path, System.DateTime.Now.ToString("yyyyMMdd-")+ Random.Range(1000,9999) + ".obj");
        
        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
        
        exporttext = GameObject.Find ("export_text");
        maintext = GameObject.Find ("main_text");

        exporttext.GetComponent<TextMesh>().text = "Exportado";
        exporttext.GetComponent<TextMesh>().color = Color.green;
        
        maintext.GetComponent<TextMesh>().text = "Se exporto en:" + path;
        
        Invoke("resetText",4);
        
        System.IO.File.WriteAllText(path, sb.ToString());
        

        //export complete, close progress dialog
    }
    

    private string ConstructOBJString(int index)
    {
        string idxString = index.ToString();
        return idxString + "/" + idxString + "/" + idxString;
    }

    private void resetText()
    {
        exporttext.GetComponent<TextMesh>().text = "Exportar";
        maintext.GetComponent<TextMesh>().text = "Figuras";
        exporttext.GetComponent<TextMesh>().color = Color.white;
    }
}