using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonRankingStorage
{
    private static readonly string fileName = "ranking.json";
    private static readonly string filePath = Path.Combine(Application.persistentDataPath, fileName);

    public static List<RankingEntry> LoadRanking()
    {
        if (!File.Exists(filePath))
        {
            return new List<RankingEntry>();
        }

        string json = File.ReadAllText(filePath);
        RankingListWrapper wrapper = JsonUtility.FromJson<RankingListWrapper>(json);
        return wrapper.entries;
    }

    public static void SaveRanking(List<RankingEntry> entries)
    {
        entries.Sort((a, b) => a.time.CompareTo(b.time)); // ëÅÇ¢èáÇ…É\Å[Ég
        RankingListWrapper wrapper = new RankingListWrapper { entries = entries };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Ranking saved to local JSON.");
        Debug.Log("Saving JSON to: " + filePath); // SaveRankingì‡
    }

    [System.Serializable]
    private class RankingListWrapper
    {
        public List<RankingEntry> entries;
    }
}

