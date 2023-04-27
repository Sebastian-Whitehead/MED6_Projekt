using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logging : MonoBehaviour
{
   private LoggingManager _loggingManager;

   //Attack - 
   //Charge 
   //Move
   //TakeDmg
   private int tile;
   private string tileName = "Bruh";

   public Player playerScript;
  
   
   private void Start()
   {
      _loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
      _loggingManager.CreateLog("Log", headers: new List<string>() {"Attack Count"});
     
     ;
      
      _loggingManager.SaveAllLogs(clear:true);
      _loggingManager.NewFilestamp();
   }

   private void Update()
   {
      int attack = playerScript.attack_count;

      //Store the data
      _loggingManager.Log("Log", "Attack Count", attack);

   }
}
