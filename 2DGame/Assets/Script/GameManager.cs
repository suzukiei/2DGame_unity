using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] GameObject Settings;
    [SerializeField] private bool CursorViewFlag;
    [SerializeField] public int select;
    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        select = 1;
        Debug.Log(select);
        // カーソル非表示
        Cursor.visible = false;
        CursorViewFlag = false;
        Settings.SetActive(CursorViewFlag);
    }

    // Update is called once per frame
    private void Update()
    {
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
}
