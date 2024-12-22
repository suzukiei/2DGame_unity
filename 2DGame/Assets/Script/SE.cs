using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE : MonoBehaviour
{

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayingEnd();
    }

    private void PlayingEnd()
    {
        if (audioSource.isPlaying) return;
        Destroy(gameObject);
    }
}
