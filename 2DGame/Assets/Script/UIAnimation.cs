using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private GameObject upIndicator;
    [SerializeField] private GameObject downIndicator;
    [SerializeField] private GameObject rightIndicator;
    [SerializeField] private GameObject leftIndicator;

    [Header("点滅設定")]
    [SerializeField] private float blinkSpeed = 1.5f;  // 点滅の速さ
    [SerializeField] private float minAlpha = 0.3f;    // 最小透明度
    [SerializeField] private float maxAlpha = 1.0f;    // 最大透明度

    private Dictionary<GameObject, Coroutine> blinkCoroutines = new Dictionary<GameObject, Coroutine>();

    // 方向インジケーターの表示設定
    public void SetAvailableDirections(bool right, bool left, bool up, bool down)
    {
        UpdateIndicatorVisibility(rightIndicator, right);
        UpdateIndicatorVisibility(leftIndicator, left);
        UpdateIndicatorVisibility(upIndicator, up);
        UpdateIndicatorVisibility(downIndicator, down);
    }

    // 移動中かどうかに応じてUIを表示/非表示
    public void SetMovingState(bool isMoving)
    {

        if (!isMoving)
        {
            //Debug.Log("UI表示停止");
            // コルーチンも全て停止
            StopAllBlinkCoroutines();
            // 移動中は全て非表示
            if (upIndicator) upIndicator.SetActive(false);
            if (downIndicator) downIndicator.SetActive(false);
            if (leftIndicator) leftIndicator.SetActive(false);
            if (rightIndicator) rightIndicator.SetActive(false);

            
        }
    }

    // 個別のインジケーターの表示/非表示と点滅を設定
    private void UpdateIndicatorVisibility(GameObject indicator, bool show)
    {
        if (indicator == null) return;

        // まず表示/非表示を設定
        indicator.SetActive(show);

        // 既存の点滅コルーチンを停止
        if (blinkCoroutines.ContainsKey(indicator) && blinkCoroutines[indicator] != null)
        {
            StopCoroutine(blinkCoroutines[indicator]);
            blinkCoroutines[indicator] = null;
        }

        // 表示される場合は点滅を開始
        if (show)
        {
            SpriteRenderer renderer = indicator.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                blinkCoroutines[indicator] = StartCoroutine(BlinkSpriteIndicator(renderer));
            }
        }
    }

    // スプライト用の点滅エフェクトのコルーチン
    private IEnumerator BlinkSpriteIndicator(SpriteRenderer renderer)
    {
        float time = 0f;

        // 元の色を保存
        Color originalColor = renderer.color;

        //Debug.Log("点滅開始: " + renderer.gameObject.name);

        while (true)
        {
            // サイン波で滑らかに点滅
            time += Time.deltaTime * blinkSpeed;
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(time * Mathf.PI) + 1f) / 2f);

            // アルファ値のみを変更
            renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);


            //Debug.Log(renderer.gameObject.name + " Alpha: " + alpha);


            yield return null;
        }
    }

    // 全ての点滅コルーチンを停止
    private void StopAllBlinkCoroutines()
    {
        foreach (var coroutine in blinkCoroutines.Values)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        blinkCoroutines.Clear();
    }

    // スクリプトが無効化されたりシーンが変わる時に全てのコルーチンを停止
    private void OnDisable()
    {
        StopAllBlinkCoroutines();
    }
}
