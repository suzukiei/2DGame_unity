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
        if (SceneManager.GetActiveScene().name=="Help"&&(Input.GetKeyDown("joystick button 0")||Input.GetKeyDown(KeyCode.Space)))
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
            PlayerPrefs.SetInt("CurrentStagePosition", 0);
            PlayerPrefs.Save();
        }
    }

    //public void OnSpaceClick(InputAction.CallbackContext contex)
    //{
    //    if(!contex.performed && bStart)
    //    {
    //        fade.FadeStart(ChangeHelp);
    //        bStart = false ;
    //    }
    //}

    public void ChangeHelp()
    {
        SceneManager.LoadScene("Help");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            fade.FadeStart(ChangeHelp);
            bStart = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            fade.FadeStart(ChangeHelp);
            bStart = false;
        }
    }


}
