using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Selects
{
    [SerializeField, Header("�X�e�[�W�ԍ�")] public int StageIndex;
    [SerializeField, Header("�X�e�[�W��")] public string StageName;
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
