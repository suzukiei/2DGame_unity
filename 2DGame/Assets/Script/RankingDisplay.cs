using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingDisplay : MonoBehaviour
{
    public Transform contentParent; // Content �� Transform
    public GameObject rankingItemPrefab; // Text �܂��� �s�̃v���n�u

    public void DisplayRanking(List<RankingEntry> entries)
    {
        // ������UI���N���A
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // �f�[�^�����ԏ��Ƀ\�[�g
        entries.Sort((a, b) => a.time.CompareTo(b.time));

        // UI����
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            GameObject item = Instantiate(rankingItemPrefab, contentParent);
            var text = item.GetComponent<TMP_Text>();
            if (text != null)
            {
                text.text = $"{i + 1}. {entry.playerNumber} - {entry.time:F2}�b";
            }
            text.enabled = true;
        }
    }

    // �f�o�b�O�p�� Start() �Ń��[�J���\�����\
    void Start()
    {
        var localRanking = JsonRankingStorage.LoadRanking();
        DisplayRanking(localRanking);
    }
}
