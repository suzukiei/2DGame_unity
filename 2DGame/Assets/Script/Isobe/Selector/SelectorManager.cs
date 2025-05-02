using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManager : SingletonMonoBehaviour<SelectorManager>
{
    public List<Selects> selectors = new List<Selects>();//�J�ڐ�
    public List<GameObject> selectobj;
    public int SelectorNum; //���݂̃Z���N�gNumber

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

