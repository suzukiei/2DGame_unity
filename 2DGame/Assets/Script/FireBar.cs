using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBar : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 90f; // 1�b�Ԃ�90�x��]�i�����\�j
    [SerializeField,Header("���v���ɂ���")] private bool clockwise = true; // ���v��肩�����v��肩

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ��]�����ɉ����āA���܂��͕��̉�]��K�p
        float direction = clockwise ? -1f : 1f;

        // ���t���[����]��K�p
        transform.Rotate(0, 0, rotationSpeed * direction * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �v���C���[�Ƀ_���[�W��^����Ȃǂ̏������ꂩ�炩��
        }
    }
}
