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
           
            if (tag)
            {
                if (a.transform.position.y >= postionMax )
                    continue;
                a.GetComponent<Rigidbody2D>().MovePosition(new Vector2(a.transform.position.x, a.transform.position.y + (moveSpeed * Time.deltaTime)));

            }
            else
            {
                if ( a.transform.position.y <= postionMin)
                    continue;
                a.GetComponent<Rigidbody2D>().MovePosition(new Vector2(a.transform.position.x, a.transform.position.y - (moveSpeed * Time.deltaTime)));
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
