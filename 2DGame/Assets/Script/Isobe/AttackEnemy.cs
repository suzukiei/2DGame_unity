using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemy : MonoBehaviour,Enemy
{
    [SerializeField, Header("移動速度")] private float moveSpeed;
    [SerializeField, Header("ダメージ")] private int attackPower;
    [SerializeField, Header("アイテム")] private GameObject Item;
    [SerializeField, Header("アイテムドロップ率")] private int ItemDropPercent;
    private Vector2 moveDirec;
    private Rigidbody2D rigid;
    private Animator Anim;
    private bool bfloor;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>(); //Enemyオブジェクトに適用されているRigidbody2Dをrigid変数に代入
        Anim = GetComponent<Animator>();
        moveDirec = Vector2.left;
        bfloor = true;
    }

    // Update is called once per frame
    void Update()
    {
        //移動
        move();
        //壁の判定確認
        ChangeMoveDirec();
        //壁判定後の向きの反転
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
        RaycastHit2D ray = Physics2D.Raycast(transform.position, -transform.right, halfSize.x + 0.1f, layerMask); //raycastは直線
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

    private void HitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor");
        Vector3 rayPos = transform.position - new Vector3(0.0f, transform.lossyScale.y / 2.0f); //Enemy足元
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.1f, 0.1f); //当たり判定の大きさ
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

    public void ReceiveDamage(int _hp)
    {
        ItemCreate(_hp);
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
}
