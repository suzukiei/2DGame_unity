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

    [SerializeField, Header("フェード時間")] private float fadetime;
    [SerializeField, Header("フェード種類")] private Mode mode;
    [SerializeField]
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
        if (!bfade) return;
        this.GetComponent<Image>().enabled = true;
        switch (mode)
        {
            case Mode.FadeIn:
                FadeIn();
                break;
            case Mode.FadeOut:
                FadeOut();
                break;
        }

        float alpha = fadeCount / fadetime; //指定したFadeTimeでFadeCountを割っていく
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        if(!bfade && mode==Mode.FadeOut)
        this.GetComponent<Image>().enabled = false;
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

    public void FadeStart (UnityAction listener)
    {
        if (bfade) return;
        bfade = true;
        onFadeComplete.AddListener(listener);

    }
}
