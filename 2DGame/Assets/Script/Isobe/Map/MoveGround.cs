using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGround : MonoBehaviour
{
    [SerializeField] private float postionMax;
    [SerializeField] private float postionMin;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool tag;
    [SerializeField] List<GameObject> groundList;
    private void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {
        foreach (var a in groundList)
        {
            a.GetComponent<BoxCollider2D>().isTrigger = false;
            if (tag)
            {
                if (a.transform.position.y >= postionMax )
                    continue;
                a.GetComponent<Rigidbody2D>().MovePosition(new Vector2(a.transform.position.x, a.transform.position.y + (moveSpeed * Time.deltaTime)));

                Vector3 cameraMinPos = Camera.main.ScreenToWorldPoint(Vector3.zero); //カメラの左端の位置
                Vector3 cameraMaxPos = Camera.main.ScreenToWorldPoint(new Vector3(1920,1080,0)); //カメラの左端の位置
                Debug.Log(cameraMaxPos);
                if (a.transform.position.y < cameraMinPos.y||a.transform.position.y > cameraMaxPos.y)//
                    a.GetComponent<BoxCollider2D>().isTrigger = true;
            }
            else
            {
                if ( a.transform.position.y <= postionMin)
                    continue;
                a.GetComponent<Rigidbody2D>().MovePosition(new Vector2(a.transform.position.x, a.transform.position.y - (moveSpeed * Time.deltaTime)));

                Vector3 cameraMinPos = Camera.main.ScreenToWorldPoint(Vector3.zero); //カメラの左端の位置
                Debug.Log(cameraMinPos);
                if (a.transform.position.y < cameraMinPos.y)// 2
                    a.GetComponent<BoxCollider2D>().isTrigger = true;
            }
    
        }
        foreach (var a in groundList)
        {
            if (tag)
            {
                if (a.transform.position.y >= postionMax)
                {
                    a.transform.position = new Vector2(a.transform.position.x, postionMin);
                    //debug.Log("tag");
                }
                    
            }
            else
            {
                if (a.transform.position.y <= postionMin)
                    a.transform.position = new Vector2(a.transform.position.x, postionMax);
            }
        }
    }
}
