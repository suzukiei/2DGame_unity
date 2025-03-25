using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private SettingManager settingManager;
    [SerializeField] private AudioMixer _audioMixer;
    //Master
    [SerializeField] private string _MastervolumeParamName;
    [SerializeField] public float _Mastervolume;
    [SerializeField] private Slider _Masterslider;
    [SerializeField] private TextMeshProUGUI textMesh;
    private float _MastercurrentVolume = -1.0f;

    //BGM
    [SerializeField] private string _BGMvolumeParamName;
    [SerializeField] public float _BGMvolume;
    [SerializeField] private Slider _BGMslider;
    private float _BGMcurrentVolume = -1.0f;
    //SE
    [SerializeField] private string _SEvolumeParamName;
    [SerializeField] public float _SEvolume;
    [SerializeField] private Slider _SEslider;
    private float _SEcurrentVolume = -1.0f;
    private void Start()
    { //変更したデータをSettingManagerからデータを取得
      // StartvoiumeSettings(1, 0.5f, 0.5f);
    }
    private void Update()
    {
        //Settingフラグを入れる
        BGMSetting();
        SESetting();
        MasterSetting();
    }
    public void StartvoiumeSettings(float _mastervolume, float _bgmvolume, float _sevolume)
    {
        _MastercurrentVolume = _Mastervolume = _mastervolume;
        _audioMixer.SetFloat(_MastervolumeParamName, _Mastervolume);
        _Masterslider.value = (_mastervolume + 80) / 80;
        textMesh.text = Mathf.FloorToInt(_Mastervolume).ToString() ;
        _BGMcurrentVolume = _BGMvolume = _bgmvolume;
        _audioMixer.SetFloat(_BGMvolumeParamName, _BGMvolume);
        _BGMslider.value = (_bgmvolume + 80) / 80;
        _SEcurrentVolume = _SEvolume = _sevolume;
        _audioMixer.SetFloat(_SEvolumeParamName, _SEvolume);
        _SEslider.value = (_sevolume + 80) / 80;
        Debug.Log(_MastercurrentVolume.ToString() + _Mastervolume.ToString());
    }
    void BGMSetting()
    {
        _BGMvolume = _BGMslider.value * 80 - 80;
        if (Mathf.Approximately(_BGMcurrentVolume, _BGMvolume))
            return;
        // AudioMixer.SetFloat で Exposed Parameter を設定する
        _audioMixer.SetFloat(_BGMvolumeParamName, _BGMvolume);
        _BGMcurrentVolume = _BGMvolume;
       
        //変更したデータをSettingManagerに送る
    }
    void SESetting()
    {
        _SEvolume = _SEslider.value * 80 - 80;
        if (Mathf.Approximately(_SEcurrentVolume, _SEvolume))
            return;
        // AudioMixer.SetFloat で Exposed Parameter を設定する
        _audioMixer.SetFloat(_SEvolumeParamName, _SEvolume);
        _SEcurrentVolume = _SEvolume;
        //変更したデータをSettingManagerに送る
    }
    void MasterSetting()
    {
        _Mastervolume = _Masterslider.value * 80 - 80;
        if (Mathf.Approximately(_MastercurrentVolume, _Mastervolume))
            return;
        // AudioMixer.SetFloat で Exposed Parameter を設定する
        _audioMixer.SetFloat(_MastervolumeParamName, _Mastervolume);
        _MastercurrentVolume = _Mastervolume;
        textMesh.text = Mathf.FloorToInt(_Mastervolume).ToString();
        //変更したデータをSettingManagerに送る
    }
    public void onClick()
    {
        settingManager.setSaveData();
    }

}
