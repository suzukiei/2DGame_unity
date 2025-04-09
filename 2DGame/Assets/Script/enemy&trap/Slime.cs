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
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isStomped = false;

    // ���N���X��Start()���㏑��
    new void Start()
    {
        // ���N���X��Start()���Ăяo��
        base.Start();

        // �K�v�ȃR���|�[�l���g�擾
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // �ŏ��̃W�����v�܂ł̎��Ԃ������_����
        lastJumpTime = Time.time - Random.Range(0f, jumpInterval * 0.5f);
    }

    // ���N���X��Update()���㏑��
    new void Update()
    {
        // ���N���X��Update()���Ăяo��
        base.Update();

        // �X���C�����L�̃W�����v����
        if (base.bfloor && Time.time > lastJumpTime + jumpInterval)
        {
            Jump();
        }
    }

    private void Jump()
    {
        // �W�����v���s
        rb.velocity = new Vector2(rb.velocity.x, 0); // Y���x�����Z�b�g
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        lastJumpTime = Time.time;

        // Run �A�j���[�V�����͊����̂��̂��g�p����̂Ńg���K�[�ݒ�͕s�v
        // Anim.SetTrigger("Jump"); <- ���̍s�͍폜

        // �X���C�����L�̐L�k�A�j���[�V����
        StartCoroutine(SlimeStretchEffect());
    }

    // �X���C���̐L�k�G�t�F�N�g
    IEnumerator SlimeStretchEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 stretchScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z);

        // �W�����v���̐L��
        transform.localScale = stretchScale;
        yield return new WaitForSeconds(0.1f);

        // ���ɖ߂�
        transform.localScale = originalScale;
    }

    // ���n���̏������g��
    new private void HitFloor()
    {
        base.HitFloor();

        // ���ɒ��n�����u��
        if (base.bfloor && rb.velocity.y <= 0.1f)
        {
            StartCoroutine(SquashEffect());
        }
    }

    // ���n���ׂ̒�G�t�F�N�g
    IEnumerator SquashEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 squashScale = new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z);

        transform.localScale = squashScale;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    // ���N���X��ReceiveDamage()���㏑��
    new public void ReceiveDamage(int _hp)
    {
        // ���݂���ꂽ�ꍇ�͕��􏈗�
        if (isStomped && canSplit && smallSlimePrefab != null)
        {
            Split();
        }
        else
        {
            // �ʏ�̃_���[�W����
            base.ReceiveDamage(_hp);
        }
    }

    // ���􏈗�
    private void Split()
    {
        for (int i = 0; i < splitCount; i++)
        {
            // �����ȃX���C���𐶐�
            GameObject smallSlime = Instantiate(
                smallSlimePrefab,
                transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0),
                Quaternion.identity
            );

            // �q�X���C���̐ݒ�
            Rigidbody2D smallRb = smallSlime.GetComponent<Rigidbody2D>();
            if (smallRb != null)
            {
                // �����_���ȕ����ɔ�΂�
                float angle = Random.Range(-30, 30) + (i % 2 == 0 ? 90 : -90); // ���E�ɕ������悤��
                angle *= Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                smallRb.AddForce(direction * splitForce, ForceMode2D.Impulse);
            }

            // �q�X���C���̕����ݒ�𖳌���
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
