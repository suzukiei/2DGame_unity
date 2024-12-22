using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{

    [SerializeField, Header("HP�A�C�R��")] private GameObject PlayerIcon;

    private Player player;
    private int beforeHP;


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>(); //�q�G�����L�[�̒���<>�Ŏw�肵���R���|�l���g�������Ă���I�u�W�F�N�g��T���Ď擾����
        beforeHP = player.GetHP();
        CreateHPIcon();
    }

    private void CreateHPIcon()
    {
        for(int i = 0; i < player.GetHP(); i++)
        {
            GameObject playerHPObj = Instantiate(PlayerIcon);
            playerHPObj.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowHPIcon();
    }

    private void ShowHPIcon()
    {
        if (beforeHP == player.GetHP()) return;

        //Image�z�� Image�R���|�[�l���g�������Ă���q�I�u�W�F�N�g��T���Ĕz��Ŏ擾
        Image[] Icons = transform.GetComponentsInChildren<Image>();

        for(int i = 0; i < Icons.Length;i++)
        {
            //GetHP�̒l��i�����ł���ꍇ�͔�\��(�I�u�W�F�N�g�̔�A�N�e�B�u)��Ԃ�
            Icons[i].gameObject.SetActive(i < player.GetHP());
        }
        beforeHP = player.GetHP();
    }
}
