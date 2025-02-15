using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLine : MonoBehaviour,Enemy
{
    [SerializeField, Header("�_���[�W")] private int attackPower;
    public void PlayerDamage(Player player)
    {
        player.Damage(attackPower);
    }
}
