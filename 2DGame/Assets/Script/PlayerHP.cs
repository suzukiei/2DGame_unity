using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{

    [SerializeField, Header("HPアイコン")] private GameObject PlayerIcon;

    private Player player;
    private int beforeHP;


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>(); //ヒエラルキーの中の<>で指定したコンポネントが入っているオブジェクトを探して取得する
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

        //Image配列 Imageコンポーネントをもっている子オブジェクトを探して配列で取得
        Image[] Icons = transform.GetComponentsInChildren<Image>();

        for(int i = 0; i < Icons.Length;i++)
        {
            //GetHPの値がi未満である場合は非表示(オブジェクトの非アクティブ)を返す
            Icons[i].gameObject.SetActive(i < player.GetHP());
        }
        beforeHP = player.GetHP();
    }
}
