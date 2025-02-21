using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class TitleManager : MonoBehaviour
{
    private bool bStart;
    private Fade fade;

    // Start is called before the first frame update
    void Start()
    {
        bStart = false;
        fade = FindObjectOfType<Fade>();
        fade.FadeStart(TitleStart);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name=="Help"&&Input.GetKeyDown("joystick button 0"))
        {
            ChangeScenSpaceClicker();
        }
    }

    private void TitleStart()
    {
        bStart = true;
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("StageSelect");
    }
    public void ChangeScenSpaceClicker()
    {
        if (bStart)
        {
            fade.FadeStart(ChangeScene);
            bStart = false;
        }
    }

    public void OnSpaceClick(InputAction.CallbackContext contex)
    {
        if(!contex.performed && bStart)
        {
            fade.FadeStart(ChangeHelp);
            bStart = false ;
        }
    }

    public void ChangeHelp()
    {
        SceneManager.LoadScene("Help");
    }

    

}
