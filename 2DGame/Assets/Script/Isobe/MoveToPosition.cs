using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveToPosition : MonoBehaviour
{
    public Transform target; // 追従したいワールドオブジェクト
    public Transform uiObject; // World Space Canvas内のUIオブジェクト（Imageなど）
    [SerializeField]
    private Camera camera;
    public float moveTime = 1.0f; // 移動にかける時間（秒）
    private Vector3 startPosition;
    public Vector3 endPosition;
    private float elapsedTime = 0f;
    void Start()
    {
        // ワールド位置をスクリーン座標に変換
        startPosition = transform.position;
        var vector3 =target.position;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        // 直接ワールド座標で位置を合わせる
        //uiObject.position = new Vector3(target.position.x, 1, 0); // 例：ターゲットの上に表示
        //uiObject.LookAt(Camera.main.transform); // 常にカメラの方向を向かせる（任意）


        endPosition = RectTransformUtility.WorldToScreenPoint(camera, vector3);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / moveTime); // 0〜1の間での補間係数

        transform.position = Vector3.Lerp(startPosition, endPosition, t);

        if (t >= 1.0f)
        {
            enabled = false; // 移動完了後は更新を停止
        }
    }
}
