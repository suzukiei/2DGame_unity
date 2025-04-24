using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBullet : MonoBehaviour,Enemy
{
    [SerializeField] private int damage = 1; // �e�̃_���[�W��
    [SerializeField] private float lifeTime = 5f; // �e�̎���

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ������ݒ�
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �v���C���[�ɓ��������ꍇ
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage(damage);
            }

            // �e��j��
            DestroyBullet();
        }

        // �n�ʂ�ǂɓ��������ꍇ
        if (other.CompareTag("Floor"))
        {

            // �e��j��
            DestroyBullet();
        }
    }

    // �e��j��
    private void DestroyBullet()
    {
       
            // �A�j���[�^�[���Ȃ���Α����j��
            Destroy(gameObject);
        
    }

    public void PlayerDamage(Player player)
    {
        player.Damage(damage);
        Destroy(gameObject);

    }
}
