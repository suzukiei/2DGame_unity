using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee_Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;  // 弾の生存時間
    [SerializeField] private int damage = 1;       // 与えるダメージ

    void Start()
    {
        // 一定時間後に弾を削除
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーにダメージを与える処理
            var player = other.GetComponent<Player>(); // プレイヤーの体力管理スクリプト
            if (player != null)
            {
                player.Damage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Floor")) // 地面に当たった場合
        {
            Destroy(gameObject);
        }
    }
}
