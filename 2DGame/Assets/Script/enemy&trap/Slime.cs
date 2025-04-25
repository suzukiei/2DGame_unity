using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : AttackEnemy
{
    [Header("スライム設定")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpInterval = 2f;

    [Header("分裂設定")]
    [SerializeField] private GameObject smallSlimePrefab;
    [SerializeField] private int splitCount = 2; // 分裂数
    [SerializeField] private float splitForce = 8f;
    [SerializeField] private bool canSplit = true; // 分裂可能かどうか


    // 状態管理
    private float lastJumpTime = 0f;
    private bool isProcessingSplit = false; // 分裂処理中かどうか

    protected override void Start()
    {
        base.Start();
        lastJumpTime = Time.time - Random.Range(0f, jumpInterval * 0.5f);
    }

    protected override void Update()
    {
        base.Update();

        // スライム特有のジャンプ処理
        if (bfloor && Time.time > lastJumpTime + jumpInterval)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        lastJumpTime = Time.time;
        StartCoroutine(SlimeStretchEffect());
    }

    IEnumerator SlimeStretchEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 stretchScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z);

        transform.localScale = stretchScale;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    protected void HitFloor()
    {
        base.HitFloor();

        if (bfloor && rigid.velocity.y <= 0.1f)
        {
            StartCoroutine(SquashEffect());
        }
    }

    IEnumerator SquashEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 squashScale = new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z);

        transform.localScale = squashScale;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    // プレイヤーが衝突したときの処理
    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    // 分裂処理中なら無視
    //    if (isProcessingSplit) return;

    //    // プレイヤーとの衝突を検出
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        // 衝突情報を取得
    //        ContactPoint2D contact = collision.GetContact(0);
    //        Vector2 relativeVelocity = collision.relativeVelocity;

    //        Debug.Log($"スライムへの衝突: normal.y={contact.normal.y}, velocity.y={relativeVelocity.y}");

    //        // 踏みつけ判定：プレイヤーが上から降ってきている
    //        if (contact.normal.y < -0.3f)
    //        {
    //            Debug.Log("踏みつけ検出: 分裂処理を開始します");

    //            // プレイヤーを少し跳ね上げる（HitEnemyより先に実行）
    //            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
    //            if (playerRb != null)
    //            {
    //                playerRb.velocity = new Vector2(playerRb.velocity.x, 5f);
    //            }

    //            // 分裂処理
    //            if (canSplit && smallSlimePrefab != null)
    //            {
    //                isProcessingSplit = true;
    //                StartCoroutine(SplitAfterFrame());
    //            }
    //        }
    //    }
    //}

    // 1フレーム待ってから分裂する（衝突処理の順序問題を回避）
    IEnumerator SplitAfterFrame()
    {
        // 1フレーム待機（Player.HitEnemyが実行される時間を確保）
        yield return new WaitForEndOfFrame();
        Split();
    }

    // 基底クラスのReceiveDamage()を上書き
    public override void ReceiveDamage(int _hp,GameObject player=null)
    {
        // 分裂処理
        if (canSplit && smallSlimePrefab != null)
        {
            isProcessingSplit = true;
        }
        // 分裂処理中なら通常のダメージ処理をスキップ
        if (isProcessingSplit)
        {
            Debug.Log("分裂処理中のため、通常ダメージ処理をスキップします");
            // プレイヤーを少し跳ね上げる（HitEnemyより先に実行）
            Rigidbody2D playerRb = player.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, 5f);
            }
            // 分裂処理
            if (canSplit && smallSlimePrefab != null)
            {
                isProcessingSplit = true;
                StartCoroutine(SplitAfterFrame());
            }
            // 分裂処理
            return;
        }

        // 通常のダメージ処理
        Debug.Log("通常ダメージ処理を実行します");
        base.ReceiveDamage(_hp);
    }

    // 分裂処理
    private void Split()
    {
        Debug.Log("スライムが分裂します！");

        for (int i = 0; i < splitCount; i++)
        {
            // 左右どちらかを決定（偶数番目は左、奇数番目は右）
            float xDirection = (i % 2 == 0) ? -1f : 1f;

            // 生成位置をわずかにずらす
            Vector3 spawnPosition = transform.position + new Vector3(xDirection * 1f, 0.1f, 0);

            // 小さなスライムを生成
            GameObject smallSlime = Instantiate(
                smallSlimePrefab,
                spawnPosition,
                Quaternion.identity
            );

            // 子スライムの設定
            Rigidbody2D smallRb = smallSlime.GetComponent<Rigidbody2D>();
            if (smallRb != null)
            {
                // 左右に飛び散るようにシンプルな方向を設定
                Vector2 direction = new Vector2(xDirection * 3f, 3f);

                // 方向を正規化して力を加える
                smallRb.AddForce(direction.normalized * splitForce, ForceMode2D.Impulse);
            }

            // 子スライムの分裂設定を無効化
            SlimeEnemy smallSlimeController = smallSlime.GetComponent<SlimeEnemy>();
            if (smallSlimeController != null)
            {
                smallSlimeController.canSplit = false;
            }
        }


        // 親スライムを破壊
        EnemyManager.Instance.DestroyEnemyObjList(this.gameObject);
        Destroy(gameObject);
    }
}