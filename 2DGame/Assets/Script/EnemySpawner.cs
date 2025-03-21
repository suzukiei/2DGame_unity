using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField, Header("敵オブジェクト")] private GameObject enemy;

    private Player player;
    private GameObject Enemyobj;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        Enemyobj = null;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        Vector3 cameraPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height)); //画面右上を取得、スクリーン座標をワールド座標に変換
        Vector3 scale = enemy.transform.lossyScale; //生成する敵の大きさを取得

        float distance = Vector2.Distance(transform.position,new Vector2(playerPos.x,transform.position.y));//引数どうしの距離を出す　xのみ
        float spawnDis = Vector2.Distance(playerPos, new Vector2(cameraPos.x + scale.x / 2.0f, playerPos.y));

        if (distance <= spawnDis && Enemyobj == null)
        {
            Enemyobj = Instantiate(enemy);
            Enemyobj.transform.position = transform.position;
            transform.parent = Enemyobj.transform;

            Debug.Log("EnemyInstantiate");
        }
    }
}
