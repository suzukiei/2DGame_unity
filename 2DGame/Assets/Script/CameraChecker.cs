using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChecker : MonoBehaviour
{

    private enum Mode
    {
        None,
        Render,
        RenderOut
    }

    private Mode mode;
    // Start is called before the first frame update
    void Start()
    {
        mode = Mode.None;
    }

    // Update is called once per frame
    void Update()
    {

        Dead();
        
    }
    //呼ばれ順

    //Renderコンポーネント(SpriteRenderer,MeshRenderer)が入っているオブジェクトがカメラに写っているときに呼ばれる
    //Sceneビューに映らなくなって初めて呼ばれなくなる
    private void OnWillRenderObject()
    {
        //Main Cameraに写っているか？
        if (Camera.current.name == "Main Camera") // 1
        {
            mode = Mode.Render;
        }


    }

    private void Dead() 
    {
        Vector3 cameraMinPos = Camera.main.ScreenToWorldPoint(Vector3.zero); //カメラの左端の位置

        if(mode == Mode.RenderOut && transform.position.x < cameraMinPos.x)// 2
        {
            Destroy(gameObject);
        }

        if(mode == Mode.Render) // 3
        {
            mode = Mode.RenderOut;
        }
    }
}
