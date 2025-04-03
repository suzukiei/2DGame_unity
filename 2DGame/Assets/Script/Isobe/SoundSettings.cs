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
    [SerializeField] private TextMeshProUGUI MastertextMesh;
    private float _MastercurrentVolume = -1.0f;

    //BGM
    [SerializeField] private string _BGMvolumeParamName;
    [SerializeField] public float _BGMvolume;
    [SerializeField] private Slider _BGMslider;
    [SerializeField] private TextMeshProUGUI BGMtextMesh;
    private float _BGMcurrentVolume = -1.0f;
    //SE
    [SerializeField] private string _SEvolumeParamName;
    [SerializeField] public float _SEvolume;
    [SerializeField] private Slider _SEslider;
    [SerializeField] private TextMeshProUGUI SEtextMesh;
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
        //変更したデータをSettingManagerからデータを取得
        _MastercurrentVolume = _Mastervolume = _mastervolume;
        _audioMixer.SetFloat(_MastervolumeParamName, _Mastervolume);
        _Masterslider.value = (_mastervolume + 80) / 80;
        MastertextMesh.text = (Mathf.FloorToInt(_Mastervolume) +80).ToString();
        _BGMcurrentVolume = _BGMvolume = _bgmvolume;
        _audioMixer.SetFloat(_BGMvolumeParamName, _BGMvolume);
        _BGMslider.value = (_bgmvolume + 80) / 80;
        BGMtextMesh.text = (Mathf.FloorToInt(_BGMvolume) + 80).ToString();
        _SEcurrentVolume = _SEvolume = _sevolume;
        _audioMixer.SetFloat(_SEvolumeParamName, _SEvolume);
        _SEslider.value = (_sevolume + 80) / 80;
        SEtextMesh.text = (Mathf.FloorToInt(_SEvolume) + 80).ToString();
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
        BGMtextMesh.text = (Mathf.FloorToInt(_BGMvolume) + 80).ToString();
      
    }
    void SESetting()
    {
        _SEvolume = _SEslider.value * 80 - 80;
        if (Mathf.Approximately(_SEcurrentVolume, _SEvolume))
            return;
        // AudioMixer.SetFloat で Exposed Parameter を設定する
        _audioMixer.SetFloat(_SEvolumeParamName, _SEvolume);
        _SEcurrentVolume = _SEvolume;
      
        SEtextMesh.text = (Mathf.FloorToInt(_SEvolume) + 80).ToString();
    }
    void MasterSetting()
    {
        _Mastervolume = _Masterslider.value * 80 - 80;
        if (Mathf.Approximately(_MastercurrentVolume, _Mastervolume))
            return;
        // AudioMixer.SetFloat で Exposed Parameter を設定する
        _audioMixer.SetFloat(_MastervolumeParamName, _Mastervolume);
        _MastercurrentVolume = _Mastervolume;
        MastertextMesh.text = (Mathf.FloorToInt(_Mastervolume)+80).ToString();
        
      
    }
    public void onClick()
    {  //変更したデータをSettingManagerに送る
        settingManager.setSaveData();
    }

}
