using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField, Header("移動速度")] private float moveSpeed;
    [SerializeField, Header("ジャンプ速度")] private float jumpSpeed;
    [SerializeField, Header("エネミーを踏んだ時のジャンプ起動時間")] private float enemyJumpTime;
    [SerializeField, Header("HP")] private int hp;
    [SerializeField, Header("無敵時間")] private float invincible;
    [SerializeField, Header("点滅時間")] private float flash;
    [SerializeField, Header("浮遊時の横移動速度")] private float airControlSpeed = 5f;
    [SerializeField, Header("heartオブジェクト")] private GameObject heartObj;
    [SerializeField, Header("Screen")] private RectTransform ScreenObj;
    [SerializeField, Header("FlagNumber")] private int flagNumber;

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

    ///Start、Update
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
    ///キー入力など
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
    //エネミーヘッドのジャンプ処理
    private void EnemyJump()
    {
        if (!enemyJumpFlag) return;
        if (XboxDevice)
        {
            if (!Input.GetKeyDown("joystick button 0")) return; //Xbox押されていなければ
            if (!bjump) return;

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
    //ジャンプ処理
    public void OnJump()
    {
        //Debug.Log(bjump);
        if (XboxDevice)
        {

            //else
            if (!Input.GetKey("joystick button 0")) return; //Xbox押されていなければ
            if (bjump) return;
            bjump = true;
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); //ForceMode2Dの設定はForceかImpulse
        }
        else
        {
            if (!Input.GetKey(KeyCode.Space)) return; //PC押されていなければ
            if (bjump) return;
            bjump = true;
            rigid.velocity = Vector2.zero;
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

    //移動処理
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
    //ジャンプ判定のスクリプト
    private void hitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor"); //floorレイヤーのレイヤー番号を取得
        Vector3 rayPos = transform.position - new Vector3(0.03f, transform.lossyScale.y / 2.0f); //プレイヤーオブジェクトの足元
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.5f, 0.02f);
        RaycastHit2D hit = Physics2D.BoxCast(rayPos, raySize, 0.0f, Vector2.zero, 0.0f, layerMask);
        if (hit.transform == null )
        {
            if(rigid.velocity.y != 0)
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
                //    Debug.Log("hit floor");
                return;
            }

        }
        else if (hit.transform.tag == "Floor" && bjump)
        {
            bjump = false;
            anim.SetBool("Jump", bjump);
            return;
        }

        //{
        //    bjump = false;
        //    anim.SetBool("Jump", bjump);
        //    Debug.Log("hit floor");
        //}

    }
    //ジャンプ判定用の表示スクリプト
    void OnDrawGizmos()
    {
        Vector3 rayPos = transform.position - new Vector3(0.03f, transform.lossyScale.y / 2.0f); //プレイヤーオブジェクトの足元
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.5f, 0.01f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(rayPos, raySize);
    }

    //当たり判定を持っているオブジェクトに衝突したとき
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            HitEnemy(collision.gameObject, decisionCollider(collision.collider));
            //Unity上で設定したレイヤー名を指定して取得して設定
        }

        if (collision.gameObject.tag == "BOSS")
        {
            HitBOSS(collision.gameObject, decisionCollider(collision.collider));
            //Unity上で設定したレイヤー名を指定して取得して設定
        }

        if (collision.gameObject.tag == "Goal")
        {
            if(GameManager.Instance.select <= flagNumber)
            GameManager.Instance.select = flagNumber;
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
            //Unity上で設定したレイヤー名を指定して取得して設定
        }
        if (collision.gameObject.tag == "Enemy" && gameObject.layer == LayerMask.NameToLayer("PlayerDamage"))
        {
            Debug.Log("Enemy Attack");
            HitEnemy(collision.gameObject, decisionCollider(collision));
            //Unity上で設定したレイヤー名を指定して取得して設定
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
        //プレイヤーの足元の位置情報を取得
        float halfScaleY = transform.lossyScale.y / 2.0f; //lossyScaleはオブジェクトの大きさ(Scale)をxyz座標で扱っているVector3型の変数
                                                          //+ (enemy.GetComponent<BoxCollider2D>().offset.y)
        float enemyHalfScale = (enemy.transform.lossyScale.y - (enemysizey - enemy.transform.lossyScale.y)) / 2.0f - 0.1f;
        Debug.Log(enemyHalfScale);
        Debug.Log(enemy.transform.position.y + (enemyHalfScale - 0.2f));

        bool isStomping = transform.position.y - (halfScaleY - 0.15f) >= enemy.transform.position.y + (enemyHalfScale - 0.2f) && rigid.velocity.y <= 0;
        Debug.Log($"踏みつけ判定: {isStomping}");
        //Playerの下半分の位置がEnemyの上半分より高い位置にいるか。-0.15fはめり込み対策
        if (transform.position.y - (halfScaleY - 0.15f) >= enemy.transform.position.y + (enemyHalfScale - 0.2f) && rigid.velocity.y <= 0)
        {
            Debug.Log("判定OK");
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
            Debug.Log("Playerがダメージを受ける");
            StartCoroutine(Damage());
        }


    }
    private void HitBOSS(GameObject enemy, float enemysizey)
    {
        //プレイヤーの足元の位置情報を取得
        float halfScaleY = transform.lossyScale.y / 2.0f; //lossyScaleはオブジェクトの大きさ(Scale)をxyz座標で扱っているVector3型の変数
                                                          //+ (enemy.GetComponent<BoxCollider2D>().offset.y)
        float enemyHalfScale = (enemy.transform.lossyScale.y - (enemysizey - enemy.transform.lossyScale.y)) / 2.0f - 0.1f;
        Debug.Log(enemyHalfScale);
        Debug.Log(enemy.transform.position.y + (enemyHalfScale - 0.2f));

        bool isStomping = transform.position.y - (halfScaleY - 0.15f) >= enemy.transform.position.y + (enemyHalfScale - 0.2f) && rigid.velocity.y <= 0;
        Debug.Log($"踏みつけ判定: {isStomping}");
        //Playerの下半分の位置がEnemyの上半分より高い位置にいるか。-0.15fはめり込み対策
        if (transform.position.y - (halfScaleY - 0.15f) >= enemy.transform.position.y + (enemyHalfScale - 0.2f))
        {
            Debug.Log("判定OK");
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
    //HPの回復
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
            hobj.GetComponent<MoveToPosition>().target = Camera.main.ScreenToWorldPoint(ScreenObj.position) + new Vector3(hp * 0.4f, 0f, 0f);
            Destroy(obj);
            yield return new WaitForSeconds(1f);
            Damage(-1);// _   [ W     HP   
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
