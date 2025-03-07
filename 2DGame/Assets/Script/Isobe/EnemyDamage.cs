using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour,Enemy
{
    [SerializeField, Header("�A�C�e��")] private GameObject Item;
    [SerializeField, Header("�A�C�e���h���b�v��")] private int ItemDropPercent;
    public void ReceiveDamage(int _hp)
    {
        Debug.Log("Enemydamege");
        ItemCreate(_hp);
        Destroy(this.transform.parent.gameObject);
    }

    private void ItemCreate(int _hp)
    {
        if (_hp >= 5) return;
        //var drop = Random.Range(1, 100);
        //Debug.Log(drop);
        //if (drop >= ItemDropPercent) return;
        var itemobj = Instantiate(Item, this.transform.position, Quaternion.identity);
    }
}
