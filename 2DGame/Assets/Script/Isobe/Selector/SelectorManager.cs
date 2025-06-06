using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManager : SingletonMonoBehaviour<SelectorManager>
{
    public List<Selects> selectors = new List<Selects>();//遷移先
    public List<GameObject> selectobj;
    public int SelectorNum; //現在のセレクトNumber

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

