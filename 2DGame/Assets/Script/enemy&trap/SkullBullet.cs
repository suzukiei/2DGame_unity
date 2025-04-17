using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBullet : MonoBehaviour
{
    [SerializeField] private int damage = 1; // ’e‚Ìƒ_ƒ[ƒW—Ê
    [SerializeField] private float lifeTime = 5f; // ’e‚Ìõ–½

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // õ–½‚ğİ’è
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ƒvƒŒƒCƒ„[‚É“–‚½‚Á‚½ê‡
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage(damage);
            }

            // ’e‚ğ”j‰ó
            DestroyBullet();
        }

        // ’n–Ê‚â•Ç‚É“–‚½‚Á‚½ê‡
        if (other.CompareTag("Floor") || other.CompareTag("Wall"))
        {

            // ’e‚ğ”j‰ó
            DestroyBullet();
        }
    }

    // ’e‚ğ”j‰ó
    private void DestroyBullet()
    {
       
            // ƒAƒjƒ[ƒ^[‚ª‚È‚¯‚ê‚Î‘¦”j‰ó
            Destroy(gameObject);
        
    }
}
