using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneChange : MonoBehaviour
{
    [SerializeField]
    private string SceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnSpaceClick(InputAction.CallbackContext contex)
    {
        SceneManager.LoadScene("StageSelect");
    }
    
    public void ChangeHelp()
    {
        SceneManager.LoadScene("StageSelect");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
