using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryPig : MonoBehaviour
{
    [Header("検出設定")]
    [SerializeField,Header("検出距離")] private float detectionRange = 5f; // プレイヤー検出距離
    [SerializeField,Header("怒り状態の速度")] private float AngrySpeed = 6f; // 怒り状態の速度

    [Header("ジャンプ設定")]
    [SerializeField] private float jumpForce = 5f; // ジャンプ力
    [SerializeField] private float jumpDelay = 0.2f; // ジャンプ前の待機時間

    // 参照
    private Transform playerTransform;
    private AttackEnemy attackEnemyScript;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isAngry = false;
    private bool isJumping = false;
    private bool Chase = false;

    void Start()
    {
        // 必要なコンポーネントを取得
        attackEnemyScript = GetComponent<AttackEnemy>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // プレイヤーを検索
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        animator.SetBool("IsWalking", true);
    }

    void Update()
    {
        if (playerTransform == null) return;

        // プレイヤーとの距離を計算
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // プレイヤーが範囲内に入ったかチェック
        if (distanceToPlayer <= detectionRange && !isAngry)
        {
            // 怒りモードに切替
            StartCoroutine(SwitchToAngryMode());
        }
        else if (distanceToPlayer > detectionRange && isAngry)
        {
            // 通常モードに戻る
            SwitchToNormalMode();
        }
    }

    IEnumerator SwitchToAngryMode()
    {
        isAngry = true;
        isJumping = true;
        Chase = false;
        // アニメーション変更（通常状態から怒り状態へ）
        animator.SetBool("IsRunning", true);
        animator.SetBool("IsWalking", false);

        yield return new WaitForSeconds(jumpDelay);

        // 現在の速度をリセット
        rb.velocity = new Vector2(0, rb.velocity.y);

        // ジャンプ
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // ジャンプが終わるまで待機（簡易的に0.5秒待機）
        yield return new WaitForSeconds(0.5f);

        // プレイヤーの方向を向く
        if (playerTransform.position.x < transform.position.x)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else
        {
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }

        // 追跡開始
        isJumping = false;
        Chase = true;
    }

    void SwitchToNormalMode()
    {
        isAngry = false;
        Chase = false;

        // アニメーション変更（怒り状態から通常状態へ）
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsWalking", true);
    }

    // 移動制御を行うLateUpdate（AttackEnemyのUpdate後に実行される）
    void LateUpdate()
    {
        if (!isAngry || isJumping || !Chase) return; // 怒りモードでない場合は処理しない

        // AttackEnemyから床判定情報を取得
        // ここでは直接bfloorにアクセスできないため、アニメーションのIsIdleパラメータから判断
        bool isOnFloor = !animator.GetBool("IsIdle");

        if (!isOnFloor) return; // 床の上にいない場合は処理しない

        // プレイヤー方向への移動ベクトルを計算
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // X方向の移動のみ適用（Y方向は重力に任せる）
        rb.velocity = new Vector2(directionToPlayer.x * AngrySpeed, rb.velocity.y);

        // 向きの更新
        if (directionToPlayer.x < 0.0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (directionToPlayer.x > 0.0f)
        {
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }
    }

    // デバッグ用に検出範囲を表示
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
