using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bee : MonoBehaviour
{
    [SerializeField,Header("�e�̃v���t�@�u")] private GameObject bulletPrefab;
    [SerializeField,Header("�U���͈�")] private float attackRange = 5f;
    [SerializeField,Header("���̍U���܂ł̊Ԋu")] private float attackInterval = 2f;
    [SerializeField,Header("�ړ����x")] private float moveSpeed = 3f;      // �ړ����x
    [SerializeField,Header("�v���C���[��ǐՂ���͈�")] private float detectionRange = 8f; // �ǐՂ��J�n���鋗��
    [SerializeField,Header("�؋󋗗�")] private float hoverHeight = 3f;    // �v���C���[�̏㉽���[�g���̈ʒu���ێ����邩

    // �I�̓�������胊�A���ɂ��邽�߂̃p�����[�^
    [SerializeField,Header("�I�̗h�ꕝ")] private float wobbleAmplitude = 0.5f;    // �h��̐U��
    [SerializeField,Header("�I���h���p�x")] private float wobbleFrequency = 10f;     // �h��̕p�x
    [SerializeField,Header("�����]���̊Ԋu")] private float changeDirectionInterval = 0.7f; // �����]���̊Ԋu
    [SerializeField] private float maxRandomOffset = 1.5f;    // �����_���ȓ����̍ő�I�t�Z�b�g

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

        // �ǐՏ���
        if (distance <= detectionRange)
        {
            MoveTowardsPlayer();
        }

        // �U������
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
        // ���Ԋu�Ń����_���ȕ����ɕω�������
        directionChangeTimer += Time.deltaTime;
        if (directionChangeTimer >= changeDirectionInterval)
        {
            randomOffset = GetRandomOffset();
            directionChangeTimer = 0f;
        }
    }

    Vector2 GetRandomOffset()
    {
        // �����_���ȃI�t�Z�b�g�𐶐�
        return new Vector2(
            Random.Range(-maxRandomOffset, maxRandomOffset),
            Random.Range(-maxRandomOffset, maxRandomOffset)
        );
    }

    Vector2 CalculateWobbleEffect()
    {
        // �T�C���g���g�����h�����
        float xWobble = Mathf.Sin(wobbleTime * wobbleFrequency) * wobbleAmplitude;
        float yWobble = Mathf.Cos(wobbleTime * wobbleFrequency * 0.7f) * wobbleAmplitude;

        return new Vector2(xWobble, yWobble);
    }

    void MoveTowardsPlayer()
    {
        // �v���C���[�̏��̖ڕW�ʒu���v�Z
        Vector2 baseTargetPosition = new Vector2(player.position.x, player.position.y + hoverHeight);

        // �����_���ȃI�t�Z�b�g�Ɨh����ʂ�ǉ�
        Vector2 wobbleEffect = CalculateWobbleEffect();
        Vector2 targetPosition = baseTargetPosition + randomOffset + wobbleEffect;

        // ���݈ʒu����ڕW�ʒu�ւ̕������v�Z
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // �ړ���K�p�i���������x���g���Ƃ�莩�R�Ɂj
        Vector2 desiredVelocity = direction * moveSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, Time.deltaTime * 3f);

        // �K�v�ɉ����č��E�̌�����ύX
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(
                direction.x < 0 ? -originalScale.x : originalScale.x,
                originalScale.y,
                originalScale.z
            );
        }

        // �I�v�V����: �����ɍ��킹�ď����X����
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
            // �v���C���[�Ɍ������Ēe�𔭎˂���i�I�v�V�����j
            Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            bulletRb.velocity = (Vector2.down + directionToPlayer * 0.5f) * 5f;
        }
    }

    // �ǐՔ͈͂ƍU���͈͂�����
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}