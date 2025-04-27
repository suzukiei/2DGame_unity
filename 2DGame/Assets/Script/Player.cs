using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    [SerializeField, Header("�ړ����x")] private float moveSpeed;
    [SerializeField, Header("�W�����v���x")] private float jumpSpeed;
    [SerializeField, Header("�G�l�~�[�𓥂񂾎��̃W�����v�N������")] private float enemyJumpTime;
    [SerializeField, Header("HP")] private int hp;
    [SerializeField, Header("���G����")] private float invincible;
    [SerializeField, Header("�_�Ŏ���")] private float flash;
    [SerializeField, Header("���V���̉��ړ����x")] private float airControlSpeed = 5f;
    [SerializeField, Header("heart�I�u�W�F�N�g")] private GameObject heartObj;
    [SerializeField, Header("Screen")] private GameObject ScreenObj;

    private Vector2 inputDirection;
    private Rigidbody2D rigid;
    [SerializeField]
    private bool bjump;
    [SerializeField]
    private bool enemyJumpFlag;

    private Animator anim;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private bool XboxDevice;

    ///Start�AUpdate
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        bjump = false;
        enemyJumpFlag = false;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        XboxDevice = false;
        XboxDeviceCheck();
    }
    // Update is called once per frame
    void Update()
    {
        OnMove();
        MOVE();
        LookMoveDirec();
        EnemyJump();
    }
    void FixedUpdate()
    {
        hitFloor();
        OnJump();
    }
    ///�L�[���͂Ȃ�
    //Xbox�R���g���[���ƃL�[�{�[�h����̐؂�ւ�
    private void XboxDeviceCheck()
    {
        InputSystem.onDeviceChange += (device, change) =>
         {
             if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
             {
                 Debug.Log($"Device '{device}' was {change}");
                 if (change.ToString() == "Added")
                     XboxDevice = true;
                 else
                     XboxDevice = false;
             }
         };
        var controllers = Input.GetJoystickNames();
        if (controllers.Length <= 0) return;
        if (controllers[0] == "") return;
        XboxDevice = true;
    }
    //�G�l�~�[�w�b�h�̃W�����v����
    private void EnemyJump()
    {
        if (!enemyJumpFlag) return;
        if (XboxDevice)
        {
            if (!Input.GetKeyDown("joystick button 0")) return; //Xbox������Ă��Ȃ����
            if (!bjump) return;

            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpSpeed * 1.2f, ForceMode2D.Impulse); //ForceMode2D�̐ݒ��Force��Impulse
            enemyJumpFlag = false;
            Debug.Log("Jump");

        }
        else
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return; //PC������Ă��Ȃ����
            if (!bjump) return;
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpSpeed * 1.2f, ForceMode2D.Impulse); //ForceMode2D�̐ݒ��Force��Impulse
            enemyJumpFlag = false;
            Debug.Log("Jump");
        }
    }
    //�W�����v����
    public void OnJump()
    {
        //Debug.Log(bjump);
        if (XboxDevice)
        {

            //else
            if (!Input.GetKey("joystick button 0")) return; //Xbox������Ă��Ȃ����
            if (bjump) return;
            bjump = true;
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); //ForceMode2D�̐ݒ��Force��Impulse
        }
        else
        {
            if (!Input.GetKey(KeyCode.Space)) return; //PC������Ă��Ȃ����
            if (bjump) return;
            bjump = true;
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); //ForceMode2D�̐ݒ��Force��Impulse
        }
    }
    //�ړ��i�L�[�{�[�h����̏ꍇ�iA�܂���D�A���E���j�AXbox����Ή��ς݁j
    public void OnMove()
    {
        float Move_horizontal = Input.GetAxis("Horizontal");
        float Move_vertical = Input.GetAxis("Vertical");
        inputDirection = new Vector2(Move_horizontal, Move_vertical);
    }

    //�ړ�����
    //x�����ɑ΂���moveSpeed��������x�����ɑ΂��ė͂�������
    private void MOVE()
    {
        //if (bjump) return;
        float currentMoveSpeed = bjump ? airControlSpeed : moveSpeed; // �W�����v���͌���
        rigid.velocity = new Vector2(inputDirection.x * currentMoveSpeed, rigid.velocity.y);
        //AnimationParameter�ō쐬����BOOL�^Walk�ɒl��ݒ肷��B�������͕ϐ���
        anim.SetBool("Walk", inputDirection.x != 0.0f); //�ړ��ʂ�0�o�Ȃ����true

    }
    private void LookMoveDirec()
    {
        if (inputDirection.x > 0.0f)
        {
            //�I�u�W�F�N�g�̊p�x���I�C���[�p(XYZ)�Ŏw�肷��
            //transform.eulerAngles = Vector3.zero;
            spriteRenderer.flipX = false;
        }
        else if (inputDirection.x < 0.0f)
        {   //�������Ɍ������Ƃ��AY180�x��]������
            //transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            spriteRenderer.flipX = true;
        }
    }
    //�W�����v����̃X�N���v�g
    private void hitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor"); //floor���C���[�̃��C���[�ԍ����擾
        Vector3 rayPos = transform.position - new Vector3(0.03f, transform.lossyScale.y / 2.0f); //�v���C���[�I�u�W�F�N�g�̑���
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.47f, 0.01f);
        RaycastHit2D hit = Physics2D.BoxCast(rayPos, raySize, 0.0f, Vector2.zero, 0.0f, layerMask);
        if (hit.transform == null && rigid.velocity.y != 0)
        {
            bjump = true;
            anim.SetBool("Jump", bjump);
            //Debug.Log("hit null");
            return;
        }
        else
        {
            bjump = false;
            anim.SetBool("Jump", bjump);
            return;
        }

        if (hit.transform.tag == "Floor" && bjump)
        {
            bjump = false;
            anim.SetBool("Jump", bjump);
            //Debug.Log("hit floor");
        }
       
    }
    //�W�����v����p�̕\���X�N���v�g
    void OnDrawGizmos()
    {
        Vector3 rayPos = transform.position - new Vector3(0.03f, transform.lossyScale.y / 2.0f); //�v���C���[�I�u�W�F�N�g�̑���
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.47f, 0.01f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(rayPos, raySize);
    }

    //�����蔻��������Ă���I�u�W�F�N�g�ɏՓ˂����Ƃ�
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            HitEnemy(collision.gameObject, decisionCollider(collision.collider));
            //Unity��Őݒ肵�����C���[�����w�肵�Ď擾���Đݒ�
        }

        if (collision.gameObject.tag == "BOSS")
        {
            HitBOSS(collision.gameObject, decisionCollider(collision.collider));
            //Unity��Őݒ肵�����C���[�����w�肵�Ď擾���Đݒ�
        }

        if (collision.gameObject.tag == "Goal")
        {
            FindObjectOfType<MainManager>().ShowGameClearUI();
            this.enabled = false;
            GetComponent<PlayerInput>().enabled = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("TrapDamege");
        if (collision.gameObject.tag == "Trap")
        {
            if (gameObject.layer != LayerMask.NameToLayer("PlayerDamage"))
            {
                gameObject.layer = LayerMask.NameToLayer("PlayerDamage");
                StartCoroutine(Damage());
                Damage(1);
                Dead();
            }
        }
        if (collision.gameObject.tag == "Item")
        {
            PlayerHPRecovery(collision.GetComponentInParent<Transform>().gameObject);
        }
        if (collision.gameObject.tag == "DeathLine")
        {
            collision.gameObject.GetComponent<Enemy>().PlayerDamage(this);
            //Unity��Őݒ肵�����C���[�����w�肵�Ď擾���Đݒ�
        }
        if (collision.gameObject.tag == "Enemy" && gameObject.layer == LayerMask.NameToLayer("PlayerDamage"))
        {
            Debug.Log("Enemy Attack");
            HitEnemy(collision.gameObject, decisionCollider(collision));
            //Unity��Őݒ肵�����C���[�����w�肵�Ď擾���Đݒ�
        }
    }


    float decisionCollider(Collider2D collider2d)
    {
        float enemysize = 0;
        if (collider2d is BoxCollider2D)
            enemysize = collider2d.GetComponent<BoxCollider2D>().size.y;
        else if (collider2d is CircleCollider2D)
            enemysize = collider2d.GetComponent<CircleCollider2D>().radius;
        return enemysize;
    }
    private void HitEnemy(GameObject enemy, float enemysizey)
    {
        //�v���C���[�̑����̈ʒu�����擾
        float halfScaleY = transform.lossyScale.y / 2.0f; //lossyScale�̓I�u�W�F�N�g�̑傫��(Scale)��xyz���W�ň����Ă���Vector3�^�̕ϐ�
                                                          //+ (enemy.GetComponent<BoxCollider2D>().offset.y)
        float enemyHalfScale = (enemy.transform.lossyScale.y - (enemysizey - enemy.transform.lossyScale.y)) / 2.0f - 0.1f;
        Debug.Log(enemyHalfScale);
        Debug.Log(enemy.transform.position.y + (enemyHalfScale - 0.2f));

        bool isStomping = transform.position.y - (halfScaleY - 0.15f) >= enemy.transform.position.y + (enemyHalfScale - 0.2f) && rigid.velocity.y <= 0;
        Debug.Log($"���݂�����: {isStomping}");
        //Player�̉������̈ʒu��Enemy�̏㔼����荂���ʒu�ɂ��邩�B-0.15f�͂߂荞�ݑ΍�
        if (transform.position.y - (halfScaleY - 0.15f) >= enemy.transform.position.y + (enemyHalfScale - 0.2f) && rigid.velocity.y <= 0)
        {
            Debug.Log("����OK");
            enemy.GetComponent<Enemy>().ReceiveDamage(GetHP(), this.gameObject);
            //Destroy(enemy);
            if (enemyJumpFlag) return;
            StartCoroutine(enemyFlag());


        }
        else
        {
            if (gameObject.layer == LayerMask.NameToLayer("PlayerDamage"))
                return;
            enemy.GetComponent<Enemy>().PlayerDamage(this);
            gameObject.layer = LayerMask.NameToLayer("PlayerDamage");
            Debug.Log("Player���_���[�W���󂯂�");
            StartCoroutine(Damage());
        }


    }
    private void HitBOSS(GameObject enemy, float enemysizey)
    {
        //�v���C���[�̑����̈ʒu�����擾
        float halfScaleY = transform.lossyScale.y / 2.0f; //lossyScale�̓I�u�W�F�N�g�̑傫��(Scale)��xyz���W�ň����Ă���Vector3�^�̕ϐ�
                                                          //+ (enemy.GetComponent<BoxCollider2D>().offset.y)
        float enemyHalfScale = (enemy.transform.lossyScale.y - (enemysizey - enemy.transform.lossyScale.y)) / 2.0f - 0.1f;
        Debug.Log(enemyHalfScale);
        Debug.Log(enemy.transform.position.y + (enemyHalfScale - 0.2f));

        bool isStomping = transform.position.y - (halfScaleY - 0.15f) >= enemy.transform.position.y + (enemyHalfScale - 0.2f) && rigid.velocity.y <= 0;
        Debug.Log($"���݂�����: {isStomping}");
        //Player�̉������̈ʒu��Enemy�̏㔼����荂���ʒu�ɂ��邩�B-0.15f�͂߂荞�ݑ΍�
        if (transform.position.y - (halfScaleY - 0.15f) >= enemy.transform.position.y + (enemyHalfScale - 0.2f))
        {
            Debug.Log("����OK");
            enemy.GetComponent<Enemy>().ReceiveDamage(GetHP(), this.gameObject);
            //Destroy(enemy);
            if (enemyJumpFlag) return;
            StartCoroutine(enemyFlag());


        }
        else
        {
            if (gameObject.layer == LayerMask.NameToLayer("PlayerDamage"))
                return;
            enemy.GetComponent<Enemy>().PlayerDamage(this);
            gameObject.layer = LayerMask.NameToLayer("PlayerDamage");
            StartCoroutine(Damage());
        }


    }
    IEnumerator enemyFlag()
    {
        enemyJumpFlag = true;
        yield return new WaitForSeconds(enemyJumpTime);
        enemyJumpFlag = false;
    }
    IEnumerator Damage()
    {
        //Color�^�ϐ� �f�t�H�͔�
        Color color = spriteRenderer.color;

        for (int i = 0; i < invincible; i++)
        {
            yield return new WaitForSeconds(flash);
            //�����x��0�Őݒ� �����Ȃ����
            spriteRenderer.color = new Color(color.r, color.g, color.b, 0.0f);

            //�����x1�Őݒ�@������
            yield return new WaitForSeconds(flash);
            spriteRenderer.color = new Color(color.r, color.g, color.b, 1.0f);
        }
        spriteRenderer.color = color;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
    private void Dead()
    {
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnBecameInvisible()
    {
        Camera camera = Camera.main;
        if (camera.name == "Main Camera" && camera.transform.position.y > transform.position.y) Destroy(gameObject);

    }
    //HP�̉�
    private void PlayerHPRecovery(GameObject obj)
    {
        StartCoroutine(heartAnimetion(obj));
    }
    IEnumerator heartAnimetion(GameObject obj)
    {

        if (hp >= 5)
        {
            Destroy(obj);
        }
        else
        {
            var hobj = Instantiate(heartObj, obj.transform.position, Quaternion.identity);
            //hobj.transform.parent = ScreenObj.transform;
            hobj.GetComponent<MoveToPosition>().target = ScreenObj.transform.position + new Vector3(hp * 0.4f, 0f, 0f);
            Destroy(obj);
            yield return new WaitForSeconds(1f);
            Damage(-1);// _   [ W     HP   
            Destroy(obj);
            Debug.Log("HPHeel");
        }

    }
    //�_���[�W����i�}�C�i�X����͂ŉ񕜂Ɏg�p�j
    public void Damage(int damage)
    {
        //�����̑傫������������ -�̒l�ƂȂ�Ȃ�����
        hp = Mathf.Max(hp - damage, 0);
        Dead();
    }
    //�O������HP�̊m�F
    public int GetHP()
    {
        return hp;
    }
}
