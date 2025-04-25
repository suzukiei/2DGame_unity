using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemy : MonoBehaviour,Enemy
{
    [SerializeField, Header("ˆÚ“®‘¬“x")] private float moveSpeed;
    [SerializeField, Header("ƒ_ƒ[ƒW")] private int attackPower;
    [SerializeField, Header("ƒAƒCƒeƒ€")] private GameObject Item;
    [SerializeField, Header("ƒAƒCƒeƒ€ƒhƒƒbƒv—¦")] private int ItemDropPercent;
    [SerializeField, Header("ƒGƒtƒFƒNƒg")] private GameObject effectanim;
    private Vector2 moveDirec;
    protected Rigidbody2D rigid;
    protected Animator Anim;
    protected bool bfloor;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody2D>(); //EnemyƒIƒuƒWƒFƒNƒg‚É“K—p‚³‚ê‚Ä‚¢‚éRigidbody2D‚ğrigid•Ï”‚É‘ã“ü
        Anim = GetComponent<Animator>();
        moveDirec = Vector2.left;
        bfloor = true;
        EnemyManager.Instance.setEnemyObjListAdd(this.gameObject);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //ˆÚ“®
        move();
        //•Ç‚Ì”»’èŠm”F
        ChangeMoveDirec();
        //•Ç”»’èŒã‚ÌŒü‚«‚Ì”½“]
        LookMoveDirec();
        //
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
        RaycastHit2D ray = Physics2D.Raycast(transform.position, -transform.right, halfSize.x + 0.1f, layerMask); //raycast‚Í’¼ü
        Debug.DrawRay(transform.position, -transform.right, Color.red);
        if (ray.transform == null) return;

        if (ray.transform.tag == "Floor")
        {
            moveDirec = -moveDirec;
        }
    }

    private void LookMoveDirec()
    {
        if (moveDirec.x < 0.0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (moveDirec.x > 0.0f)
        {
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }

    }

    protected void HitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor");
        Vector3 rayPos = transform.position - new Vector3(0.0f, transform.lossyScale.y / 2.0f); //Enemy‘«Œ³
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.1f, 0.1f); //“–‚½‚è”»’è‚Ì‘å‚«‚³
        RaycastHit2D Hit = Physics2D.BoxCast(rayPos, raySize, 0.0f, Vector2.zero, layerMask);

        if (Hit.transform == null)
        {
            Debug.Log("enemy");
            bfloor = false;
            Anim.SetBool("IsIdle", true);
            return;
        }
        else if (Hit.transform.tag == "Floor" && !bfloor)
        {
            Debug.Log("enemyFloor");
            bfloor = true;
            Anim.SetBool("IsIdle", false);
        }
    }

    public void PlayerDamage(Player player)
    {
        player.Damage(attackPower);
    }

<<<<<<< Updated upstream
    public virtual void ReceiveDamage(int _hp, GameObject player = null)
=======
<<<<<<< HEAD
    public void ReceiveDamage(int _hp)
=======
    public virtual void ReceiveDamage(int _hp)
>>>>>>> parent of ee7931c (4æœˆ25æ—¥ãƒãƒ¼ã‚¸)
>>>>>>> Stashed changes
    {
        ItemCreate(_hp);
        Instantiate(effectanim, this.transform.position, Quaternion.identity);
        EnemyManager.Instance.DestroyEnemyObjList(this.gameObject);
        Destroy(this.gameObject);
    }

    private void ItemCreate(int _hp)
    {
        if (_hp >= 5) return;
        var drop = Random.Range(1, 100);
        Debug.Log(drop);
        if (drop >= ItemDropPercent) return;
        var itemobj = Instantiate(Item, this.transform.position, Quaternion.identity);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DeathLine")
        {
            EnemyManager.Instance.DestroyEnemyObjList(this.gameObject);
            Destroy(this.gameObject);
        }
    }
}
