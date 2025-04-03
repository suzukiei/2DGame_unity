using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss_Bee : MonoBehaviour,Enemy
{


    // 蜂の状態を表すenum
    public enum BeeState { Idle, Chase, Attack, Return }
    private BeeState currentState = BeeState.Idle;

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
    [Header("リターン設定")]
    [SerializeField] private float returnSpeed = 2f; // 戻る速度
    [SerializeField] private float returnDelay = 3f; // 視界から外れてから戻り始めるまでの遅延

    private float attackTimer = 0f;
    private float directionChangeTimer = 0f;
    private Vector2 randomOffset;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private float wobbleTime = 0f;
    private bool isFloor = false;


    private Vector3 initialPosition; // 初期位置を保存
    private float outOfRangeTimer = 0f; // 視界外にいる時間
    private bool isReturning = false; // 初期位置に戻っているか

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        randomOffset = Random.insideUnitCircle * maxRandomOffset;
        initialPosition = transform.position;

        // コライダー設定
        CapsuleCollider2D[] colliders = GetComponents<CapsuleCollider2D>();
        if (colliders.Length == 2)
        {
            colliders[0].isTrigger = false; // プレイヤー用
            colliders[1].isTrigger = true;  // 床用
        }
    }

    void Update()
    {
        if (player == null) return;

        wobbleTime += Time.deltaTime;
        UpdateRandomOffset();

        // 現在の状態に基づいて行動を決定
        float distance = Vector2.Distance(transform.position, player.position);

        // 状態更新
        switch (currentState)
        {
            case BeeState.Idle:
                if (distance <= detectionRange)
                    currentState = BeeState.Chase;
                break;

            case BeeState.Chase:
                if (distance <= attackRange)
                    currentState = BeeState.Attack;
                else if (distance > detectionRange)
                {
                    outOfRangeTimer += Time.deltaTime;
                    if (outOfRangeTimer >= returnDelay)
                    {
                        currentState = BeeState.Return;
                        Debug.Log("帰還開始");
                    }
                }
                else
                    MoveTowardsPlayer();
                break;

            case BeeState.Attack:
                MoveTowardsPlayer();
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackInterval)
                {
                    Attack();
                    attackTimer = 0f;
                }
                if (distance > attackRange)
                    currentState = BeeState.Chase;
                break;

            case BeeState.Return:
                if (distance <= detectionRange)
                {
                    currentState = BeeState.Chase;
                    outOfRangeTimer = 0f;
                }
                else
                {
                    float distToInitial = Vector2.Distance(transform.position, initialPosition);
                    if (distToInitial < 0.1f)
                    {
                        transform.position = initialPosition;
                        rb.velocity = Vector2.zero;
                        currentState = BeeState.Idle;
                        outOfRangeTimer = 0f;
                        Debug.Log("初期位置に戻りました");
                    }
                    else
                        ReturnToInitialPosition();
                }
                break;
        }
    }

    void UpdateRandomOffset()
    {
        directionChangeTimer += Time.deltaTime;
        if (directionChangeTimer >= changeDirectionInterval)
        {
            randomOffset = Random.insideUnitCircle * maxRandomOffset;
            directionChangeTimer = 0f;
        }
    }

    Vector2 CalculateWobbleEffect()
    {
        return new Vector2(
            Mathf.Sin(wobbleTime * wobbleFrequency) * wobbleAmplitude,
            Mathf.Cos(wobbleTime * wobbleFrequency * 0.7f) * wobbleAmplitude
        );
    }

    void MoveTowardsPlayer()
    {
        // プレイヤーの上空目標位置を計算
        Vector2 targetPos = new Vector2(player.position.x, player.position.y + hoverHeight)
                           + randomOffset + CalculateWobbleEffect();

        // 境界制限
        if (leftBoundary && rightBoundary && topBoundary && bottomBoundary)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, leftBoundary.position.x, rightBoundary.position.x);
            targetPos.y = Mathf.Clamp(targetPos.y, bottomBoundary.position.y, topBoundary.position.y);
        }

        // 目標位置への方向を計算
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        Vector2 desiredVelocity = direction * moveSpeed;

        // 壁チェック
        RaycastHit2D wallHit = Physics2D.Raycast(
            transform.position, new Vector2(direction.x, 0), 0.5f,
            LayerMask.GetMask("Floor")
        );

        if (wallHit.collider != null)
        {
            float wallHeight = wallHit.collider.bounds.max.y;
            if (transform.position.y < wallHeight)
                desiredVelocity = new Vector2(rb.velocity.x * 0.5f, moveSpeed * 0.8f);
        }

        // 速度・向き・傾き更新
        rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, Time.deltaTime * 3f);
        if (direction.x != 0)
            transform.localScale = new Vector3(
                direction.x < 0 ? -originalScale.x : originalScale.x,
                originalScale.y, originalScale.z
            );
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Clamp(rb.velocity.x * -5f, -20f, 20f));
    }

    void Attack()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            Vector2 dirToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            bulletRb.velocity = (Vector2.down + dirToPlayer * 0.5f) * 5f;
        }
    }

    void ReturnToInitialPosition()
    {
        Vector2 direction = ((Vector2)initialPosition - (Vector2)transform.position).normalized;
        Vector2 desiredVelocity = direction * returnSpeed;

        // 蜂らしい動きを維持
        rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity + CalculateWobbleEffect() * 0.5f, Time.deltaTime * 3f);

        // 向きと傾きの更新
        if (direction.x != 0)
            transform.localScale = new Vector3(
                direction.x < 0 ? -originalScale.x : originalScale.x,
                originalScale.y, originalScale.z
            );
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Clamp(rb.velocity.x * -5f, -20f, 20f));
    }

    public void PlayerDamage(Player player)
    {
        player.Damage(damage);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            ContactPoint2D contact = other.GetContact(0);
            Vector2 normal = contact.normal;

            if (normal.y > 0.5f)
                rb.velocity = new Vector2(rb.velocity.x, moveSpeed * bounceForce);
            else if (Mathf.Abs(normal.x) > 0.5f)
                rb.velocity = new Vector2(normal.x * moveSpeed, rb.velocity.y);
        }
    }

    void OnDrawGizmosSelected()
    {
        // 追跡範囲と攻撃範囲を可視化
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 境界の可視化
        if (leftBoundary && rightBoundary && topBoundary && bottomBoundary)
        {
            Gizmos.color = Color.cyan;
            Vector3 tL = new Vector3(leftBoundary.position.x, topBoundary.position.y);
            Vector3 tR = new Vector3(rightBoundary.position.x, topBoundary.position.y);
            Vector3 bL = new Vector3(leftBoundary.position.x, bottomBoundary.position.y);
            Vector3 bR = new Vector3(rightBoundary.position.x, bottomBoundary.position.y);

            Gizmos.DrawLine(tL, tR);
            Gizmos.DrawLine(bL, bR);
            Gizmos.DrawLine(tL, bL);
            Gizmos.DrawLine(tR, bR);
        }
    }
}