using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : AttackEnemy
{
    [Header("ƒXƒ‰ƒCƒ€İ’è")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpInterval = 2f;

    [Header("•ª—ôİ’è")]
    [SerializeField] private GameObject smallSlimePrefab;
    [SerializeField] private int splitCount = 2; // •ª—ô”
    [SerializeField] private float splitForce = 8f;
    [SerializeField] private bool canSplit = true; // •ª—ô‰Â”\‚©‚Ç‚¤‚©


    // ó‘ÔŠÇ—
    private float lastJumpTime = 0f;
    private bool isProcessingSplit = false; // •ª—ôˆ—’†‚©‚Ç‚¤‚©

    protected override void Start()
    {
        base.Start();
        lastJumpTime = Time.time - Random.Range(0f, jumpInterval * 0.5f);
    }

    protected override void Update()
    {
        base.Update();

        // ƒXƒ‰ƒCƒ€“Á—L‚ÌƒWƒƒƒ“ƒvˆ—
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

<<<<<<< Updated upstream
    // ƒvƒŒƒCƒ„[‚ªÕ“Ë‚µ‚½‚Æ‚«‚Ìˆ—
    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    // •ª—ôˆ—’†‚È‚ç–³‹
    //    if (isProcessingSplit) return;

    //    // ƒvƒŒƒCƒ„[‚Æ‚ÌÕ“Ë‚ğŒŸo
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        // Õ“Ëî•ñ‚ğæ“¾
    //        ContactPoint2D contact = collision.GetContact(0);
    //        Vector2 relativeVelocity = collision.relativeVelocity;

    //        Debug.Log($"ƒXƒ‰ƒCƒ€‚Ö‚ÌÕ“Ë: normal.y={contact.normal.y}, velocity.y={relativeVelocity.y}");

    //        // “¥‚İ‚Â‚¯”»’èFƒvƒŒƒCƒ„[‚ªã‚©‚ç~‚Á‚Ä‚«‚Ä‚¢‚é
    //        if (contact.normal.y < -0.3f)
    //        {
    //            Debug.Log("“¥‚İ‚Â‚¯ŒŸo: •ª—ôˆ—‚ğŠJn‚µ‚Ü‚·");

    //            // ƒvƒŒƒCƒ„[‚ğ­‚µ’µ‚Ëã‚°‚éiHitEnemy‚æ‚èæ‚ÉÀsj
    //            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
    //            if (playerRb != null)
    //            {
    //                playerRb.velocity = new Vector2(playerRb.velocity.x, 5f);
    //            }

    //            // •ª—ôˆ—
    //            if (canSplit && smallSlimePrefab != null)
    //            {
    //                isProcessingSplit = true;
    //                StartCoroutine(SplitAfterFrame());
    //            }
    //        }
    //    }
    //}

    // 1ƒtƒŒ[ƒ€‘Ò‚Á‚Ä‚©‚ç•ª—ô‚·‚éiÕ“Ëˆ—‚Ì‡˜–â‘è‚ğ‰ñ”ğj
    IEnumerator SplitAfterFrame()
=======
<<<<<<< HEAD
    // Šî’êƒNƒ‰ƒX‚ÌReceiveDamage()‚ğã‘‚«
    new public void ReceiveDamage(int _hp)
>>>>>>> Stashed changes
    {
        // 1ƒtƒŒ[ƒ€‘Ò‹@iPlayer.HitEnemy‚ªÀs‚³‚ê‚éŠÔ‚ğŠm•Ûj
        yield return new WaitForEndOfFrame();
        Split();
    }

    // Šî’êƒNƒ‰ƒX‚ÌReceiveDamage()‚ğã‘‚«
    public override void ReceiveDamage(int _hp,GameObject player=null)
    {
        // •ª—ôˆ—
        if (canSplit && smallSlimePrefab != null)
        {
            isProcessingSplit = true;
        }
        // •ª—ôˆ—’†‚È‚ç’Êí‚Ìƒ_ƒ[ƒWˆ—‚ğƒXƒLƒbƒv
        if (isProcessingSplit)
        {
<<<<<<< Updated upstream
            Debug.Log("•ª—ôˆ—’†‚Ì‚½‚ßA’Êíƒ_ƒ[ƒWˆ—‚ğƒXƒLƒbƒv‚µ‚Ü‚·");
            // ƒvƒŒƒCƒ„[‚ğ­‚µ’µ‚Ëã‚°‚éiHitEnemy‚æ‚èæ‚ÉÀsj
            Rigidbody2D playerRb = player.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, 5f);
            }
            // •ª—ôˆ—
            if (canSplit && smallSlimePrefab != null)
            {
                isProcessingSplit = true;
                StartCoroutine(SplitAfterFrame());
            }
            // •ª—ôˆ—
            return;
=======
            // ’Êí‚Ìƒ_ƒ[ƒWˆ—
            base.ReceiveDamage(_hp);
=======
    // ƒvƒŒƒCƒ„[‚ªÕ“Ë‚µ‚½‚Æ‚«‚Ìˆ—
    void OnCollisionEnter2D(Collision2D collision)
    {
        // •ª—ôˆ—’†‚È‚ç–³‹
        if (isProcessingSplit) return;

        // ƒvƒŒƒCƒ„[‚Æ‚ÌÕ“Ë‚ğŒŸo
        if (collision.gameObject.CompareTag("Player"))
        {
            // Õ“Ëî•ñ‚ğæ“¾
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 relativeVelocity = collision.relativeVelocity;

            Debug.Log($"ƒXƒ‰ƒCƒ€‚Ö‚ÌÕ“Ë: normal.y={contact.normal.y}, velocity.y={relativeVelocity.y}");

            // “¥‚İ‚Â‚¯”»’èFƒvƒŒƒCƒ„[‚ªã‚©‚ç~‚Á‚Ä‚«‚Ä‚¢‚é
            if (contact.normal.y < -0.3f)
            {
                Debug.Log("“¥‚İ‚Â‚¯ŒŸo: •ª—ôˆ—‚ğŠJn‚µ‚Ü‚·");

                // ƒvƒŒƒCƒ„[‚ğ­‚µ’µ‚Ëã‚°‚éiHitEnemy‚æ‚èæ‚ÉÀsj
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 5f);
                }

                // •ª—ôˆ—
                if (canSplit && smallSlimePrefab != null)
                {
                    isProcessingSplit = true;
                    StartCoroutine(SplitAfterFrame());
                }
            }
        }
    }

    // 1ƒtƒŒ[ƒ€‘Ò‚Á‚Ä‚©‚ç•ª—ô‚·‚éiÕ“Ëˆ—‚Ì‡˜–â‘è‚ğ‰ñ”ğj
    IEnumerator SplitAfterFrame()
    {
        // 1ƒtƒŒ[ƒ€‘Ò‹@iPlayer.HitEnemy‚ªÀs‚³‚ê‚éŠÔ‚ğŠm•Ûj
        yield return new WaitForEndOfFrame();
        Split();
    }

    // Šî’êƒNƒ‰ƒX‚ÌReceiveDamage()‚ğã‘‚«
    public override void ReceiveDamage(int _hp)
    {
        // •ª—ôˆ—’†‚È‚ç’Êí‚Ìƒ_ƒ[ƒWˆ—‚ğƒXƒLƒbƒv
        if (isProcessingSplit)
        {
            Debug.Log("•ª—ôˆ—’†‚Ì‚½‚ßA’Êíƒ_ƒ[ƒWˆ—‚ğƒXƒLƒbƒv‚µ‚Ü‚·");
            return;
>>>>>>> parent of ee7931c (4æœˆ25æ—¥ãƒãƒ¼ã‚¸)
>>>>>>> Stashed changes
        }

        // ’Êí‚Ìƒ_ƒ[ƒWˆ—
        Debug.Log("’Êíƒ_ƒ[ƒWˆ—‚ğÀs‚µ‚Ü‚·");
        base.ReceiveDamage(_hp);
    }

    // •ª—ôˆ—
    private void Split()
    {
        Debug.Log("ƒXƒ‰ƒCƒ€‚ª•ª—ô‚µ‚Ü‚·I");

        for (int i = 0; i < splitCount; i++)
        {
            // ¶‰E‚Ç‚¿‚ç‚©‚ğŒˆ’èi‹ô””Ô–Ú‚Í¶AŠï””Ô–Ú‚Í‰Ej
            float xDirection = (i % 2 == 0) ? -1f : 1f;

            // ¶¬ˆÊ’u‚ğ‚í‚¸‚©‚É‚¸‚ç‚·
            Vector3 spawnPosition = transform.position + new Vector3(xDirection * 1f, 0.1f, 0);

            // ¬‚³‚ÈƒXƒ‰ƒCƒ€‚ğ¶¬
            GameObject smallSlime = Instantiate(
                smallSlimePrefab,
                spawnPosition,
                Quaternion.identity
            );

            // qƒXƒ‰ƒCƒ€‚Ìİ’è
            Rigidbody2D smallRb = smallSlime.GetComponent<Rigidbody2D>();
            if (smallRb != null)
            {
                // ¶‰E‚É”ò‚ÑU‚é‚æ‚¤‚ÉƒVƒ“ƒvƒ‹‚È•ûŒü‚ğİ’è
                Vector2 direction = new Vector2(xDirection * 3f, 3f);

                // •ûŒü‚ğ³‹K‰»‚µ‚Ä—Í‚ğ‰Á‚¦‚é
                smallRb.AddForce(direction.normalized * splitForce, ForceMode2D.Impulse);
            }

            // qƒXƒ‰ƒCƒ€‚Ì•ª—ôİ’è‚ğ–³Œø‰»
            SlimeEnemy smallSlimeController = smallSlime.GetComponent<SlimeEnemy>();
            if (smallSlimeController != null)
            {
                smallSlimeController.canSplit = false;
            }
        }


        // eƒXƒ‰ƒCƒ€‚ğ”j‰ó
        EnemyManager.Instance.DestroyEnemyObjList(this.gameObject);
        Destroy(gameObject);
    }
}