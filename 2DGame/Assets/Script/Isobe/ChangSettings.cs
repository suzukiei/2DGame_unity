using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangSettings : MonoBehaviour
{
    [SerializeField] GameObject TopObj;
    [SerializeField] List<GameObject> BottomObj;
    public void OnClick()
    {
        TopObj.GetComponent<Canvas>().sortingOrder = 5;
        foreach (var gameobj in BottomObj)
        {

            gameobj.GetComponent<Canvas>().sortingOrder = -1;

        }
        
    }
}
