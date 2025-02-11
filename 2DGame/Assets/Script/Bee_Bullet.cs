using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee_Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;  // �e�̐�������
    [SerializeField] private int damage = 1;       // �^����_���[�W

    void Start()
    {
        // ��莞�Ԍ�ɒe���폜
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �v���C���[�Ƀ_���[�W��^���鏈��
            var player = other.GetComponent<Player>(); // �v���C���[�̗̑͊Ǘ��X�N���v�g
            if (player != null)
            {
                player.Damage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Floor")) // �n�ʂɓ��������ꍇ
        {
            Destroy(gameObject);
        }
    }
}
