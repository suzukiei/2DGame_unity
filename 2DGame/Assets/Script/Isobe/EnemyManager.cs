using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
{
    [SerializeField] private List<GameObject> EnemyObj;
    [SerializeField] private int GameListCount;
    [SerializeField] private  bool enemySpownFlag;
    public bool EnemySpownFlag() { return enemySpownFlag; }
    private void Start()
    {
        getEnemyObjList();
        enemySpownFlag = true ;
    }
    //������G�l�~�[�����ׂĎ擾     
    private void getEnemyObjList()
    {
        GameObject[] getArrayObj = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject game in getArrayObj)
        {

            if (game.layer == 7 && game.gameObject.tag == "Enemy")
            {
                EnemyObj.Add(game);
                game.transform.parent = this.gameObject.transform;
                GameListCount++;
            }
        }
    }
    public void setEnemyObjListAdd(GameObject game)
    {
        EnemyObj.Add(game);
        game.transform.parent = this.gameObject.transform;
        GameListCount++;
    }
    public void DestroyEnemyObjList(GameObject game)
    {
        EnemyObj.Remove(game);
    }
    public void EnemyListClear()
    {
        foreach (var enemy in EnemyObj)
        {
            //.SetActive�ɕύX�����邩��
            Destroy(enemy);
        }
        EnemyObj.Clear();
    }
   

}
