using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSceneManager : MonoBehaviour
{
    //�v���C���[�p
    //[SerializeField, Header("�ړ����x")] private float moveSpeed;
    //�X�e�[�W�Ǘ�
    [SerializeField, Header("�X�e�[�W�ԍ�")] public int StageIndex;
    [SerializeField, Header("�X�e�[�W��")] public string StageName;


    private Rigidbody2D rigid;
    private bool bjump;
    private Animator anim;
    private Vector2 inputDirection;
   
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
        //MOVE();
        //OnMove();
    }

    //private void MOVE()
    //{
    //    if (bjump) return;
    //    rigid.velocity = new Vector2(inputDirection.x * moveSpeed, inputDirection.y * moveSpeed);
    //    //AnimationParameter�ō쐬����BOOL�^Walk�ɒl��ݒ肷��B�������͕ϐ���
    //    anim.SetBool("Walk", inputDirection.x != 0.0f || inputDirection.y != 0.0f); //�ړ��ʂ�0�o�Ȃ����true
    //}


    //public void OnMove()
    //{
    //    float Move_horizontal = Input.GetAxis("Horizontal");
    //    float Move_vertical = Input.GetAxis("Vertical");

    //    inputDirection = new Vector2(Move_horizontal, Move_vertical);


    //}
}
