using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpown : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spownTimer;
    [SerializeField] private float spowndistanceMin;
    [SerializeField] private float spowndistanceMax;
    [SerializeField]
    private GameObject player;
    private bool spownflag;
    private float timer;
    private void Start()
    {
        player = GameObject.Find("Player");
        spownflag = false;
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null||!EnemyManager.Instance.EnemySpownFlag())
            return;
        //�v���C���[���߂��ɂ��Ȃ������m�F����
        spownDistanceCheck();
        //���Ԃ��v�Z
        calculateTime();
        //�G�l�~�[�̐�������
        createEnemy();
    }
    private void spownDistanceCheck()
    {
        float enemyDistance = Mathf.Abs(player.transform.position.x - this.transform.position.x);
        //Debug.Log(enemyDistance);
        if (enemyDistance <= spowndistanceMin)//&& enemyDistance >= spowndistanceMax
            spownflag = false;
        else
            spownflag = true;
        //Debug.Log(spownflag);
    }
    private void calculateTime()
    {
        if (!spownflag) return;
        timer += Time.deltaTime;
    }
    private void createEnemy()
    {
        if (timer <= spownTimer) return;
        timer = 0;
        Instantiate(enemyPrefab,this.transform.position, Quaternion.identity);
    }
}
