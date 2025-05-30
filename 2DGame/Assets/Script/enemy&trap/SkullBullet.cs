using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBullet : MonoBehaviour,Enemy
{
    [SerializeField] private int damage = 1; // 弾のダメージ量
    [SerializeField] private float lifeTime = 5f; // 弾の寿命

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 寿命を設定
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーに当たった場合
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage(damage);
            }

            // 弾を破壊
            DestroyBullet();
        }

        // 地面や壁に当たった場合
        if (other.CompareTag("Floor"))
        {

            // 弾を破壊
            DestroyBullet();
        }
    }

    // 弾を破壊
    private void DestroyBullet()
    {
       
            // アニメーターがなければ即時破壊
            Destroy(gameObject);
        
    }

    public void PlayerDamage(Player player)
    {
        player.Damage(damage);
        Destroy(gameObject);

    }
}
