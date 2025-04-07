using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //�O���t�B�b�N�֘A
    [SerializeField] public int screenSelect;//FPS
    [SerializeField] public int screenResolution;
    [SerializeField] public bool FPSSelect;
    [SerializeField] public int FPSValue;
    [SerializeField] public int Brightness;
    //�T�E���h�֘A
    //Master
    [SerializeField] public float _Mastervolume;
    //BGM
    [SerializeField] public float _BGMvolume;
    //SE
    [SerializeField] public float _SEvolume;

}

