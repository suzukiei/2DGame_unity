using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = FindObjectOfType<Player>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = (Player.transform.position - transform.position);
        transform.rotation = Quaternion.FromToRotation(Vector2.left,dir);
    }
}
