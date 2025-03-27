using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryPig : MonoBehaviour
{
    [Header("���o�ݒ�")]
    [SerializeField,Header("���o����")] private float detectionRange = 5f; // �v���C���[���o����
    [SerializeField,Header("�{���Ԃ̑��x")] private float AngrySpeed = 6f; // �{���Ԃ̑��x

    [Header("�W�����v�ݒ�")]
    [SerializeField] private float jumpForce = 5f; // �W�����v��
    [SerializeField] private float jumpDelay = 0.2f; // �W�����v�O�̑ҋ@����

    // �Q��
    private Transform playerTransform;
    private AttackEnemy attackEnemyScript;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isAngry = false;
    private bool isJumping = false;
    private bool Chase = false;

    void Start()
    {
        // �K�v�ȃR���|�[�l���g���擾
        attackEnemyScript = GetComponent<AttackEnemy>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // �v���C���[������
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        animator.SetBool("IsWalking", true);
    }

    void Update()
    {
        if (playerTransform == null) return;

        // �v���C���[�Ƃ̋������v�Z
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // �v���C���[���͈͓��ɓ��������`�F�b�N
        if (distanceToPlayer <= detectionRange && !isAngry)
        {
            // �{�胂�[�h�ɐؑ�
            StartCoroutine(SwitchToAngryMode());
        }
        else if (distanceToPlayer > detectionRange && isAngry)
        {
            // �ʏ탂�[�h�ɖ߂�
            SwitchToNormalMode();
        }
    }

    IEnumerator SwitchToAngryMode()
    {
        isAngry = true;
        isJumping = true;
        Chase = false;
        // �A�j���[�V�����ύX�i�ʏ��Ԃ���{���Ԃցj
        animator.SetBool("IsRunning", true);
        animator.SetBool("IsWalking", false);

        yield return new WaitForSeconds(jumpDelay);

        // ���݂̑��x�����Z�b�g
        rb.velocity = new Vector2(0, rb.velocity.y);

        // �W�����v
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // �W�����v���I���܂őҋ@�i�ȈՓI��0.5�b�ҋ@�j
        yield return new WaitForSeconds(0.5f);

        // �v���C���[�̕���������
        if (playerTransform.position.x < transform.position.x)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else
        {
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }

        // �ǐՊJ�n
        isJumping = false;
        Chase = true;
    }

    void SwitchToNormalMode()
    {
        isAngry = false;
        Chase = false;

        // �A�j���[�V�����ύX�i�{���Ԃ���ʏ��Ԃցj
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsWalking", true);
    }

    // �ړ�������s��LateUpdate�iAttackEnemy��Update��Ɏ��s�����j
    void LateUpdate()
    {
        if (!isAngry || isJumping || !Chase) return; // �{�胂�[�h�łȂ��ꍇ�͏������Ȃ�

        // AttackEnemy���珰��������擾
        // �����ł͒���bfloor�ɃA�N�Z�X�ł��Ȃ����߁A�A�j���[�V������IsIdle�p�����[�^���画�f
        bool isOnFloor = !animator.GetBool("IsIdle");

        if (!isOnFloor) return; // ���̏�ɂ��Ȃ��ꍇ�͏������Ȃ�

        // �v���C���[�����ւ̈ړ��x�N�g�����v�Z
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // X�����̈ړ��̂ݓK�p�iY�����͏d�͂ɔC����j
        rb.velocity = new Vector2(directionToPlayer.x * AngrySpeed, rb.velocity.y);

        // �����̍X�V
        if (directionToPlayer.x < 0.0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (directionToPlayer.x > 0.0f)
        {
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }
    }

    // �f�o�b�O�p�Ɍ��o�͈͂�\��
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
