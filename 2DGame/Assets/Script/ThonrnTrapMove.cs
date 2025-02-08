using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThonrnTrapMove : MonoBehaviour
{
    [SerializeField]
    private GameObject moveTrapObj;
    [SerializeField]
    private bool ThonrnStop;
    [SerializeField]
    private float trapStop;
    private float timer;
    [SerializeField]
    Animator anim;
    private void Start()
    {
        ThonrnStop = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trap");
        if (!ThonrnStop)
        {
            if (collision.CompareTag("Player"))
            {
                //Debug.Log("Trapon");
                ThonrnStop = true;
                anim.SetTrigger("Animation");
                StartCoroutine(StopMoveTimer());
            }
        }
    }
    IEnumerator StopMoveTimer()
    {
        yield return new WaitForSeconds(3f);
        ThonrnStop = false;
    }
}
