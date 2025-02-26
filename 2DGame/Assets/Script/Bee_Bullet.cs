using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee_Bullet : MonoBehaviour,Enemy
{
    [SerializeField] private float lifeTime = 3f;  // 弾の生存時間
    [SerializeField] private int damage = 1;       // 与えるダメージ

    void Start()
    {
        // 一定時間後に弾を削除
        Destroy(gameObject, lifeTime);


        // 2つのコライダーの設定を確認
        CircleCollider2D[] colliders = GetComponents<CircleCollider2D>();
        if (colliders.Length == 2)
        {
            // 1つ目のコライダー：プレイヤー用（物理衝突）
            colliders[0].isTrigger = false;

            // 2つ目のコライダー：床用（トリガー）
            colliders[1].isTrigger = true;
        }
    }

    public void PlayerDamage(Player player)
    {
        player.Damage(damage);
        Destroy(gameObject);

    }

    // 床とのトリガー判定
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        // プレイヤーにダメージを与える処理
    //        var player = other.GetComponent<Player>(); // プレイヤーの体力管理スクリプト
    //        if (player != null)
    //        {
    //            player.Damage(damage);
    //        }
    //        Destroy(gameObject);
    //    }
    //    else if (other.CompareTag("Floor")) // 地面に当たった場合
    //    {
    //        Destroy(gameObject);
    //    }
    //}
}
