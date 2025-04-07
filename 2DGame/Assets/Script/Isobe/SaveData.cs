using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //グラフィック関連
    [SerializeField] public int screenSelect;//FPS
    [SerializeField] public int screenResolution;
    [SerializeField] public bool FPSSelect;
    [SerializeField] public int FPSValue;
    [SerializeField] public int Brightness;
    //サウンド関連
    //Master
    [SerializeField] public float _Mastervolume;
    //BGM
    [SerializeField] public float _BGMvolume;
    //SE
    [SerializeField] public float _SEvolume;

}

