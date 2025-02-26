using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss_Bee : MonoBehaviour,Enemy
{
    [SerializeField,Header("�e�̃v���t�@�u")] private GameObject bulletPrefab;
    [SerializeField,Header("�U���͈�")] private float attackRange = 5f;
    [SerializeField,Header("�Փˎ��U����")] private int damage = 1;
    [SerializeField,Header("���̍U���܂ł̊Ԋu")] private float attackInterval = 2f;
    [SerializeField,Header("�ړ����x")] private float moveSpeed = 3f;      // �ړ����x
    [SerializeField,Header("�v���C���[��ǐՂ���͈�")] private float detectionRange = 8f; // �ǐՂ��J�n���鋗��
    [SerializeField,Header("�؋󋗗�")] private float hoverHeight = 3f;    // �v���C���[�̏㉽���[�g���̈ʒu���ێ����邩
    [SerializeField,Header("�u���b�N�Փˎ�������")] private float bounceForce = 5f;    // ������

    // �I�̓�������胊�A���ɂ��邽�߂̃p�����[�^
    [SerializeField,Header("�I�̗h�ꕝ")] private float wobbleAmplitude = 0.5f;    // �h��̐U��
    [SerializeField,Header("�I���h���p�x")] private float wobbleFrequency = 10f;     // �h��̕p�x
    [SerializeField,Header("�����]���̊Ԋu")] private float changeDirectionInterval = 0.7f; // �����]���̊Ԋu
    [SerializeField] private float maxRandomOffset = 1.5f;    // �����_���ȓ����̍ő�I�t�Z�b�g
    [SerializeField,Header("���E")] private Transform leftBoundary;  // ���̋��E
    [SerializeField] private Transform rightBoundary; // �E�̋��E
    [SerializeField] private Transform topBoundary;   // ��̋��E
    [SerializeField] private Transform bottomBoundary;// ���̋��E

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

        // 2�̃R���C�_�[�̐ݒ���m�F
        CapsuleCollider2D[] colliders = GetComponents<CapsuleCollider2D>();
        if (colliders.Length == 2)
        {
            // 1�ڂ̃R���C�_�[�F�v���C���[�p�i�����Փˁj
            colliders[0].isTrigger = false;

            // 2�ڂ̃R���C�_�[�F���p�i�g���K�[�j
            colliders[1].isTrigger = true;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        //Player����I�܂ł̋������v�Z
        float distance = Vector2.Distance(transform.position, player.position);

        //�ړ��͈͓������������߂����Player��ǔ�
        //�����łȂ���Α����ɒ�~
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

        //���E���߂Ă���ꍇd
        if (leftBoundary != null && rightBoundary != null && topBoundary != null && bottomBoundary != null)
        {
            //���E���Ɉʒu�𐧌�����
            targetPosition.x = Mathf.Clamp(targetPosition.x, leftBoundary.position.x, rightBoundary.position.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, bottomBoundary.position.y, topBoundary.position.y);
        }

        // ���݈ʒu����ڕW�ʒu�ւ̕������v�Z
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        //// �ړ���K�p�i���������x���g���Ƃ�莩�R�Ɂj
        //Vector2 desiredVelocity = direction * moveSpeed;
        //rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, Time.deltaTime * 3f);      


        // �ړ��O�̕ǃ`�F�b�N
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
            // �ǂ����o���ꂽ�ꍇ�̏���
            // �ǂ̍������m�F���āA����������Ɉړ�
            float wallHeight = wallHit.collider.bounds.max.y;
            if (transform.position.y < wallHeight)
            {
                // ���݂̉������̑��x���ێ����Ȃ���A�������Ə㏸
                desiredVelocity = new Vector2(
                    rb.velocity.x * 0.5f, // �������̑��x����������
                    moveSpeed * 0.8f      // �K�x�ȏ㏸���x
                );
            }
            else
            {
                // �ǂ̏�ɓ��B������ʏ�̒ǐՂɖ߂�
                desiredVelocity = direction * moveSpeed;
            }
        }
        else
        {
            // �ʏ�̒ǐ�
            desiredVelocity = direction * moveSpeed;
        }

        // ���x�̓K�p�i�}���ȕω���h���j
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

    //Player�Փˎ�
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
            // �v���C���[�Ɍ������Ēe�𔭎˂���i�I�v�V�����j
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

            // ������̏Փ˂̏ꍇ�i���̏�ʂƂ̏Փˁj
            if (normal.y > 0.5f)
            {
                // ��苭��������̔����͂�������
                rb.velocity = new Vector2(rb.velocity.x, moveSpeed * bounceForce);
            }
            // ������̏Փ˂̏ꍇ
            else if (Mathf.Abs(normal.x) > 0.5f)
            {
                // �������ւ̔���
                rb.velocity = new Vector2(normal.x * moveSpeed, rb.velocity.y);
            }
        }
    }


    // �ǐՔ͈͂ƍU���͈͂�����
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (leftBoundary != null && rightBoundary != null && topBoundary != null && bottomBoundary != null)
        {
            Gizmos.color = Color.cyan;
            // ���E�̎l���̍��W
            Vector3 topLeft = new Vector3(leftBoundary.position.x, topBoundary.position.y);
            Vector3 topRight = new Vector3(rightBoundary.position.x, topBoundary.position.y);
            Vector3 bottomLeft = new Vector3(leftBoundary.position.x, bottomBoundary.position.y);
            Vector3 bottomRight = new Vector3(rightBoundary.position.x, bottomBoundary.position.y);

            // �㉺���E�̋��E����`��
            Gizmos.DrawLine(topLeft, topRight);     // ���
            Gizmos.DrawLine(bottomLeft, bottomRight); // ����
            Gizmos.DrawLine(topLeft, bottomLeft);     // ����
            Gizmos.DrawLine(topRight, bottomRight);   // �E��
        }
    }
}