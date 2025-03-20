using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private GameObject upIndicator;
    [SerializeField] private GameObject downIndicator;
    [SerializeField] private GameObject rightIndicator;
    [SerializeField] private GameObject leftIndicator;

    [Header("�_�Őݒ�")]
    [SerializeField] private float blinkSpeed = 1.5f;  // �_�ł̑���
    [SerializeField] private float minAlpha = 0.3f;    // �ŏ������x
    [SerializeField] private float maxAlpha = 1.0f;    // �ő哧���x

    private Dictionary<GameObject, Coroutine> blinkCoroutines = new Dictionary<GameObject, Coroutine>();

    // �����C���W�P�[�^�[�̕\���ݒ�
    public void SetAvailableDirections(bool right, bool left, bool up, bool down)
    {
        UpdateIndicatorVisibility(rightIndicator, right);
        UpdateIndicatorVisibility(leftIndicator, left);
        UpdateIndicatorVisibility(upIndicator, up);
        UpdateIndicatorVisibility(downIndicator, down);
    }

    // �ړ������ǂ����ɉ�����UI��\��/��\��
    public void SetMovingState(bool isMoving)
    {

        if (!isMoving)
        {
            //Debug.Log("UI�\����~");
            // �R���[�`�����S�Ē�~
            StopAllBlinkCoroutines();
            // �ړ����͑S�Ĕ�\��
            if (upIndicator) upIndicator.SetActive(false);
            if (downIndicator) downIndicator.SetActive(false);
            if (leftIndicator) leftIndicator.SetActive(false);
            if (rightIndicator) rightIndicator.SetActive(false);

            
        }
    }

    // �ʂ̃C���W�P�[�^�[�̕\��/��\���Ɠ_�ł�ݒ�
    private void UpdateIndicatorVisibility(GameObject indicator, bool show)
    {
        if (indicator == null) return;

        // �܂��\��/��\����ݒ�
        indicator.SetActive(show);

        // �����̓_�ŃR���[�`�����~
        if (blinkCoroutines.ContainsKey(indicator) && blinkCoroutines[indicator] != null)
        {
            StopCoroutine(blinkCoroutines[indicator]);
            blinkCoroutines[indicator] = null;
        }

        // �\�������ꍇ�͓_�ł��J�n
        if (show)
        {
            SpriteRenderer renderer = indicator.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                blinkCoroutines[indicator] = StartCoroutine(BlinkSpriteIndicator(renderer));
            }
        }
    }

    // �X�v���C�g�p�̓_�ŃG�t�F�N�g�̃R���[�`��
    private IEnumerator BlinkSpriteIndicator(SpriteRenderer renderer)
    {
        float time = 0f;

        // ���̐F��ۑ�
        Color originalColor = renderer.color;

        //Debug.Log("�_�ŊJ�n: " + renderer.gameObject.name);

        while (true)
        {
            // �T�C���g�Ŋ��炩�ɓ_��
            time += Time.deltaTime * blinkSpeed;
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(time * Mathf.PI) + 1f) / 2f);

            // �A���t�@�l�݂̂�ύX
            renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);


            //Debug.Log(renderer.gameObject.name + " Alpha: " + alpha);


            yield return null;
        }
    }

    // �S�Ă̓_�ŃR���[�`�����~
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

    // �X�N���v�g�����������ꂽ��V�[�����ς�鎞�ɑS�ẴR���[�`�����~
    private void OnDisable()
    {
        StopAllBlinkCoroutines();
    }
}
