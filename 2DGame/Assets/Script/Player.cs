using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    [SerializeField,Header("�ړ����x")]private float moveSpeed;
    [SerializeField,Header("�W�����v���x")] private float jumpSpeed;
    [SerializeField, Header("HP")] private int hp;
    [SerializeField, Header("���G����")] private float invincible;
    [SerializeField, Header("�_�Ŏ���")] private float flash;
    [SerializeField, Header("���V���̉��ړ����x")] private float airControlSpeed = 5f;


    private Vector2 inputDirection;
    private Rigidbody2D rigid;
    private bool bjump;

    private Animator anim;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        bjump = false;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MOVE();
        OnMove();
        LookMoveDirec();
        hitFloor();
        OnJump();
      
    }

    //x�����ɑ΂���moveSpeed��������x�����ɑ΂��ė͂�������
    private void MOVE()
    {
        if (bjump) return;
        float currentMoveSpeed = bjump ? airControlSpeed : moveSpeed; // �W�����v���͌���
        rigid.velocity = new Vector2(inputDirection.x * currentMoveSpeed, rigid.velocity.y);
        //AnimationParameter�ō쐬����BOOL�^Walk�ɒl��ݒ肷��B�������͕ϐ���
        anim.SetBool("Walk", inputDirection.x != 0.0f); //�ړ��ʂ�0�o�Ȃ����true
    }

    private void LookMoveDirec()
    {
        if(inputDirection.x > 0.0f)
        {
            //�I�u�W�F�N�g�̊p�x���I�C���[�p(XYZ)�Ŏw�肷��
            transform.eulerAngles = Vector3.zero;

        }else if(inputDirection.x < 0.0f)
        {   //�������Ɍ������Ƃ��AY180�x��]������
            transform.eulerAngles = new Vector3(0.0f,180.0f,0.0f);
        }
    }

    //�����蔻��������Ă���I�u�W�F�N�g�ɏՓ˂����Ƃ�
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if(collision.gameObject.tag == "Floor")//���ꂪFloor�ł���ꍇ
        //{
        //    bjump = false;
        //    anim.SetBool("Jump", bjump);
        //}
        if(collision.gameObject.tag == "Enemy")
        {
            HitEnemy(collision.gameObject);
            //Unity��Őݒ肵�����C���[�����w�肵�Ď擾���Đݒ�
            


        }

        if(collision.gameObject.tag == "Goal")
        {
            FindObjectOfType<MainManager>().ShowGameClearUI();
            this.enabled = false;
            GetComponent<PlayerInput>().enabled = false;
        }
    }

    private void hitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor"); //floor���C���[�̃��C���[�ԍ����擾
        Vector3 rayPos = transform.position - new Vector3(0.0f, transform.lossyScale.y / 2.0f); //�v���C���[�I�u�W�F�N�g�̑���
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.1f,0.1f);

        RaycastHit2D hit = Physics2D.BoxCast(rayPos, raySize, 0.0f, Vector2.zero, 0.0f, layerMask);
        if (hit.transform == null)
        {
            bjump = true;
            anim.SetBool("Jump", bjump);
            Debug.Log("hit null");
            return;
        }
        if (hit.transform.tag == "Floor" && bjump)
        {
            bjump = false;
            anim.SetBool("Jump", bjump);
            Debug.Log("hit floor");
        }



    }

    private void HitEnemy(GameObject enemy)
    {
        float halfScaleY = transform.lossyScale.y / 2.0f; //lossyScale�̓I�u�W�F�N�g�̑傫��(Scale)��xyz���W�ň����Ă���Vector3�^�̕ϐ�
        float enemyHalfScale = enemy.transform.lossyScale.y / 2.0f;

        //Player�̉������̈ʒu��Enemy�̏㔼����荂���ʒu�ɂ��邩�B-0.1f�͂߂荞�ݑ΍�
        if(transform.position.y - (halfScaleY - 0.1f) >= enemy.transform.position.y + (enemyHalfScale - 0.1f))
        {
            enemy.GetComponent<Enemy>().ReceiveDamage();
            //Destroy(enemy);
            rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
        else
        {
            
            enemy.GetComponent<Enemy>().PlayerDamage(this);
            gameObject.layer = LayerMask.NameToLayer("PlayerDamage");
            StartCoroutine(Damage());
        }
        

    }

    //
    IEnumerator Damage()
    {
        //Color�^�ϐ� �f�t�H�͔�
        Color color = spriteRenderer.color;

        for(int i = 0; i < invincible; i++)
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
        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Camera camera = Camera.main;
        if(camera.name == "Main Camera" && camera.transform.position.y > transform.position.y) Destroy(gameObject);
    }



    public void OnJump()
    {
        Debug.Log(bjump);
        if (!Input.GetKeyDown(KeyCode.Space)) return; //������Ă��Ȃ����

        rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); //ForceMode2D�̐ݒ��Force��Impulse
        //bjump = true;
        //anim.SetBool("Jump", bjump);

        Debug.Log("jump");

        /*
         force�͏������x���A���X�ɉ��� 
        Impulse�͏��������������X�Ɍ��� 
         */
    }

    public void OnMove()
    {
        float Move_horizontal = Input.GetAxis("Horizontal");
        float Move_vertical = Input.GetAxis("Vertical");

        inputDirection = new Vector2(Move_horizontal, Move_vertical);


    }

    public void Damage(int damage)
    {
        //�����̑傫������������ -�̒l�ƂȂ�Ȃ�����
        hp = Mathf.Max(hp - damage, 0);
        Dead();
    }

    public int GetHP()
    {
        return hp;
    }
}
