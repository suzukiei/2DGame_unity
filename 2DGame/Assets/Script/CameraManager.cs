using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    [SerializeField, Header("�U�����鎞��")] private float shakeTime;
    [SerializeField, Header("�U������傫��")] private float shakeMagnitude;

    private Player player;
    private float shakeCount;
    private Vector3 initPos;
    private int currentPlayerHP;
        
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        currentPlayerHP = player.GetHP();
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ShakeCheck();
        FollowPlayer();
    }

    private void ShakeCheck()
    {
        if (currentPlayerHP != player.GetHP() && currentPlayerHP >= player.GetHP())
        {
            currentPlayerHP = player.GetHP();
            shakeCount = 0.0f;
            StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        Vector3 initPos = transform.position;

        while (shakeCount < shakeTime)
        {
            //shakeMagnitude�Ŏw�肵���͈͕�x,y�̃J�������W��h�炷
            float x = initPos.x + Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = initPos.y + Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.position = new Vector3 (x, y, initPos.z);

            //�ЂƂO�̃t���[���̎��Ԃ����Z
            shakeCount += Time.deltaTime;

            yield return null;
        }

        transform.position = initPos;
    }

    private void FollowPlayer()
    {
        if (player == null) return;

        float x = player.transform.position.x;  
        x = Mathf.Clamp(x,initPos.x,Mathf.Infinity); //initPos���ŏ��l�AInfinity���ő�l�Ƃ���x�ɂ��̊Ԃ̒l����
        transform.position = new Vector3(x, transform.position.y,transform.position.z);
    }
}
