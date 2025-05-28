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
    private TextMeshProUGUI myPlayerIDGUI;
    [SerializeField]
    private TextMeshProUGUI myTimeGUI;
    private void Start()
    {
        for(int i =0;i>TimeGUI.Count;i++)
        {
            //ここに今までの戦績を入れる
        }
        myPlayerIDGUI.text = GameManager.Instance.PlayerID.ToString();
        myTimeGUI.text =GameManager.Instance.gameClearTime.ToString();
        AddNewRanking("ID:"+GameManager.Instance.PlayerID,GameManager.Instance.gameClearTime);
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

    //ここでやらないといけないこと
    //まずGoogleスプレットシートから何番目のの挑戦者かのNumberとそのClearTimeのリストを取得する
    //インターネットの接続がなかったりして取得できなたら事前に保持しているJsonファイルからデータを取得して
    //リストに格納する格納できらたら描画を行う
    //取得したリストと今回のプレイヤーのデータを比較してClearの早い順に並び替える
    //並び替えたデータはJsonファイルに格納とGoogleスプレットシートのデータを新規で並び替えをしたデーターに置き換える

}
