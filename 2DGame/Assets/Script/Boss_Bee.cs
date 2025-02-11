using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bee : MonoBehaviour
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackInterval = 2f;

    [SerializeField] private float moveSpeed = 3f;      // 移動速度
    [SerializeField] private float detectionRange = 8f; // 追跡を開始する距離
    [SerializeField] private float hoverHeight = 3f;    // プレイヤーの上何メートルの位置を維持するか

    private float attackTimer = 0f;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 originalScale;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 追跡処理
        if (distance <= detectionRange)
        {
            MoveTowardsPlayer();
        }

        // 攻撃処理
        if (distance <= attackRange)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                Attack();
                attackTimer = 0f;
            }
        }
    }

    void MoveTowardsPlayer()
    {
        // プレイヤーの上空の目標位置を計算
        Vector2 targetPosition = new Vector2(player.position.x, player.position.y + hoverHeight);

        // 現在位置から目標位置への方向を計算
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // 移動を適用
        rb.velocity = direction * moveSpeed;

        // 必要に応じて左右の向きを変更
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(
                direction.x < 0 ? -originalScale.x : originalScale.x,
                originalScale.y,
                originalScale.z
            );
        }
    }

    void Attack()
    {
        //// 既存の攻撃処理
        //if (animator != null)
        //{
        //    animator.SetTrigger("Attack");
        //}

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.velocity = Vector2.down * 5f;
        }
    }

    // 追跡範囲と攻撃範囲を可視化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
