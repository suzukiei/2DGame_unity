using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBar : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 90f; // 1秒間に90度回転（調整可能）
    [SerializeField,Header("時計回りにする")] private bool clockwise = true; // 時計回りか反時計回りか

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 回転方向に応じて、正または負の回転を適用
        float direction = clockwise ? -1f : 1f;

        // 毎フレーム回転を適用
        transform.Rotate(0, 0, rotationSpeed * direction * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーにダメージを与えるなどの処理これからかく
        }
    }
}
