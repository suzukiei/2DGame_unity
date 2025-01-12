using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageSelector : MonoBehaviour
{
    //ステージポイント管理
    private SelectSceneManager[] stageIndexs;
    private int currentIndex;

    // Start is called before the first frame update
    void Start()
    {

        //stageIndexs = FindObjectsOfType<SelectSceneManager>();

        //FindObjectsOfType<SelectSceneManager>()だけだと配列に入ってくる順番が不明瞭なため、
        //きちんとステージ番号順で管理する
        stageIndexs = FindObjectsOfType<SelectSceneManager>().OrderBy(obj => obj.StageIndex).ToArray();

        if (stageIndexs.Length > 0)
        {
            MoveToStagePoint(currentIndex);
        }

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentIndex);
        Debug.Log(stageIndexs[currentIndex].StageIndex);
        Debug.Log(stageIndexs[currentIndex].transform.position);

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            //Indexは0から始まるため、範囲を超えないように
            if (stageIndexs.Length -1  > currentIndex)
            {
                currentIndex += 1;
                MoveToStagePoint(currentIndex);
            }
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(currentIndex > 0) currentIndex -= 1;
            MoveToStagePoint(currentIndex);
        }

    }

    private void MoveToStagePoint(int Index)
    {
        // プレイヤーの位置をステージポイントの位置に移動
        transform.position = stageIndexs[Index].transform.position;
    }


}
