using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
   [SerializeField] private Color baseColor, subColor, HoverColor;
   
   private bool even;
   Renderer rend;
  // [SerializeField] private GameObject hovered;

   
   public void Init(bool isEven){
    rend = GetComponent<Renderer>();
    even = isEven;
    rend.material.color = even ? subColor : baseColor;
   }

   void OnMouseEnter(){
      Debug.Log("Enter");
      rend.material.color = HoverColor;
   }
  
  void OnMouseExit(){
   rend.material.color = even ? subColor : baseColor;
      Debug.Log("Exit");
   }
}
