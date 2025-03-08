using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    [SerializeField, Header("移動速度")] private float moveSpeed;
    [SerializeField, Header("ジャンプ速度")] private float jumpSpeed;
    [SerializeField, Header("エネミーを踏んだ時のジャンプ起動時間")] private float enemyJumpTime;
    [SerializeField, Header("HP")] private int hp;
    [SerializeField, Header("無敵時間")] private float invincible;
    [SerializeField, Header("点滅時間")] private float flash;
    [SerializeField, Header("浮遊時の横移動速度")] private float airControlSpeed = 5f;


    private Vector2 inputDirection;
    private Rigidbody2D rigid;
    [SerializeField]
    private bool bjump;
    private bool enemyJumpFlag;

    private Animator anim;

    private SpriteRenderer spriteRenderer;
    private bool XboxDevice;    

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
        MOVE();
        OnMove();
        LookMoveDirec();
        hitFloor();
        OnJump();
        EnemyJump();
    }
    //Xboxコントローラとキーボード操作の切り替え
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
    //x方向に対してmoveSpeedをかけてx方向に対して力を加える
    private void MOVE()
    {
        //if (bjump) return;
        float currentMoveSpeed = bjump ? airControlSpeed : moveSpeed; // ジャンプ中は減速
        rigid.velocity = new Vector2(inputDirection.x * currentMoveSpeed, rigid.velocity.y);
        //AnimationParameterで作成したBOOL型Walkに値を設定する。第一引数は変数名
        anim.SetBool("Walk", inputDirection.x != 0.0f); //移動量が0出なければtrue
    }

    private void LookMoveDirec()
    {
        if (inputDirection.x > 0.0f)
        {
            //オブジェクトの角度をオイラー角(XYZ)で指定する
            //transform.eulerAngles = Vector3.zero;
            spriteRenderer.flipX = false;
        }
        else if (inputDirection.x < 0.0f)
        {   //左方向に向けたとき、Y180度回転させる
            //transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            spriteRenderer.flipX = true;
        }
    }
    //当たり判定を持っているオブジェクトに衝突したとき
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
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
            PlayerHPRecovery(collision.GetComponentInParent<Transform>().gameObject);
        }
        if (collision.gameObject.tag == "DeathLine")
        {
            collision.gameObject.GetComponent<Enemy>().PlayerDamage(this);
            //Unity上で設定したレイヤー名を指定して取得して設定
        }
        if (collision.gameObject.tag == "Enemy"&& gameObject.layer == LayerMask.NameToLayer("PlayerDamage"))
        {
            Debug.Log("Enemy Attack");
            HitEnemy(collision.gameObject);
            //Unity上で設定したレイヤー名を指定して取得して設定
        }
    }
    //ジャンプ判定のスクリプト
    private void hitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor"); //floorレイヤーのレイヤー番号を取得
        Vector3 rayPos = transform.position - new Vector3(0.0f + 0.03f, transform.lossyScale.y / 2.0f); //プレイヤーオブジェクトの足元
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.35f, 0.05f);
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
    //ジャンプ判定用の表示スクリプト
    void OnDrawGizmos()
    {
        Vector3 rayPos = transform.position - new Vector3(0.0f+ 0.03f, transform.lossyScale.y / 2.0f); //プレイヤーオブジェクトの足元
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.35f, 0.05f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(rayPos, raySize);
    }
    private void EnemyJump()
    {
        if (!enemyJumpFlag) return;
        if (XboxDevice)
        {
            if (!Input.GetKeyDown("joystick button 0")) return; //Xbox押されていなければ
            if(!bjump) return;
            
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpSpeed * 1.2f, ForceMode2D.Impulse); //ForceMode2Dの設定はForceかImpulse
            enemyJumpFlag = false;
            Debug.Log("Jump");
            
        }
        else
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return; //PC押されていなければ
            if (!bjump) return;
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpSpeed * 1.2f, ForceMode2D.Impulse); //ForceMode2Dの設定はForceかImpulse
            enemyJumpFlag = false;
            Debug.Log("Jump");
        }
    }
    private void HitEnemy(GameObject enemy)
    {
        float halfScaleY = transform.lossyScale.y / 2.0f; //lossyScaleはオブジェクトの大きさ(Scale)をxyz座標で扱っているVector3型の変数
        float enemyHalfScale = (enemy.transform.lossyScale.y - 0.1f) / 2.0f;

        //Playerの下半分の位置がEnemyの上半分より高い位置にいるか。-0.1fはめり込み対策
        if (transform.position.y - (halfScaleY - 0.1f) >= enemy.transform.position.y + (enemyHalfScale - 0.1f))
        {
            enemy.GetComponent<Enemy>().ReceiveDamage(GetHP());
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
        //Color型変数 デフォは白
        Color color = spriteRenderer.color;

        for (int i = 0; i < invincible; i++)
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
    //ジャンプ処理
    public void OnJump()
    {
        if (XboxDevice)
        {

            //else
            if (!Input.GetKeyDown("joystick button 0") || bjump) return; //Xbox押されていなければ
            rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); //ForceMode2Dの設定はForceかImpulse
        }
        else
        {
            if (!Input.GetKeyDown(KeyCode.Space) || bjump) return; //PC押されていなければ
            rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); //ForceMode2Dの設定はForceかImpulse
        }
    }
    //移動（キーボード操作の場合（AまたはD、左右矢印）、Xbox操作対応済み）
    public void OnMove()
    {
        float Move_horizontal = Input.GetAxis("Horizontal");
        float Move_vertical = Input.GetAxis("Vertical");
        inputDirection = new Vector2(Move_horizontal, Move_vertical);
    }
    //HPの回復
    private void PlayerHPRecovery(GameObject obj)
    {
        if (hp >= 5)
        {
            Destroy(obj);
            return;
        }
        else
        {
            Damage(-1);//ダメージ判定でHPを回復
            Destroy(obj);
            Debug.Log("HPHeel");
        }
    }
    //ダメージ判定（マイナスを入力で回復に使用）
    public void Damage(int damage)
    {
        //数字の大きい方を代入する -の値とならないため
        hp = Mathf.Max(hp - damage, 0);
        Dead();
    }
    //外部からHPの確認
    public int GetHP()
    {
        return hp;
    }
}
