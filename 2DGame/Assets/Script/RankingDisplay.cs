using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingDisplay : MonoBehaviour
{
    public Transform contentParent; // Content の Transform
    public GameObject rankingItemPrefab; // Text または 行のプレハブ

    public void DisplayRanking(List<RankingEntry> entries)
    {
        // 既存のUIをクリア
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // データを時間順にソート
        entries.Sort((a, b) => a.time.CompareTo(b.time));

        // UI生成
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            GameObject item = Instantiate(rankingItemPrefab, contentParent);
            var text = item.GetComponent<TMP_Text>();
            if (text != null)
            {
                text.text = $"{i + 1}. {entry.playerNumber} - {entry.time:F2}秒";
            }
            text.enabled = true;
        }
    }

    // デバッグ用に Start() でローカル表示も可能
    void Start()
    {
        var localRanking = JsonRankingStorage.LoadRanking();
        DisplayRanking(localRanking);
    }
}
