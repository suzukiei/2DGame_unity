using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss_Bee : MonoBehaviour,Enemy
{


    // �I�̏�Ԃ�\��enum
    public enum BeeState { Idle, Chase, Attack, Return }
    private BeeState currentState = BeeState.Idle;

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
    [Header("���^�[���ݒ�")]
    [SerializeField] private float returnSpeed = 2f; // �߂鑬�x
    [SerializeField] private float returnDelay = 3f; // ���E����O��Ă���߂�n�߂�܂ł̒x��

    private float attackTimer = 0f;
    private float directionChangeTimer = 0f;
    private Vector2 randomOffset;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private float wobbleTime = 0f;
    private bool isFloor = false;


    private Vector3 initialPosition; // �����ʒu��ۑ�
    private float outOfRangeTimer = 0f; // ���E�O�ɂ��鎞��
    private bool isReturning = false; // �����ʒu�ɖ߂��Ă��邩

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        randomOffset = Random.insideUnitCircle * maxRandomOffset;
        initialPosition = transform.position;

        // �R���C�_�[�ݒ�
        CapsuleCollider2D[] colliders = GetComponents<CapsuleCollider2D>();
        if (colliders.Length == 2)
        {
            colliders[0].isTrigger = false; // �v���C���[�p
            colliders[1].isTrigger = true;  // ���p
        }
    }

    void Update()
    {
        if (player == null) return;

        wobbleTime += Time.deltaTime;
        UpdateRandomOffset();

        // ���݂̏�ԂɊ�Â��čs��������
        float distance = Vector2.Distance(transform.position, player.position);

        // ��ԍX�V
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
                        Debug.Log("�A�ҊJ�n");
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
                        Debug.Log("�����ʒu�ɖ߂�܂���");
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
        // �v���C���[�̏��ڕW�ʒu���v�Z
        Vector2 targetPos = new Vector2(player.position.x, player.position.y + hoverHeight)
                           + randomOffset + CalculateWobbleEffect();

        // ���E����
        if (leftBoundary && rightBoundary && topBoundary && bottomBoundary)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, leftBoundary.position.x, rightBoundary.position.x);
            targetPos.y = Mathf.Clamp(targetPos.y, bottomBoundary.position.y, topBoundary.position.y);
        }

        // �ڕW�ʒu�ւ̕������v�Z
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        Vector2 desiredVelocity = direction * moveSpeed;

        // �ǃ`�F�b�N
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

        // ���x�E�����E�X���X�V
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

        // �I�炵���������ێ�
        rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity + CalculateWobbleEffect() * 0.5f, Time.deltaTime * 3f);

        // �����ƌX���̍X�V
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
        // �ǐՔ͈͂ƍU���͈͂�����
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // ���E�̉���
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