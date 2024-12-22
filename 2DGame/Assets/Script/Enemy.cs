using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField, Header("�ړ����x")] private float moveSpeed;
    [SerializeField, Header("�_���[�W")] private int attackPower;
   
    private Vector2 moveDirec;


    private Rigidbody2D rigid;
    private Animator Anim;
    private bool bfloor;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>(); //Enemy�I�u�W�F�N�g�ɓK�p����Ă���Rigidbody2D��rigid�ϐ��ɑ��
        Anim = GetComponent<Animator>();
        moveDirec = Vector2.left;
        bfloor = true;
    }

    // Update is called once per frame
    void Update()
    {
        move();
        ChangeMoveDirec();
        LookMoveDirec();
        HitFloor();
    }   

    private void move()
    {

        if (!bfloor) return;
        rigid.velocity = new Vector2(moveDirec.x * moveSpeed, rigid.velocity.y);

    }

    private void ChangeMoveDirec()
    {
        Vector2 halfSize = transform.localScale / 2.0f;
        int layerMask = LayerMask.GetMask("Floor");
        RaycastHit2D ray = Physics2D.Raycast(transform.position, -transform.right, halfSize.x + 0.1f, layerMask); //raycast�͒���
        if (ray.transform == null) return;

        if(ray.transform.tag == "Floor")
        {
            moveDirec = -moveDirec;
        }
    }

    private void LookMoveDirec()
    {
        if(moveDirec .x < 0.0f)
        {
            transform.eulerAngles = Vector3.zero;
        }else if(moveDirec.x > 0.0f)
        {
            transform.eulerAngles = new Vector3(0.0f,180.0f,0.0f);   
        }
    }

    private void HitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor");
        Vector3 rayPos = transform.position - new Vector3(0.0f, transform.lossyScale.y / 2.0f); //Enemy����
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.1f, 0.1f); //�����蔻��̑傫��
        RaycastHit2D Hit = Physics2D.BoxCast(rayPos,raySize,0.0f,Vector2.zero,layerMask);

        if (Hit.transform == null)
        {
            bfloor = false;
            Anim.SetBool("IsIdle", true);
            return;
        }
        else if(Hit.transform.tag == "Floor" && !bfloor)
        {
            bfloor = true;
            Anim.SetBool("IsIdle", false);
        }
    }

    public void PlayerDamage(Player player)
    {
        player.Damage(attackPower);
    }
}
