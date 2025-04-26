using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBoss : MonoBehaviour, Enemy
{
    // �X�J���{�X�̏�Ԃ�\��enum
    public enum SkullState { Waiting, Moving, Vulnerable, Defeated }
    private SkullState currentState = SkullState.Waiting;

    [Header("�ړ��ݒ�")]
    [SerializeField] private Transform[] waypoints; // �ړ�����o�H�|�C���g
    [SerializeField] private float moveSpeed = 3f; // �ړ����x
    [SerializeField] private float waypointReachDistance = 0.1f; // �E�F�C�|�C���g�ɓ��B�����Ƃ݂Ȃ�����

    [Header("�e���ːݒ�")]
    [SerializeField] private GameObject bulletPrefab; // �e�̃v���n�u
    [SerializeField] private float fireRate = 1f; // ���ˊԊu�i�b�j
    [SerializeField] private float bulletSpeed = 5f; // �e�̑��x
    [SerializeField] private int burstCount = 3; // ��x�ɔ��˂���e�̐�
    [SerializeField] private float burstInterval = 0.2f; // �o�[�X�g���̔��ˊԊu

    [Header("�X�e�[�g�ݒ�")]
    [SerializeField] private float invinciblePhaseDuration = 20f; // ���G��Ԃ̌p������
    [SerializeField] private float vulnerablePhaseDuration = 10f; // �Ǝ��Ԃ̌p������
    [SerializeField] private int maxHealth = 20; // �ő�HP
    [SerializeField] private int damage = 1; // �v���C���[�ւ̐ڐG�_���[�W

    [Header("���E")]
    [SerializeField] private Transform leftBoundary;  // ���̋��E
    [SerializeField] private Transform rightBoundary; // �E�̋��E
    [SerializeField] private Transform topBoundary;   // ��̋��E
    [SerializeField] private Transform bottomBoundary;// ���̋��E

    // ����J�ϐ�
    private int currentHealth;
    private int currentWaypoint = 0;
    private bool isBattleStarted = false;
    private bool isInvincible = true;
    private float lastFireTime;
    private float originalRadius;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform playerTransform;
    private Vector3 originalScale;
    private bool isFiring = false;
    private string playerTag = "Player";
    private CircleCollider2D SkullCollider;
    

    private void Start()
    {
        // �R���|�[�l���g�擾
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        SkullCollider = GetComponent<CircleCollider2D>();

        // ������
        currentHealth = maxHealth;
        if (SkullCollider is CircleCollider2D circleCollider)
            originalRadius = circleCollider.radius;
        // �A�j���[�^�[�̏����ݒ�
        if (animator != null)
        {
            animator.SetBool("IsDown", false);
        }

        // �v���C���[����
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    private void Update()
    {
        // �v���C���[�����邩�m�F
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                return; // �v���C���[��������Ȃ�
            }
        }

        // �v���C���[�����E���ɂ��邩�m�F
        if (!isBattleStarted)
        {
            bool isPlayerInBoundary = IsPointInBoundary(playerTransform.position);

            if (isPlayerInBoundary)
            {
                StartBattle();
            }
        }

        // �X�e�[�g����
        switch (currentState)
        {
            case SkullState.Waiting:
                // �ҋ@���͉������Ȃ�
                break;

            case SkullState.Moving:
                MoveAlongPath();

                // ���Ԋu�ōU��
                if (!isFiring && Time.time > lastFireTime + fireRate)
                {
                    StartCoroutine(FireBurst());
                    lastFireTime = Time.time;
                }
                break;

            case SkullState.Vulnerable:
                // �Ǝ��Ԃł͓����Ȃ�
                rb.velocity = Vector2.zero;
                break;

            case SkullState.Defeated:
                // �|���ꂽ���
                rb.velocity = Vector2.zero;
                break;
        }
    }

    // �w�肵���_�����E���ɂ��邩�`�F�b�N
    private bool IsPointInBoundary(Vector3 point)
    {
        if (leftBoundary == null || rightBoundary == null ||
            topBoundary == null || bottomBoundary == null)
        {
            Debug.LogWarning("���E���ݒ肳��Ă��܂���");
            return false;
        }

        bool isInX = point.x >= leftBoundary.position.x && point.x <= rightBoundary.position.x;
        bool isInY = point.y >= bottomBoundary.position.y && point.y <= topBoundary.position.y;

        return isInX && isInY;
    }

    // �퓬�J�n
    private void StartBattle()
    {
        isBattleStarted = true;
        Debug.Log("�{�X�퓬�J�n�I");

        // ���G��Ԃ�ݒ�
        isInvincible = true;

        // �ړ��J�n
        currentState = SkullState.Moving;

        // ����I�ɒe�𔭎˂��邽�߂̏����ݒ�
        lastFireTime = Time.time;

        // �t�F�[�Y�^�C�}�[�J�n
        StartCoroutine(PhaseController());
    }

    // �o�H�ɉ����Ĉړ�
    private void MoveAlongPath()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // ���݂̃E�F�C�|�C���g���L�����m�F
        if (waypoints[currentWaypoint] == null)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            return;
        }

        // ���݂̖ڕW�|�C���g�ֈړ�
        Vector2 targetPosition = waypoints[currentWaypoint].position;
        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;

        // �ړ����x��ݒ�
        rb.velocity = moveDirection * moveSpeed;

        // �ڕW�|�C���g�ɋ߂Â����玟�̃|�C���g��
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        if (distanceToTarget < waypointReachDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;

            // �|�C���g���B���ɍU��
            if (!isFiring)
            {
                StartCoroutine(FireBurst());
            }
        }

        transform.localScale = originalScale; // ��Ɍ��̌������ێ�
    }

    // �o�[�X�g�ˌ�
    private IEnumerator FireBurst()
    {
        isFiring = true;
        for (int i = 0; i < burstCount; i++)
        {
            FireBullet();
            yield return new WaitForSeconds(burstInterval);
        }

        isFiring = false;
    }

    // �e�𔭎�
    private void FireBullet()
    {
        if (bulletPrefab == null || playerTransform == null) return;

        // �e�𐶐�
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            // �v���C���[�ւ̕������v�Z�i�����I�ɔ��ˁj
            Vector2 directionToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;

            // �e�ɑ��x��ݒ�
            bulletRb.velocity = directionToPlayer * bulletSpeed;

            // �e�̊p�x���v���C���[�Ɍ�����i�K�v�ɉ����āj
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    // �t�F�[�Y�R���g���[���[
    private IEnumerator PhaseController()
    {
        while (currentHealth > 0 && currentState != SkullState.Defeated)
        {
            // ���G�t�F�[�Y (Idle�A�j���[�V����)
            isInvincible = true;
            currentState = SkullState.Moving;

            // �����ݒ��ʏ��Ԃ�
            rb.gravityScale = 0f; // ���V��Ԃŏd�͂Ȃ�
                SkullCollider.radius = originalRadius; // �I���W�i�����a��ۑ����Ă���

            // �A�j���[�V�����ݒ�
            if (animator != null)
            {
                // IsDown �p�����[�^��false�ɐݒ�
                animator.SetBool("IsDown", false);
            }

            yield return new WaitForSeconds(invinciblePhaseDuration);

            // �Ǝ�t�F�[�Y (Down�A�j���[�V����)
            isInvincible = false;
            currentState = SkullState.Vulnerable;


            // �����ݒ�𗎉���ԂɕύX
            rb.velocity = Vector2.zero; // �܂����x�����Z�b�g
            rb.gravityScale = 20f; // �d�͂����߂ɐݒ肵�ċ}���ɗ���������
            SkullCollider.radius = originalRadius * 0.5f;

            // �A�j���[�V�����ݒ�
            if (animator != null)
            {
                // IsDown �p�����[�^��true�ɐݒ�
                animator.SetBool("IsDown", true);
            }

            yield return new WaitForSeconds(vulnerablePhaseDuration);
        }
    }

    // �v���C���[����̃_���[�W������
    public void ReceiveDamage(int damage)
    {
        // ���G��ԂȂ�_���[�W�𖳌���
        if (isInvincible || currentState == SkullState.Defeated) return;

        currentHealth -= damage;
        Debug.Log($"�{�X��{damage}�_���[�W���󂯂܂����I�c��HP: {currentHealth}");

        // HP��0�ɂȂ�����|���ꂽ��Ԃ�
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // �_���[�W�A�j���[�V�����Đ� (Hit�X�e�[�g)
            if (animator != null)
            {
                animator.SetTrigger("IsHit");

                StartCoroutine(ResetTrigger());
            }
        }
    }

    // �g���K�[���Z�b�g�p�R���[�`��
    private IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(1f);
        animator.ResetTrigger("IsHit");
    }



    // �|���ꂽ����
    private void Die()
    {
        currentState = SkullState.Defeated;
        Debug.Log("�{�X��|���܂����I");

        // ���S�A�j���[�V�����Đ�
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // �R���C�_�[������
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Rigidbody��~
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // ���b��ɔj��
        Destroy(gameObject, 3f);
    }


    // �v���C���[�ƏՓ˂�����
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            // �Փˏ����擾
            ContactPoint2D contact = collision.GetContact(0);


            //Player���Ń_���[�W���󂯂Ă��܂����߈�U�����蔻��𖳎�����B
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);

            // �v���C���[�𒵂˕Ԃ点��B
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 10f);
                }

                // �_���[�W���󂯂�
                ReceiveDamage(1);

           

            
        }
    }

    // �v���C���[�Ƀ_���[�W��^����
    public void PlayerDamage(Player player)
    {
        if (player != null)
        {
            player.Damage(damage);
        }
    }

    // Gizmo�`�� (�G�f�B�^�p)
    private void OnDrawGizmosSelected()
    {
        // ���E��\��
        if (leftBoundary != null && rightBoundary != null &&
            topBoundary != null && bottomBoundary != null)
        {
            Gizmos.color = Color.cyan;

            Vector3 tL = new Vector3(leftBoundary.position.x, topBoundary.position.y);
            Vector3 tR = new Vector3(rightBoundary.position.x, topBoundary.position.y);
            Vector3 bL = new Vector3(leftBoundary.position.x, bottomBoundary.position.y);
            Vector3 bR = new Vector3(rightBoundary.position.x, bottomBoundary.position.y);

            Gizmos.DrawLine(tL, tR); // ���
            Gizmos.DrawLine(bL, bR); // ����
            Gizmos.DrawLine(tL, bL); // ����
            Gizmos.DrawLine(tR, bR); // �E��
        }

        // �o�H��\��
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Vector3 currentPos = waypoints[i].position;
                    Vector3 nextPos = waypoints[(i + 1) % waypoints.Length].position;
                    Gizmos.DrawLine(currentPos, nextPos);
                    Gizmos.DrawSphere(currentPos, 0.2f);
                }
            }
        }
    }
}
