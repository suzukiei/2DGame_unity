using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageSelector : MonoBehaviour
{
    //�X�e�[�W�|�C���g�Ǘ�
    private SelectSceneManager[] stageIndexs;
    private int currentIndex;

    // Start is called before the first frame update
    void Start()
    {

        //stageIndexs = FindObjectsOfType<SelectSceneManager>();

        //FindObjectsOfType<SelectSceneManager>()�������Ɣz��ɓ����Ă��鏇�Ԃ��s���ĂȂ��߁A
        //������ƃX�e�[�W�ԍ����ŊǗ�����
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
            //Index��0����n�܂邽�߁A�͈͂𒴂��Ȃ��悤��
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
        // �v���C���[�̈ʒu���X�e�[�W�|�C���g�̈ʒu�Ɉړ�
        transform.position = stageIndexs[Index].transform.position;
    }


}
