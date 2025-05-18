using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ClearTImeViewScript : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> TimeGUI;
    [SerializeField]
    private TextMeshProUGUI myTimeGUI;
    private void Start()
    {
        for(int i =0;i>TimeGUI.Count;i++)
        {
            //�����ɍ��܂ł̐�т�����
        }
        myTimeGUI.text =GameManager.Instance.gameClearTime.ToString();
        AddNewRanking("sasaki", 10);
    }
    // Update is called once per frame
    void Update()
    {
       
    }
    public void AddNewRanking(string playerNumber, float time)
    {
        var newEntry = new RankingEntry
        {
            date = System.DateTime.Now.ToString(),
            playerNumber = playerNumber,
            time = time
        };

        List<RankingEntry> current = JsonRankingStorage.LoadRanking();
        current.Add(newEntry);
        JsonRankingStorage.SaveRanking(current);
    }

    //�����ł��Ȃ��Ƃ����Ȃ�����
    //�܂�Google�X�v���b�g�V�[�g���牽�Ԗڂ̂̒���҂���Number�Ƃ���ClearTime�̃��X�g���擾����
    //�C���^�[�l�b�g�̐ڑ����Ȃ������肵�Ď擾�ł��Ȃ��玖�O�ɕێ����Ă���Json�t�@�C������f�[�^���擾����
    //���X�g�Ɋi�[����i�[�ł��炽��`����s��
    //�擾�������X�g�ƍ���̃v���C���[�̃f�[�^���r����Clear�̑������ɕ��ёւ���
    //���ёւ����f�[�^��Json�t�@�C���Ɋi�[��Google�X�v���b�g�V�[�g�̃f�[�^��V�K�ŕ��ёւ��������f�[�^�[�ɒu��������

}
