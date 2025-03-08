using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSceneManager : MonoBehaviour
{
    //プレイヤー用
    //[SerializeField, Header("移動速度")] private float moveSpeed;
    //ステージ管理
    [SerializeField, Header("ステージ番号")] public int StageIndex;
    [SerializeField, Header("ステージ名")] public string StageName;


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
    //    //AnimationParameterで作成したBOOL型Walkに値を設定する。第一引数は変数名
    //    anim.SetBool("Walk", inputDirection.x != 0.0f || inputDirection.y != 0.0f); //移動量が0出なければtrue
    //}


    //public void OnMove()
    //{
    //    float Move_horizontal = Input.GetAxis("Horizontal");
    //    float Move_vertical = Input.GetAxis("Vertical");

    //    inputDirection = new Vector2(Move_horizontal, Move_vertical);


    //}
}
