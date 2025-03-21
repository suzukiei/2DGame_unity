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

    }
}
