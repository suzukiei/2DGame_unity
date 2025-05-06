using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManager : SingletonMonoBehaviour<SelectorManager>
{
    public List<Selects> selectors = new List<Selects>();//‘JˆÚæ
    public List<GameObject> selectobj;
    public int SelectorNum; //Œ»İ‚ÌƒZƒŒƒNƒgNumber

    private void Awake()
    {
        SelectorNum = GameManager.Instance.select;
        Debug.Log("select" + SelectorNum);
        for (int i = 0; i <= SelectorNum; i++)
        {
            selectobj[i].SetActive(true);
        }
    }
}

