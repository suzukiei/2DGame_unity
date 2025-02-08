using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    [SerializeField,Header("移動速度")]private float moveSpeed;
    [SerializeField,Header("ジャンプ速度")] private float jumpSpeed;
    [SerializeField, Header("HP")] private int hp;
    [SerializeField, Header("無敵時間")] private float invincible;
    [SerializeField, Header("点滅時間")] private float flash;
    [SerializeField, Header("浮遊時の横移動速度")] private float airControlSpeed = 5f;


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

    //x方向に対してmoveSpeedをかけてx方向に対して力を加える
    private void MOVE()
    {
        if (bjump) return;
        float currentMoveSpeed = bjump ? airControlSpeed : moveSpeed; // ジャンプ中は減速
        rigid.velocity = new Vector2(inputDirection.x * currentMoveSpeed, rigid.velocity.y);
        //AnimationParameterで作成したBOOL型Walkに値を設定する。第一引数は変数名
        anim.SetBool("Walk", inputDirection.x != 0.0f); //移動量が0出なければtrue
    }

    private void LookMoveDirec()
    {
        if(inputDirection.x > 0.0f)
        {
            //オブジェクトの角度をオイラー角(XYZ)で指定する
            transform.eulerAngles = Vector3.zero;

        }else if(inputDirection.x < 0.0f)
        {   //左方向に向けたとき、Y180度回転させる
            transform.eulerAngles = new Vector3(0.0f,180.0f,0.0f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("TrapDamege");
        if (collision.gameObject.tag == "Trap")
        {
            //Debug.Log("TrapDamegeTag");
            StartCoroutine(Damage());
            Damage(1);
            Dead();
        }
        if (collision.gameObject.tag == "Item")
        {
            Damage(-1);
            Destroy(collision.GetComponentInParent<Transform>().gameObject);
        }
    }

    //当たり判定を持っているオブジェクトに衝突したとき
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if(collision.gameObject.tag == "Floor")//それがFloorである場合
        //{
        //    bjump = false;
        //    anim.SetBool("Jump", bjump);
        //}
        if(collision.gameObject.tag == "Enemy")
        {
            HitEnemy(collision.gameObject);
            //Unity上で設定したレイヤー名を指定して取得して設定
        }
       
        if (collision.gameObject.tag == "Goal")
        {
            FindObjectOfType<MainManager>().ShowGameClearUI();
            this.enabled = false;
            GetComponent<PlayerInput>().enabled = false;
        }
    }

    private void hitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor"); //floorレイヤーのレイヤー番号を取得
        Vector3 rayPos = transform.position - new Vector3(0.0f, transform.lossyScale.y / 2.0f); //プレイヤーオブジェクトの足元
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.1f,0.1f);

        RaycastHit2D hit = Physics2D.BoxCast(rayPos, raySize, 0.0f, Vector2.zero, 0.0f, layerMask);
        if (hit.transform == null)
        {
            bjump = true;
            anim.SetBool("Jump", bjump);
            //Debug.Log("hit null");
            return;
        }
        if (hit.transform.tag == "Floor" && bjump)
        {
            bjump = false;
            anim.SetBool("Jump", bjump);
            //Debug.Log("hit floor");
        }



    }

    private void HitEnemy(GameObject enemy)
    {
        float halfScaleY = transform.lossyScale.y / 2.0f; //lossyScaleはオブジェクトの大きさ(Scale)をxyz座標で扱っているVector3型の変数
        float enemyHalfScale = enemy.transform.lossyScale.y / 2.0f;

        //Playerの下半分の位置がEnemyの上半分より高い位置にいるか。-0.1fはめり込み対策
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
        //Color型変数 デフォは白
        Color color = spriteRenderer.color;

        for(int i = 0; i < invincible; i++)
        {
            yield return new WaitForSeconds(flash);
            //透明度は0で設定 見えない状態
            spriteRenderer.color = new Color(color.r, color.g, color.b, 0.0f);

            //透明度1で設定　見える
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
        var controllers = Input.GetJoystickNames();
        if (controllers[0]=="")
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return; //PC押されていなければ
        }
        else
        if (!Input.GetKeyDown("joystick button 0")) return; //Xbox押されていなければ

        Debug.Log(controllers.Length);
        rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); //ForceMode2Dの設定はForceかImpulse
        //bjump = true;
        //anim.SetBool("Jump", bjump);

        //Debug.Log("jump");

        /*
         forceは初速が遅く、徐々に加速 
        Impulseは初速が速いが徐々に減速 
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
        //数字の大きい方を代入する -の値とならないため
        hp = Mathf.Max(hp - damage, 0);
        Dead();
    }

    public int GetHP()
    {
        return hp;
    }
}
