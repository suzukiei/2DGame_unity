using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using DialogueEditor;
using Unity.VisualScripting;

public class StageSelector : MonoBehaviour
{
    // 経路計算用
     [Header("Steering")]
    public float speed = 1.0f;
    public float stoppingDistance = 0.1f; // 目的地に到達とみなす距離

    private Vector2 traceArea = Vector2.zero;
    private NavMeshPath path;
    private int pathIndex;

    //ステージポイント管理
    private SelectSceneManager[] stageIndexs;
    private int currentIndex;

    //アニメ
    private Animator animator;
    private Fade fade;
    // private bool bStart;

    //メッセージダイアログ
    public NPCConversation Sentakushi;

    // Start is called before the first frame update
    void Start()
    {

        //stageIndexs = FindObjectsOfType<SelectSceneManager>();

        //FindObjectsOfType<SelectSceneManager>()だけだと配列に入ってくる順番が不明瞭なため、
        //きちんとステージ番号順で管理する
        stageIndexs = FindObjectsOfType<SelectSceneManager>().OrderBy(obj => obj.StageIndex).ToArray();

        // NavMeshPath を初期化
        path = new NavMeshPath();

        animator = GetComponent<Animator>();

        fade = FindObjectOfType<Fade>();

        // bStart = false;

        Sentakushi = GameObject.Find("Sentakushi_Dialogue").GetComponent<NPCConversation>();
        
        // 保存された位置がある場合 | デフォ値は0とする
        currentIndex = PlayerPrefs.GetInt("CurrentStagePosition", 0);

        if (stageIndexs.Length > 0)
        {
            SetKyori(stageIndexs[currentIndex].transform.position);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentIndex);
        //Debug.Log(stageIndexs[currentIndex].StageIndex);
        //Debug.Log(stageIndexs[currentIndex].transform.position);

        //ダイアログが出ているときはキー操作を受け付けない
        if (!ConversationManager.Instance.IsConversationActive)
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown("joystick button 1"))
            {

                //Indexは0から始まるため、範囲を超えないように
                if (stageIndexs.Length - 1 > currentIndex)
                {
                    currentIndex++;
                    SetKyori(stageIndexs[currentIndex].transform.position);
                }
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown("joystick button 2"))
            {
                if (currentIndex > 0)
                {
                    currentIndex--;
                    SetKyori(stageIndexs[currentIndex].transform.position);

                }
            }
            MoveToStagePoint();
        }

    }

    private void MoveToStagePoint()
    {
        // プレイヤーの位置をステージポイントの位置に移動
        //transform.position = stageIndexs[Index].transform.position;

        //経路が存在しないか、到達済みの場合
        if (path.corners.Length == 0 || pathIndex >= path.corners.Length)
        {
            // 移動していない時はアニメーションを停止
            if (animator != null) animator.SetBool("Walk", false);

            if (currentIndex > 0 && pathIndex > 0)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown("joystick button 0"))
                {
                    ShowDialogue();                  
                    //fade.FadeStart(ChangeScene);
                }
            }
            return;
        }

        Vector2 currentPos = transform.position;
        Vector2 targetPos = path.corners[pathIndex];

        #region アニメーション処理
        // 移動中かどうかを判定
        bool isMoving = Vector2.Distance(currentPos, targetPos) > stoppingDistance;

        // アニメーターがアタッチされている場合、Walk パラメータを設定
        if (animator != null)
        {
            animator.SetBool("Walk", isMoving);
        }
        #endregion

        #region 判定処理関係
        // 現在位置が NavMesh の範囲内かチェック
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(currentPos, out hit, 0.1f, NavMesh.AllAreas))
        {
            // NavMesh の範囲外の場合、最も近い有効な位置に戻す
            if (NavMesh.SamplePosition(currentPos, out hit, 1.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                currentPos = hit.position;
            }
        }

        // タイルの中心にスナップする前に、目標位置が NavMesh 上にあるか確認
        if (NavMesh.SamplePosition(targetPos, out hit, 0.1f, NavMesh.AllAreas))
        {
            targetPos = hit.position;
        }

        // その後でタイルの中心にスナップ
        targetPos = TileCenterKeisan(targetPos);

        float distanceToTarget = Vector2.Distance(currentPos, targetPos);
        if (distanceToTarget <= stoppingDistance)
        {
            pathIndex++;
            return;
        }

        // 一度に移動する距離を制限
        float maxMoveDistance = speed * Time.deltaTime;
        Vector2 moveDirection = (targetPos - currentPos).normalized;
        Vector2 newPosition = currentPos + moveDirection * Mathf.Min(maxMoveDistance, distanceToTarget);

        // 移動前に新しい位置が NavMesh 上にあるか確認
        if (NavMesh.SamplePosition(newPosition, out hit, 0.1f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        #endregion
    }

    private void KyoriKeisan(Vector2 current, Vector2 target)
    {
        // NavMesh に基づいて経路を求める
        path.ClearCorners();
        pathIndex = 0;

        if (NavMesh.CalculatePath(current, target, NavMesh.AllAreas, path))
        {
            Debug.Log("経路計算成功");
        }
        else
        {
            Debug.LogError("経路計算失敗");
        }
    }

    public void SetKyori(Vector2 target)
    {
        traceArea = target;
        KyoriKeisan(transform.position, target);
    }

    //タイルの中心を求める
    private Vector2 TileCenterKeisan(Vector2 position)
    {
        // タイルサイズを取得 (例: タイルの1マスが1x1ユニットの場合)
        float tileSize = 1f;

        // 座標をタイルの中心にスナップ
        float snappedX = Mathf.Floor(position.x / tileSize) * tileSize + (tileSize / 2);
        float snappedY = Mathf.Floor(position.y / tileSize) * tileSize + (tileSize / 2);

        return new Vector2(snappedX, snappedY);
    }


    private void SavePosition()
    {
        PlayerPrefs.SetInt("CurrentStagePosition", currentIndex);
        PlayerPrefs.Save();
    }

    private void ShowDialogue()
    {
        ConversationManager.Instance.StartConversation(Sentakushi);
    }

    public void ChangeScene()
    {
        SavePosition();
        SceneManager.LoadScene(stageIndexs[currentIndex].StageName);
    }


    //private void KyoriKeisan(int Index)
    //{
    //    // NavMeshPath を再計算
    //    path.ClearCorners();
    //    pathIndex = 0;

    //    Vector3 targetPosition = stageIndexs[Index].transform.position;

    //    if (NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
    //    {
    //        Debug.Log("経路計算成功！");
    //    }
    //    else
    //    {
    //        Debug.LogError("経路計算失敗！");
    //    }
    //}






}
