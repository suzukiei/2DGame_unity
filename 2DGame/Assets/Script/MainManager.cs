using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{

    [SerializeField,Header("�Q�[���I�[�o�[")] private GameObject gameOverUI;
    [SerializeField,Header("�Q�[���N���A")] private GameObject gameClearUI;

    private GameObject Player;

    private bool bShowUI;

    // Start is called before the first frame update
    void Start()
    {
        //�v���C���[�^�̃Q�[���I�u�W�F�N�g����
        Player = FindAnyObjectByType<Player>().gameObject;
        bShowUI = false;
        FindObjectOfType<Fade>().FadeStart(MainStart);
        Player.GetComponent<Player>().enabled = false;
        foreach (EnemySpawner enemySpawner in FindObjectsOfType<EnemySpawner>())
        {
            enemySpawner.enabled = false;

        }
    }

    private void MainStart()
    {
        Player.GetComponent<Player>().enabled = true;

        foreach (EnemySpawner enemySpawner in FindObjectsOfType<EnemySpawner>())
        {
            enemySpawner.enabled = true;

        }
            
    }

    // Update is called once per frame
    void Update()
    {
        ShowGameOverUI();
    }

    private void ShowGameOverUI()
    {  
        if (Player != null) return;
        EnemyManager.Instance.EnemyListClear();
        gameOverUI.SetActive(true);
        bShowUI = true;
    }

    public void ShowGameClearUI()
    {
        EnemyManager.Instance.EnemyListClear();
        gameClearUI.SetActive(true);
        bShowUI = true;
    }

    public void OnRestart(InputAction.CallbackContext context)
    {

        if (!bShowUI || !context.performed) return;
        SceneManager.LoadScene("StageSelect");
        Debug.Log("Goal");
    }
}
