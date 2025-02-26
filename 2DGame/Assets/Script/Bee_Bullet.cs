using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee_Bullet : MonoBehaviour,Enemy
{
    [SerializeField] private float lifeTime = 3f;  // �e�̐�������
    [SerializeField] private int damage = 1;       // �^����_���[�W

    void Start()
    {
        // ��莞�Ԍ�ɒe���폜
        Destroy(gameObject, lifeTime);


        // 2�̃R���C�_�[�̐ݒ���m�F
        CircleCollider2D[] colliders = GetComponents<CircleCollider2D>();
        if (colliders.Length == 2)
        {
            // 1�ڂ̃R���C�_�[�F�v���C���[�p�i�����Փˁj
            colliders[0].isTrigger = false;

            // 2�ڂ̃R���C�_�[�F���p�i�g���K�[�j
            colliders[1].isTrigger = true;
        }
    }

    public void PlayerDamage(Player player)
    {
        player.Damage(damage);
        Destroy(gameObject);

    }

    // ���Ƃ̃g���K�[����
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        // �v���C���[�Ƀ_���[�W��^���鏈��
    //        var player = other.GetComponent<Player>(); // �v���C���[�̗̑͊Ǘ��X�N���v�g
    //        if (player != null)
    //        {
    //            player.Damage(damage);
    //        }
    //        Destroy(gameObject);
    //    }
    //    else if (other.CompareTag("Floor")) // �n�ʂɓ��������ꍇ
    //    {
    //        Destroy(gameObject);
    //    }
    //}
}
