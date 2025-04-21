using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveToPosition : MonoBehaviour
{
    public Vector3 target; // 追従したいワールドオブジェクト
    public float moveTime = 1.0f; // 移動にかける時間（秒）
    private Vector3 startPosition;
    public Vector3 endPosition;
    private float elapsedTime = 0f;
    void Start()
    {
        //開始位置を取得
        startPosition = transform.position;
        //到着位置を取得
        endPosition = target;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / moveTime); // 0〜1の間での補間係数
        endPosition = target;
        transform.position = Vector3.Lerp(startPosition, endPosition, t);

        if (t >= 1.0f)
        {
            Destroy(this.gameObject);
            enabled = false; // 移動完了後は更新を停止
        }
    }
}
