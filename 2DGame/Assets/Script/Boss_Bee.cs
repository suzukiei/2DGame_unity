using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bee : MonoBehaviour
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackInterval = 2f;

    [SerializeField] private float moveSpeed = 3f;      // �ړ����x
    [SerializeField] private float detectionRange = 8f; // �ǐՂ��J�n���鋗��
    [SerializeField] private float hoverHeight = 3f;    // �v���C���[�̏㉽���[�g���̈ʒu���ێ����邩

    private float attackTimer = 0f;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 originalScale;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

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

    void MoveTowardsPlayer()
    {
        // �v���C���[�̏��̖ڕW�ʒu���v�Z
        Vector2 targetPosition = new Vector2(player.position.x, player.position.y + hoverHeight);

        // ���݈ʒu����ڕW�ʒu�ւ̕������v�Z
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // �ړ���K�p
        rb.velocity = direction * moveSpeed;

        // �K�v�ɉ����č��E�̌�����ύX
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(
                direction.x < 0 ? -originalScale.x : originalScale.x,
                originalScale.y,
                originalScale.z
            );
        }
    }

    void Attack()
    {
        //// �����̍U������
        //if (animator != null)
        //{
        //    animator.SetTrigger("Attack");
        //}

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.velocity = Vector2.down * 5f;
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
