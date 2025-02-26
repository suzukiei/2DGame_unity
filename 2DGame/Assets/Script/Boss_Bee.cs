using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss_Bee : MonoBehaviour,Enemy
{
    [SerializeField,Header("弾のプレファブ")] private GameObject bulletPrefab;
    [SerializeField,Header("攻撃範囲")] private float attackRange = 5f;
    [SerializeField,Header("衝突時攻撃力")] private int damage = 1;
    [SerializeField,Header("次の攻撃までの間隔")] private float attackInterval = 2f;
    [SerializeField,Header("移動速度")] private float moveSpeed = 3f;      // 移動速度
    [SerializeField,Header("プレイヤーを追跡する範囲")] private float detectionRange = 8f; // 追跡を開始する距離
    [SerializeField,Header("滞空距離")] private float hoverHeight = 3f;    // プレイヤーの上何メートルの位置を維持するか
    [SerializeField,Header("ブロック衝突時反発力")] private float bounceForce = 5f;    // 反発力

    // 蜂の動きをよりリアルにするためのパラメータ
    [SerializeField,Header("蜂の揺れ幅")] private float wobbleAmplitude = 0.5f;    // 揺れの振幅
    [SerializeField,Header("蜂が揺れる頻度")] private float wobbleFrequency = 10f;     // 揺れの頻度
    [SerializeField,Header("方向転換の間隔")] private float changeDirectionInterval = 0.7f; // 方向転換の間隔
    [SerializeField] private float maxRandomOffset = 1.5f;    // ランダムな動きの最大オフセット
    [SerializeField,Header("境界")] private Transform leftBoundary;  // 左の境界
    [SerializeField] private Transform rightBoundary; // 右の境界
    [SerializeField] private Transform topBoundary;   // 上の境界
    [SerializeField] private Transform bottomBoundary;// 下の境界

    private float attackTimer = 0f;
    private float directionChangeTimer = 0f;
    private Vector2 randomOffset;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private float wobbleTime = 0f;
    private bool isFloor = false; 

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        randomOffset = GetRandomOffset();

        // 2つのコライダーの設定を確認
        CapsuleCollider2D[] colliders = GetComponents<CapsuleCollider2D>();
        if (colliders.Length == 2)
        {
            // 1つ目のコライダー：プレイヤー用（物理衝突）
            colliders[0].isTrigger = false;

            // 2つ目のコライダー：床用（トリガー）
            colliders[1].isTrigger = true;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        //Playerから蜂までの距離を計算
        float distance = Vector2.Distance(transform.position, player.position);

        //移動範囲内よりも距離が近ければPlayerを追尾
        //そうでなければ即座に停止
        if (distance <= detectionRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);

        wobbleTime += Time.deltaTime;
        UpdateRandomOffset();

        

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

        //境界を定めている場合d
        if (leftBoundary != null && rightBoundary != null && topBoundary != null && bottomBoundary != null)
        {
            //境界内に位置を制限する
            targetPosition.x = Mathf.Clamp(targetPosition.x, leftBoundary.position.x, rightBoundary.position.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, bottomBoundary.position.y, topBoundary.position.y);
        }

        // 現在位置から目標位置への方向を計算
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        //// 移動を適用（少し加速度を使うとより自然に）
        //Vector2 desiredVelocity = direction * moveSpeed;
        //rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, Time.deltaTime * 3f);      


        // 移動前の壁チェック
        float rayDistance = 0.5f;
        RaycastHit2D wallHit = Physics2D.Raycast(
            transform.position,
            new Vector2(direction.x, 0),
            rayDistance,
            LayerMask.GetMask("Floor")
        );

        Vector2 desiredVelocity;

        if (wallHit.collider != null)
        {
            // 壁が検出された場合の処理
            // 壁の高さを確認して、少しだけ上に移動
            float wallHeight = wallHit.collider.bounds.max.y;
            if (transform.position.y < wallHeight)
            {
                // 現在の横方向の速度を維持しながら、ゆっくりと上昇
                desiredVelocity = new Vector2(
                    rb.velocity.x * 0.5f, // 横方向の速度を少し減速
                    moveSpeed * 0.8f      // 適度な上昇速度
                );
            }
            else
            {
                // 壁の上に到達したら通常の追跡に戻る
                desiredVelocity = direction * moveSpeed;
            }
        }
        else
        {
            // 通常の追跡
            desiredVelocity = direction * moveSpeed;
        }

        // 速度の適用（急激な変化を防ぐ）
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

    //Player衝突時
    public void PlayerDamage(Player player)
    {
        player.Damage(damage);
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

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            ContactPoint2D contact = other.GetContact(0);
            Vector2 normal = contact.normal;

            // 上向きの衝突の場合（床の上面との衝突）
            if (normal.y > 0.5f)
            {
                // より強い上向きの反発力を加える
                rb.velocity = new Vector2(rb.velocity.x, moveSpeed * bounceForce);
            }
            // 横からの衝突の場合
            else if (Mathf.Abs(normal.x) > 0.5f)
            {
                // 横方向への反発
                rb.velocity = new Vector2(normal.x * moveSpeed, rb.velocity.y);
            }
        }
    }


    // 追跡範囲と攻撃範囲を可視化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (leftBoundary != null && rightBoundary != null && topBoundary != null && bottomBoundary != null)
        {
            Gizmos.color = Color.cyan;
            // 境界の四隅の座標
            Vector3 topLeft = new Vector3(leftBoundary.position.x, topBoundary.position.y);
            Vector3 topRight = new Vector3(rightBoundary.position.x, topBoundary.position.y);
            Vector3 bottomLeft = new Vector3(leftBoundary.position.x, bottomBoundary.position.y);
            Vector3 bottomRight = new Vector3(rightBoundary.position.x, bottomBoundary.position.y);

            // 上下左右の境界線を描画
            Gizmos.DrawLine(topLeft, topRight);     // 上辺
            Gizmos.DrawLine(bottomLeft, bottomRight); // 下辺
            Gizmos.DrawLine(topLeft, bottomLeft);     // 左辺
            Gizmos.DrawLine(topRight, bottomRight);   // 右辺
        }
    }
}