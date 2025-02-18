using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bee : MonoBehaviour
{
    [SerializeField,Header("弾のプレファブ")] private GameObject bulletPrefab;
    [SerializeField,Header("攻撃範囲")] private float attackRange = 5f;
    [SerializeField,Header("次の攻撃までの間隔")] private float attackInterval = 2f;
    [SerializeField,Header("移動速度")] private float moveSpeed = 3f;      // 移動速度
    [SerializeField,Header("プレイヤーを追跡する範囲")] private float detectionRange = 8f; // 追跡を開始する距離
    [SerializeField,Header("滞空距離")] private float hoverHeight = 3f;    // プレイヤーの上何メートルの位置を維持するか

    // 蜂の動きをよりリアルにするためのパラメータ
    [SerializeField,Header("蜂の揺れ幅")] private float wobbleAmplitude = 0.5f;    // 揺れの振幅
    [SerializeField,Header("蜂が揺れる頻度")] private float wobbleFrequency = 10f;     // 揺れの頻度
    [SerializeField,Header("方向転換の間隔")] private float changeDirectionInterval = 0.7f; // 方向転換の間隔
    [SerializeField] private float maxRandomOffset = 1.5f;    // ランダムな動きの最大オフセット

    private float attackTimer = 0f;
    private float directionChangeTimer = 0f;
    private Vector2 randomOffset;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private float wobbleTime = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        randomOffset = GetRandomOffset();
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);

        wobbleTime += Time.deltaTime;
        UpdateRandomOffset();

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

    void UpdateRandomOffset()
    {
        // 一定間隔でランダムな方向に変化させる
        directionChangeTimer += Time.deltaTime;
        if (directionChangeTimer >= changeDirectionInterval)
        {
            randomOffset = GetRandomOffset();
            directionChangeTimer = 0f;
        }
    }

    Vector2 GetRandomOffset()
    {
        // ランダムなオフセットを生成
        return new Vector2(
            Random.Range(-maxRandomOffset, maxRandomOffset),
            Random.Range(-maxRandomOffset, maxRandomOffset)
        );
    }

    Vector2 CalculateWobbleEffect()
    {
        // サイン波を使った揺れ効果
        float xWobble = Mathf.Sin(wobbleTime * wobbleFrequency) * wobbleAmplitude;
        float yWobble = Mathf.Cos(wobbleTime * wobbleFrequency * 0.7f) * wobbleAmplitude;

        return new Vector2(xWobble, yWobble);
    }

    void MoveTowardsPlayer()
    {
        // プレイヤーの上空の目標位置を計算
        Vector2 baseTargetPosition = new Vector2(player.position.x, player.position.y + hoverHeight);

        // ランダムなオフセットと揺れ効果を追加
        Vector2 wobbleEffect = CalculateWobbleEffect();
        Vector2 targetPosition = baseTargetPosition + randomOffset + wobbleEffect;

        // 現在位置から目標位置への方向を計算
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // 移動を適用（少し加速度を使うとより自然に）
        Vector2 desiredVelocity = direction * moveSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, Time.deltaTime * 3f);

        // 必要に応じて左右の向きを変更
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(
                direction.x < 0 ? -originalScale.x : originalScale.x,
                originalScale.y,
                originalScale.z
            );
        }

        // オプション: 動きに合わせて少し傾ける
        float tiltAngle = Mathf.Clamp(rb.velocity.x * -5f, -20f, 20f);
        transform.rotation = Quaternion.Euler(0, 0, tiltAngle);
    }

    void Attack()
    {
        //if (animator != null)
        //{
        //    animator.SetTrigger("Attack");
        //}
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            // プレイヤーに向かって弾を発射する（オプション）
            Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            bulletRb.velocity = (Vector2.down + directionToPlayer * 0.5f) * 5f;
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