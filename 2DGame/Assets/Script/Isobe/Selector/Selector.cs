using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Selects
{
    [SerializeField, Header("ステージ番号")] public int StageIndex;
    [SerializeField, Header("ステージ名")] public string StageName;
}



public class Selector : MonoBehaviour
{

    //[SerializeField]
   public List<Selects> selectors = new List<Selects>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
