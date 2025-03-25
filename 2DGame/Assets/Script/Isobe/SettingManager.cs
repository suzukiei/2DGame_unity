using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private DataManager dataManager;
    [SerializeField] private SaveData saveData;
    [SerializeField] private SoundSettings soundSettings;
    [SerializeField] private GraphicSelect graphicSelect;
    private void Start()
    {
        saveData = dataManager.data;
        DataLoad(saveData);
    }
    private void FixedUpdate()
    {   
    }
    void DataLoad(SaveData _saveData)
    {
        soundSettings.StartvoiumeSettings(_saveData._Mastervolume, _saveData._BGMvolume, _saveData._SEvolume);
        graphicSelect.StartGraphicSelect(_saveData.screenSelect, _saveData.screenResolution, _saveData.FPSSelect, _saveData.FPSValue, _saveData.Brightness);
    }

    public void setSaveData()
    {
        saveData.screenSelect = (int)graphicSelect.screenSelect;
        saveData.screenResolution = (int)graphicSelect.screenResolution;
        saveData.FPSSelect = graphicSelect.FPSSelect;
        saveData.FPSValue = graphicSelect.FPSValue;
        saveData.Brightness = graphicSelect.Brightness;
        //サウンド関連
        //Master
        saveData._Mastervolume = soundSettings._Mastervolume;
        //BGM
        saveData._BGMvolume = soundSettings._BGMvolume;
        //SE
        saveData._SEvolume = soundSettings._SEvolume;
        dataManager.data = saveData;
        dataManager.SettingSave();
    }
}
