using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class Fade : MonoBehaviour
{

    enum Mode
    {
        FadeIn,
        FadeOut
    }

    [SerializeField, Header("�t�F�[�h����")] private float fadetime;
    [SerializeField, Header("�t�F�[�h���")] private Mode mode;

    private bool bfade;
    private float fadeCount;
    private Image image;
    private UnityEvent onFadeComplete = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        switch(mode)
        {
            case Mode.FadeIn:
                fadeCount = fadetime;
                break;
            case Mode.FadeOut:
                fadeCount = 0;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Fade_();
    }

    private void Fade_()
    {

        if (!bfade)
        {
            Debug.Log("bfade��false�̂��߁A�t�F�[�h�͎��s����܂���");
            return;
        }

        Debug.Log($"Fade�������s��. Mode: {mode}, fadeCount: {fadeCount}");

        switch(mode)
        {
            case Mode.FadeIn:
                FadeIn();
                break;
            case Mode.FadeOut:
                FadeOut();
                break;
        }

        float alpha = fadeCount / fadetime; //�w�肵��FadeTime��FadeCount�������Ă���
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    private void FadeIn()
    {
        fadeCount -= Time.deltaTime;

        if(fadeCount <= 0)
        {
            mode = Mode.FadeOut;
            bfade = false;
            onFadeComplete.Invoke();
        }
    }

    private void FadeOut()
    {
        fadeCount += Time.deltaTime;
        if(fadeCount >= fadetime)
        {
            mode = Mode.FadeIn;
            bfade = false;
            onFadeComplete.Invoke();
        }
    }

    public void FadeStart(UnityAction listener)
    {
        Debug.Log($"FadeStart called. Current bfade: {bfade}");
        if (bfade) return;
        bfade = true;
        Debug.Log("�t�F�[�h���J�n���܂�");
        onFadeComplete.AddListener(listener);
    }
}
