using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpown : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spownTimer;
    [SerializeField] private float spowndistance;
    [SerializeField]
    private GameObject player;
    private bool spownflag;
    private float timer;
    private void Start()
    {
        player = GameObject.Find("Player");
        spownflag = false;
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;
        //プレイヤーが近くにいないかを確認する
        spownDistanceCheck();
        //時間を計算
        calculateTime();
        //エネミーの生成処理
        createEnemy();
    }
    private void spownDistanceCheck()
    {
        if (Mathf.Abs(player.transform.position.x - this.transform.position.x) <= spowndistance)
            spownflag = false;
        else
            spownflag = true;
    }
    private void calculateTime()
    {
        if (!spownflag) return;
        timer += Time.deltaTime;
    }
    private void createEnemy()
    {
        if (timer <= spownTimer) return;
        timer = 0;
        Instantiate(enemyPrefab,this.transform.position, Quaternion.identity);
    }
}
