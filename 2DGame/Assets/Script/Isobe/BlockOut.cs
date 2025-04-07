using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
//using UnityEngine.U2D;
public class BlockOut : MonoBehaviour
{
    [SerializeField] private Light2D light2d;
    static bool light = false;

    //[SerializeField] private Light2DBase light2bate;

    private void Update()
    {
        if (light)
        {
            if (light2d.intensity >= 0.2f)
                light2d.intensity -= 0.001f;
        }
        if (!light)
        {
            if (light2d.intensity <= 0.5f)
                light2d.intensity += 0.001f;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            light = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            light = false;
        }
    }
}
