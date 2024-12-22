using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    [SerializeField, Header("振動する時間")] private float shakeTime;
    [SerializeField, Header("振動する大きさ")] private float shakeMagnitude;

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
        if (currentPlayerHP != player.GetHP())
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
            //shakeMagnitudeで指定した範囲分x,yのカメラ座標を揺らす
            float x = initPos.x + Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = initPos.y + Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.position = new Vector3 (x, y, initPos.z);

            //ひとつ前のフレームの時間を加算
            shakeCount += Time.deltaTime;

            yield return null;
        }

        transform.position = initPos;
    }

    private void FollowPlayer()
    {
        if (player == null) return;

        float x = player.transform.position.x;  
        x = Mathf.Clamp(x,initPos.x,Mathf.Infinity); //initPosを最小値、Infinityを最大値としてxにその間の値を代入
        transform.position = new Vector3(x, transform.position.y,transform.position.z);
    }
}
