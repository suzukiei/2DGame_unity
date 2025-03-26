using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private FireBar parentFireBar;
    [SerializeField] private int damage = 1;

    void Start()
    {
        // e‚©‚çQÆ‚ğæ“¾
        parentFireBar = GetComponentInParent<FireBar>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

        }
    }

    public void PlayerDamage(Player player)
    {
        player.Damage(damage);
    }
}
