using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBoss : MonoBehaviour, Enemy
{
    // スカルボスの状態を表すenum
    public enum SkullState { Waiting, Moving, Vulnerable, Defeated }
    private SkullState currentState = SkullState.Waiting;

    [Header("移動設定")]
    [SerializeField] private Transform[] waypoints; // 移動する経路ポイント
    [SerializeField] private float moveSpeed = 3f; // 移動速度
    [SerializeField] private float waypointReachDistance = 0.1f; // ウェイポイントに到達したとみなす距離

    [Header("弾発射設定")]
    [SerializeField] private GameObject bulletPrefab; // 弾のプレハブ
    [SerializeField] private float fireRate = 1f; // 発射間隔（秒）
    [SerializeField] private float bulletSpeed = 5f; // 弾の速度
    [SerializeField] private int burstCount = 3; // 一度に発射する弾の数
    [SerializeField] private float burstInterval = 0.2f; // バースト内の発射間隔

    [Header("ステート設定")]
    [SerializeField] private float invinciblePhaseDuration = 20f; // 無敵状態の継続時間
    [SerializeField] private float vulnerablePhaseDuration = 10f; // 脆弱状態の継続時間
    [SerializeField] private int maxHealth = 20; // 最大HP
    [SerializeField] private int damage = 1; // プレイヤーへの接触ダメージ

    [Header("境界")]
    [SerializeField] private Transform leftBoundary;  // 左の境界
    [SerializeField] private Transform rightBoundary; // 右の境界
    [SerializeField] private Transform topBoundary;   // 上の境界
    [SerializeField] private Transform bottomBoundary;// 下の境界

    // 非公開変数
    private int currentHealth;
    private int currentWaypoint = 0;
    private bool isBattleStarted = false;
    private bool isInvincible = true;
    private float lastFireTime;
    private float originalRadius;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform playerTransform;
    private Vector3 originalScale;
    private bool isFiring = false;
    private string playerTag = "Player";
    private CircleCollider2D SkullCollider;
    

    private void Start()
    {
        // コンポーネント取得
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        SkullCollider = GetComponent<CircleCollider2D>();

        // 初期化
        currentHealth = maxHealth;
        if (SkullCollider is CircleCollider2D circleCollider)
            originalRadius = circleCollider.radius;
        // アニメーターの初期設定
        if (animator != null)
        {
            animator.SetBool("IsDown", false);
        }

        // プレイヤー検索
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    private void Update()
    {
        // プレイヤーがいるか確認
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                return; // プレイヤーが見つからない
            }
        }

        // プレイヤーが境界内にいるか確認
        if (!isBattleStarted)
        {
            bool isPlayerInBoundary = IsPointInBoundary(playerTransform.position);

            if (isPlayerInBoundary)
            {
                StartBattle();
            }
        }

        // ステート処理
        switch (currentState)
        {
            case SkullState.Waiting:
                // 待機中は何もしない
                break;

            case SkullState.Moving:
                MoveAlongPath();

                // 一定間隔で攻撃
                if (!isFiring && Time.time > lastFireTime + fireRate)
                {
                    StartCoroutine(FireBurst());
                    lastFireTime = Time.time;
                }
                break;

            case SkullState.Vulnerable:
                // 脆弱状態では動かない
                rb.velocity = Vector2.zero;
                break;

            case SkullState.Defeated:
                // 倒された状態
                rb.velocity = Vector2.zero;
                break;
        }
    }

    // 指定した点が境界内にあるかチェック
    private bool IsPointInBoundary(Vector3 point)
    {
        if (leftBoundary == null || rightBoundary == null ||
            topBoundary == null || bottomBoundary == null)
        {
            Debug.LogWarning("境界が設定されていません");
            return false;
        }

        bool isInX = point.x >= leftBoundary.position.x && point.x <= rightBoundary.position.x;
        bool isInY = point.y >= bottomBoundary.position.y && point.y <= topBoundary.position.y;

        return isInX && isInY;
    }

    // 戦闘開始
    private void StartBattle()
    {
        isBattleStarted = true;
        Debug.Log("ボス戦闘開始！");

        // 無敵状態を設定
        isInvincible = true;

        // 移動開始
        currentState = SkullState.Moving;

        // 定期的に弾を発射するための初期設定
        lastFireTime = Time.time;

        // フェーズタイマー開始
        StartCoroutine(PhaseController());
    }

    // 経路に沿って移動
    private void MoveAlongPath()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // 現在のウェイポイントが有効か確認
        if (waypoints[currentWaypoint] == null)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            return;
        }

        // 現在の目標ポイントへ移動
        Vector2 targetPosition = waypoints[currentWaypoint].position;
        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;

        // 移動速度を設定
        rb.velocity = moveDirection * moveSpeed;

        // 目標ポイントに近づいたら次のポイントに
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        if (distanceToTarget < waypointReachDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;

            // ポイント到達時に攻撃
            if (!isFiring)
            {
                StartCoroutine(FireBurst());
            }
        }

        transform.localScale = originalScale; // 常に元の向きを維持
    }

    // バースト射撃
    private IEnumerator FireBurst()
    {
        isFiring = true;
        for (int i = 0; i < burstCount; i++)
        {
            FireBullet();
            yield return new WaitForSeconds(burstInterval);
        }

        isFiring = false;
    }

    // 弾を発射
    private void FireBullet()
    {
        if (bulletPrefab == null || playerTransform == null) return;

        // 弾を生成
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            // プレイヤーへの方向を計算（直線的に発射）
            Vector2 directionToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;

            // 弾に速度を設定
            bulletRb.velocity = directionToPlayer * bulletSpeed;

            // 弾の角度をプレイヤーに向ける（必要に応じて）
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    // フェーズコントローラー
    private IEnumerator PhaseController()
    {
        while (currentHealth > 0 && currentState != SkullState.Defeated)
        {
            // 無敵フェーズ (Idleアニメーション)
            isInvincible = true;
            currentState = SkullState.Moving;

            // 物理設定を通常状態に
            rb.gravityScale = 0f; // 浮遊状態で重力なし
                SkullCollider.radius = originalRadius; // オリジナル半径を保存しておく

            // アニメーション設定
            if (animator != null)
            {
                // IsDown パラメータをfalseに設定
                animator.SetBool("IsDown", false);
            }

            yield return new WaitForSeconds(invinciblePhaseDuration);

            // 脆弱フェーズ (Downアニメーション)
            isInvincible = false;
            currentState = SkullState.Vulnerable;


            // 物理設定を落下状態に変更
            rb.velocity = Vector2.zero; // まず速度をリセット
            rb.gravityScale = 20f; // 重力を強めに設定して急速に落下させる
            SkullCollider.radius = originalRadius * 0.5f;

            // アニメーション設定
            if (animator != null)
            {
                // IsDown パラメータをtrueに設定
                animator.SetBool("IsDown", true);
            }

            yield return new WaitForSeconds(vulnerablePhaseDuration);
        }
    }

    // プレイヤーからのダメージを処理
    public void ReceiveDamage(int damage)
    {
        // 無敵状態ならダメージを無効化
        if (isInvincible || currentState == SkullState.Defeated) return;

        currentHealth -= damage;
        Debug.Log($"ボスが{damage}ダメージを受けました！残りHP: {currentHealth}");

        // HPが0になったら倒された状態に
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // ダメージアニメーション再生 (Hitステート)
            if (animator != null)
            {
                animator.SetTrigger("IsHit");
            }
        }
    }

    // 倒された処理
    private void Die()
    {
        currentState = SkullState.Defeated;
        Debug.Log("ボスを倒しました！");

        // 死亡アニメーション再生
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // コライダー無効化
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Rigidbody停止
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // 数秒後に破壊
        Destroy(gameObject, 3f);
    }

    // プレイヤーの弾に当たった時
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            // プレイヤーの弾に当たった
            ReceiveDamage(1);

            // 弾を破壊
            Destroy(other.gameObject);
        }
    }

    // プレイヤーと衝突した時
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            // プレイヤーにダメージを与える
            PlayerDamage(collision.gameObject.GetComponent<Player>());
        }
    }

    // プレイヤーにダメージを与える
    public void PlayerDamage(Player player)
    {
        if (player != null)
        {
            player.Damage(damage);
        }
    }

    // Gizmo描画 (エディタ用)
    private void OnDrawGizmosSelected()
    {
        // 境界を表示
        if (leftBoundary != null && rightBoundary != null &&
            topBoundary != null && bottomBoundary != null)
        {
            Gizmos.color = Color.cyan;

            Vector3 tL = new Vector3(leftBoundary.position.x, topBoundary.position.y);
            Vector3 tR = new Vector3(rightBoundary.position.x, topBoundary.position.y);
            Vector3 bL = new Vector3(leftBoundary.position.x, bottomBoundary.position.y);
            Vector3 bR = new Vector3(rightBoundary.position.x, bottomBoundary.position.y);

            Gizmos.DrawLine(tL, tR); // 上辺
            Gizmos.DrawLine(bL, bR); // 下辺
            Gizmos.DrawLine(tL, bL); // 左辺
            Gizmos.DrawLine(tR, bR); // 右辺
        }

        // 経路を表示
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Vector3 currentPos = waypoints[i].position;
                    Vector3 nextPos = waypoints[(i + 1) % waypoints.Length].position;
                    Gizmos.DrawLine(currentPos, nextPos);
                    Gizmos.DrawSphere(currentPos, 0.2f);
                }
            }
        }
    }
}
