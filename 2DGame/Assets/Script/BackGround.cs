using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackGround : MonoBehaviour
{
    //Rangeを使用することで上限と下限を決められる
    [SerializeField, Header("視覚"), Range(0, 1)] private float parallaxEffect;
    

    private GameObject cameraa;
    private float langth;
    private float startPosX;
    // Start is called before the first frame update
    void Start()
    {
       startPosX = transform.position.x;
        langth = GetComponent<SpriteRenderer>().bounds.size.x; //画像横幅のサイズを取得
        cameraa = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //一定時間で呼ばれるUpdate
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
