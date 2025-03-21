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

    }
}
