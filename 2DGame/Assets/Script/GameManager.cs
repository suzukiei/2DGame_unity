using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] GameObject Settings;
    [SerializeField] private bool CursorViewFlag;
    [SerializeField] public int select;
    [SerializeField] public float gameClearTime;
    [SerializeField] public int PlayerID;
    private float gametimer;

    public void Start()
    {
        //PlayerPrefs.SetInt("PlayerID", 0);
        PlayerPrefs.SetInt("CurrentStagePosition", 0);
        PlayerID = PlayerPrefs.GetInt("PlayerID", 0) + 1;
        PlayerPrefs.SetInt("PlayerID", PlayerID); 
        DontDestroyOnLoad(this.gameObject);
        select = 1;
        Debug.Log(select);
        // カーソル非表示
        Cursor.visible = false;
        CursorViewFlag = false;
        Settings.SetActive(CursorViewFlag);
        gameClearTime=0;
        gametimer = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        gametimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log(CursorViewFlag);
            //Cursor.visible = Cursor.visible ? false : true;
            if (CursorViewFlag)
            {
                CursorViewFlag = !CursorViewFlag;
                Cursor.visible = CursorViewFlag;
                //Time.timeScale = 1;
                Debug.Log(CursorViewFlag);
                Settings.SetActive(CursorViewFlag);
            }
            else
            {
                CursorViewFlag = !CursorViewFlag;
                Cursor.visible = CursorViewFlag;
                //Time.timeScale = 0;
                Debug.Log(CursorViewFlag);
                Settings.SetActive(CursorViewFlag);
            }
           
            
        }
    }
    public void setGameTimer()
    {
        gameClearTime = float.Parse(gametimer.ToString("f2"));
    }
}
