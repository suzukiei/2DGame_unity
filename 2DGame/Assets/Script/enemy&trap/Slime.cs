using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : AttackEnemy
{
    [Header("�X���C���ݒ�")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpInterval = 2f;

    [Header("�����ݒ�")]
    [SerializeField] private GameObject smallSlimePrefab;
    [SerializeField] private int splitCount = 2; // ������
    [SerializeField] private float splitForce = 3f;
    [SerializeField] private bool canSplit = true; // ����\���ǂ���

    // ��ԊǗ�
    private float lastJumpTime = 0f;
    private bool isProcessingSplit = false; // ���􏈗������ǂ���

    protected override void Start()
    {
        base.Start();
        lastJumpTime = Time.time - Random.Range(0f, jumpInterval * 0.5f);
    }

    protected override void Update()
    {
        base.Update();

        // �X���C�����L�̃W�����v����
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

    // �v���C���[���Փ˂����Ƃ��̏���
    void OnCollisionEnter2D(Collision2D collision)
    {
        // ���􏈗����Ȃ疳��
        if (isProcessingSplit) return;

        // �v���C���[�Ƃ̏Փ˂����o
        if (collision.gameObject.CompareTag("Player"))
        {
            // �Փˏ����擾
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 relativeVelocity = collision.relativeVelocity;

            Debug.Log($"�X���C���ւ̏Փ�: normal.y={contact.normal.y}, velocity.y={relativeVelocity.y}");

            // ���݂�����F�v���C���[���ォ��~���Ă��Ă���
            if (contact.normal.y < -0.3f)
            {
                Debug.Log("���݂����o: ���􏈗����J�n���܂�");

                // �v���C���[���������ˏグ��iHitEnemy����Ɏ��s�j
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 5f);
                }

                // ���􏈗�
                if (canSplit && smallSlimePrefab != null)
                {
                    isProcessingSplit = true;
                    StartCoroutine(SplitAfterFrame());
                }
            }
        }
    }

    // 1�t���[���҂��Ă��番�􂷂�i�Փˏ����̏�����������j
    IEnumerator SplitAfterFrame()
    {
        // 1�t���[���ҋ@�iPlayer.HitEnemy�����s����鎞�Ԃ��m�ہj
        yield return new WaitForEndOfFrame();
        Split();
    }

    // ���N���X��ReceiveDamage()���㏑��
    public override void ReceiveDamage(int _hp)
    {
        // ���􏈗����Ȃ�ʏ�̃_���[�W�������X�L�b�v
        if (isProcessingSplit)
        {
            Debug.Log("���􏈗����̂��߁A�ʏ�_���[�W�������X�L�b�v���܂�");
            return;
        }

        // �ʏ�̃_���[�W����
        Debug.Log("�ʏ�_���[�W���������s���܂�");
        base.ReceiveDamage(_hp);
    }

    // ���􏈗�
    private void Split()
    {
        Debug.Log("�X���C����������s���܂�");

        for (int i = 0; i < splitCount; i++)
        {
            GameObject smallSlime = Instantiate(
                smallSlimePrefab,
                transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0),
                Quaternion.identity
            );

            Rigidbody2D smallRb = smallSlime.GetComponent<Rigidbody2D>();
            if (smallRb != null)
            {
                float angle = Random.Range(-30, 30) + (i % 2 == 0 ? 90 : -90);
                angle *= Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                smallRb.AddForce(direction * splitForce, ForceMode2D.Impulse);
            }

            SlimeEnemy smallSlimeController = smallSlime.GetComponent<SlimeEnemy>();
            if (smallSlimeController != null)
            {
                smallSlimeController.canSplit = false;
            }
        }

        // �e�X���C����j��
        EnemyManager.Instance.DestroyEnemyObjList(this.gameObject);
        Destroy(gameObject);
    }
}