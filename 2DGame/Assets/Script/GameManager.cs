using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject Settings;
    [SerializeField] private bool CursorViewFlag;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
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
