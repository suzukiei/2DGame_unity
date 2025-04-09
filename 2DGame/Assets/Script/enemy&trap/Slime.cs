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
    [SerializeField] private float splitForce = 3f;
    [SerializeField] private bool canSplit = true; // 分裂可能かどうか

    // 状態管理
    private float lastJumpTime = 0f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isStomped = false;

    // 基底クラスのStart()を上書き
    new void Start()
    {
        // 基底クラスのStart()を呼び出し
        base.Start();

        // 必要なコンポーネント取得
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 最初のジャンプまでの時間をランダム化
        lastJumpTime = Time.time - Random.Range(0f, jumpInterval * 0.5f);
    }

    // 基底クラスのUpdate()を上書き
    new void Update()
    {
        // 基底クラスのUpdate()を呼び出し
        base.Update();

        // スライム特有のジャンプ処理
        if (base.bfloor && Time.time > lastJumpTime + jumpInterval)
        {
            Jump();
        }
    }

    private void Jump()
    {
        // ジャンプ実行
        rb.velocity = new Vector2(rb.velocity.x, 0); // Y速度をリセット
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        lastJumpTime = Time.time;

        // Run アニメーションは既存のものを使用するのでトリガー設定は不要
        // Anim.SetTrigger("Jump"); <- この行は削除

        // スライム特有の伸縮アニメーション
        StartCoroutine(SlimeStretchEffect());
    }

    // スライムの伸縮エフェクト
    IEnumerator SlimeStretchEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 stretchScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z);

        // ジャンプ時の伸び
        transform.localScale = stretchScale;
        yield return new WaitForSeconds(0.1f);

        // 元に戻す
        transform.localScale = originalScale;
    }

    // 着地時の処理を拡張
    new private void HitFloor()
    {
        base.HitFloor();

        // 床に着地した瞬間
        if (base.bfloor && rb.velocity.y <= 0.1f)
        {
            StartCoroutine(SquashEffect());
        }
    }

    // 着地時の潰れエフェクト
    IEnumerator SquashEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 squashScale = new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z);

        transform.localScale = squashScale;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    // 基底クラスのReceiveDamage()を上書き
    new public void ReceiveDamage(int _hp)
    {
        // 踏みつけられた場合は分裂処理
        if (isStomped && canSplit && smallSlimePrefab != null)
        {
            Split();
        }
        else
        {
            // 通常のダメージ処理
            base.ReceiveDamage(_hp);
        }
    }

    // 分裂処理
    private void Split()
    {
        for (int i = 0; i < splitCount; i++)
        {
            // 小さなスライムを生成
            GameObject smallSlime = Instantiate(
                smallSlimePrefab,
                transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0),
                Quaternion.identity
            );

            // 子スライムの設定
            Rigidbody2D smallRb = smallSlime.GetComponent<Rigidbody2D>();
            if (smallRb != null)
            {
                // ランダムな方向に飛ばす
                float angle = Random.Range(-30, 30) + (i % 2 == 0 ? 90 : -90); // 左右に分かれるように
                angle *= Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                smallRb.AddForce(direction * splitForce, ForceMode2D.Impulse);
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
