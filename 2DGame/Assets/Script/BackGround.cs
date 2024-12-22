using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackGround : MonoBehaviour
{
    //Range���g�p���邱�Ƃŏ���Ɖ��������߂���
    [SerializeField, Header("���o"), Range(0, 1)] private float parallaxEffect;
    

    private GameObject cameraa;
    private float langth;
    private float startPosX;
    // Start is called before the first frame update
    void Start()
    {
       startPosX = transform.position.x;
        langth = GetComponent<SpriteRenderer>().bounds.size.x; //�摜�����̃T�C�Y���擾
        cameraa = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //��莞�ԂŌĂ΂��Update
    private void FixedUpdate()
    {
        Parallax();
    }

    private void Parallax()
    {
        float temp = cameraa.transform.position.x * (1 - parallaxEffect);
        float dist = cameraa.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startPosX + dist, transform.position.y, transform.position.z);

        if(temp > startPosX + langth)
        {
            startPosX += langth;
        }else if (temp < startPosX - langth)
        {
            startPosX -= langth;
        }

    }
}
