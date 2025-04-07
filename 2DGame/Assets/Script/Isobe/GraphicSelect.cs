using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum ScreenSelect
{
    FullscreenWindow,
    Windowed
}
public enum ScreenResolution
{
    FullHD,
    HD,
    
}
public class GraphicSelect : MonoBehaviour
{
    [SerializeField] private SettingManager settingManager;
    [SerializeField] public ScreenSelect screenSelect;//FPS
    [SerializeField] public ScreenResolution screenResolution;
    [SerializeField] public bool FPSSelect;
    [SerializeField] public int FPSValue;
    [SerializeField] public int Brightness;
    [SerializeField] TMP_Dropdown dropdownScreenSelect;
    [SerializeField] TMP_Dropdown dropdownScreenResolution;
    [SerializeField] TMP_InputField InputFieldFPSValue;
    public void StartGraphicSelect(int _screenSelect, int _screenResolution,bool _FPSSelect,int _FPSValue,int _Brightness)
    {
        //設定値
        screenSelect = (ScreenSelect)_screenSelect;//FPS
        screenResolution = (ScreenResolution)_screenResolution;
        FPSSelect= _FPSSelect;
        FPSValue= _FPSValue;
        Brightness = _Brightness;
      
        Application.targetFrameRate = _FPSValue; // 30fpsに設定
        SetScreenResolution(screenSelect, screenResolution, FPSValue);
        //画面
        dropdownScreenSelect.value= _screenSelect;
        dropdownScreenResolution.value = _screenResolution;
        //SettingManagerからデータをもたって来る
        InputFieldFPSValue.text = _FPSValue.ToString();
    }
    private void Start()
    {
        
    }
    private void FixedUpdate()
    {
        screenSelect = (ScreenSelect)dropdownScreenSelect.value;
        screenResolution = (ScreenResolution)dropdownScreenResolution.value;
        if(InputFieldFPSValue.text!="")
        FPSValue = int.Parse(InputFieldFPSValue.text);
    }
    private Vector2 gameScreenResolution(ScreenResolution _screenResolution)
    {
        switch (_screenResolution)
        {
            case ScreenResolution.FullHD:
                return new Vector2(1920, 1080);
            case ScreenResolution.HD:
                return new Vector2(1280, 720);
            default:
                return new Vector2(1920, 1080);
        }

    }
    private FullScreenMode gameScreenResolution(ScreenSelect _screenSelect)
    {
        switch (_screenSelect)
        {
            case ScreenSelect.FullscreenWindow:
                return FullScreenMode.FullScreenWindow;
            case ScreenSelect.Windowed:
                return FullScreenMode.Windowed;
            default:
                return FullScreenMode.Windowed;
        }


    }
    void SetScreenResolution(ScreenSelect _screenSelect, ScreenResolution _screenResolution, int _FPSValue = 60)
    {
        Vector2 getResolution = gameScreenResolution(_screenResolution);
        switch (_screenSelect)
        {
            case ScreenSelect.FullscreenWindow:
                Screen.SetResolution((int)getResolution.x, (int)getResolution.y, gameScreenResolution(_screenSelect), _FPSValue);
                break;
            case ScreenSelect.Windowed:
                Screen.SetResolution((int)getResolution.x, (int)getResolution.y, gameScreenResolution(_screenSelect), _FPSValue);
                break;
            default:
                break;
        }
        Application.targetFrameRate = FPSValue; // 30fpsに設定
    }
    public void  onClick()
    {
        SetScreenResolution(screenSelect, screenResolution, FPSValue);
        settingManager.setSaveData();
    }

}
