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
    //�Ă΂ꏇ

    //Render�R���|�[�l���g(SpriteRenderer,MeshRenderer)�������Ă���I�u�W�F�N�g���J�����Ɏʂ��Ă���Ƃ��ɌĂ΂��
    //Scene�r���[�ɉf��Ȃ��Ȃ��ď��߂ČĂ΂�Ȃ��Ȃ�
    private void OnWillRenderObject()
    {
        //Main Camera�Ɏʂ��Ă��邩�H
        if (Camera.current.name == "Main Camera") // 1
        {
            mode = Mode.Render;
        }


    }

    private void Dead() 
    {
        Vector3 cameraMinPos = Camera.main.ScreenToWorldPoint(Vector3.zero); //�J�����̍��[�̈ʒu

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
