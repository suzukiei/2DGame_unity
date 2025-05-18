using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPView : MonoBehaviour
{
    [SerializeField]
    private SkullBoss skullBoss;
    [SerializeField]
    private Image image;
    private void Update()
    {
        float hp = (float)skullBoss.getHP() / (float)skullBoss.maxHealth;
        //Debug.Log(hp);
        image.fillAmount = hp;
    }
}

