using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using Unity.VisualScripting;

public class CsvReadWrite : MonoBehaviour
{
    private List<string[]> _rowData = new List<string[]>();
    private readonly Vector3 _guardianSize = new Vector3(3f, 4f, 3f); // Defined Guardian Size
    public Player playerScript;


    // Use this for initialization
    void Start()
    {
        //guardianRendSize = GameObject.Find("Guardian").GetComponent<Renderer>().bounds.size; // Get the render dimensions of a game object named guardian without any parent.
        First();
        InvokeRepeating(nameof(Save), 1.0f, repeatRate:1.0f);
      
    }

    void First(){
        string[] rowDataTemp = new string[3];
        rowDataTemp[0] = "New Participant";
        _rowData.Add(rowDataTemp);
    }


    //float w = Mathf.Sqrt((new X - old X) * (new X - old X) + (new Z - old Z) * (new Z - old Z))

    // ReSharper disable Unity.PerformanceAnalysis
    public Vector3 oldPos = new Vector3 (0f,0f,0f);


    void Save()
    {
        
        // Creating First row of titles manually..
        string[] rowDataTemp = new string[3];

        // You can add up the values in as many cells as you want.
        for (int i = 0; i < 1; i++)
        {
            rowDataTemp = new string[1]; // Speed Parameter
            var position = transform.position; // Current Camera Position
            float w = Mathf.Sqrt(Mathf.Pow((position.x - oldPos.x), 2) +
                                 Mathf.Pow((position.z - oldPos.z), 2));
            
            if (Mathf.Abs(position.x) > (_guardianSize.x / 2) |
                Mathf.Abs(position.z) > (_guardianSize.z / 2)) // If the player's head leaves the boundary multiply the speed score with -1;
            {
                w = w * -1;
            }

            rowDataTemp[0] = position.x + "," + position.y + "," + position.z + "," + w;
        }

        oldPos = transform.position;
        _rowData.Add(rowDataTemp);


    }

    void OnApplicationQuit()
         {

             string[][] output = new string[_rowData.Count][];

             for (int i = 0; i < output.Length; i++)
             {
                 output[i] = _rowData[i];
             }

             int length = output.GetLength(0);
             const string delimiter = ",";

             StringBuilder sb = new StringBuilder();

             for (int index = 0; index < length; index++)
                 sb.AppendLine(string.Join(delimiter, output[index]));


             string filePath = GetPath();

             StreamWriter outStream = System.IO.File.AppendText(filePath);
             outStream.WriteLine(sb);
             outStream.Close();
             Debug.Log(transform.position);
         }

    // Following method is used to retrieve the relative path as device platform
    private string GetPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/CSV/" + "Saved_data.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"Saved_data.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"Saved_data.csv";
#else
        return Application.dataPath +"/"+"Saved_data.csv";
#endif
    }
}