using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xbox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 0"))
        {
            Debug.Log("button0");
        }
        if (Input.GetKeyDown("joystick button 1"))
        {
            Debug.Log("button1");
        }
        if (Input.GetKeyDown("joystick button 2"))
        {
            Debug.Log("button2");
        }
        if (Input.GetKeyDown("joystick button 3"))
        {
            Debug.Log("button3");
        }
        if (Input.GetKeyDown("joystick button 4"))
        {
            Debug.Log("button4");
        }
        if (Input.GetKeyDown("joystick button 5"))
        {
            Debug.Log("button5");
        }
        if (Input.GetKeyDown("joystick button 6"))
        {
            Debug.Log("button6");
        }
        if (Input.GetKeyDown("joystick button 7"))
        {
            Debug.Log("button7");
        }
        if (Input.GetKeyDown("joystick button 8"))
        {
            Debug.Log("button8");
        }
        if (Input.GetKeyDown("joystick button 9"))
        {
            Debug.Log("button9");
        }
    }
}
