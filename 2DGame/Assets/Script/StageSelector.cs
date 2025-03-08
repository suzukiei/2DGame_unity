using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
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
    [SerializeField]
    private Vector2 maxmin =new Vector2(0,1);
    //ステージポイント管理
    private SelectSceneManager[] stageIndexs;
    private int currentIndex;

    //アニメ
    private Animator animator;
    private Fade fade;
    // private bool bStart;
    private bool isEnteringStage = false;  //フラグ

    //効果音
    [SerializeField] private AudioClip enterSound; 
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {

       
        //FindObjectsOfType<SelectSceneManager>()だけだと配列に入ってくる順番が不明瞭なため、
        //きちんとステージ番号順で管理する
        stageIndexs = FindObjectsOfType<SelectSceneManager>().OrderBy(obj => obj.StageIndex).ToArray();

        // NavMeshPath を初期化
        path = new NavMeshPath();

        animator = GetComponent<Animator>();

        fade = FindObjectOfType<Fade>();
        maxmin = new Vector2(0, 1);
        // bStart = false;

        //Sentakushi = GameObject.Find("Sentakushi_Dialogue").GetComponent<NPCConversation>();

        // 保存された位置がある場合 | デフォ値は0とする
        currentIndex = PlayerPrefs.GetInt("CurrentStagePosition", 0);

        audioSource = GetComponent<AudioSource>();

        if (stageIndexs.Length > 0)
        {
            SetKyori(stageIndexs[currentIndex].transform.position);
        }

        //最初にフェードインが必要なラムダ式 これしないと画面真っ暗なまま始まる
        fade.FadeStart(() => {     
        });

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentIndex);
        //Debug.Log(stageIndexs[currentIndex].StageIndex);
        //Debug.Log(stageIndexs[currentIndex].transform.position);
        //ダイアログが出ているときはキー操作を受け付けない
        if (!isEnteringStage)
        {

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Horizontal") >=1f)
            {
                //Indexは0から始まるため、範囲を超えないように
                if (stageIndexs.Length - 1 > currentIndex)
                {
                    if (currentIndex < maxmin.y)
                    {
                        currentIndex++;
                        SetKyori(stageIndexs[currentIndex].transform.position);
                        maxmin = new Vector2(currentIndex - 1, currentIndex);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Horizontal") <= -1f)
            {
                if (currentIndex > 0)
                {

                    if (currentIndex > maxmin.x)
                    {
                        currentIndex--;
                        SetKyori(stageIndexs[currentIndex].transform.position);
                        maxmin = new Vector2(currentIndex, currentIndex + 1);
                    }
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
            if (maxmin.y - maxmin.x == 1)
            {
                maxmin = new Vector2(currentIndex-1, currentIndex + 1);
            }
            if (!isEnteringStage && animator != null)  // 遷移中でない場合のみアニメーション更新
            {
                animator.SetBool("Walk", false);
            }

            if (currentIndex > 0 && pathIndex > 0)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown("joystick button 0"))
                {
                    Debug.Log("エンターキーが押されました");

                    StartCoroutine(EnterStageAnimation());

                }
            }
            return;
        }

        Vector2 currentPos = transform.position;
        Vector2 targetPos = path.corners[pathIndex];

        #region アニメーション処理
        // 移動中かどうかを判定       
        // アニメーターがアタッチされている場合、Walk パラメータを設定
        if (!isEnteringStage)  // 遷移中でない場合のみアニメーション更新
        {
            bool isMoving = Vector2.Distance(currentPos, targetPos) > stoppingDistance;
            if (animator != null)
            {
                animator.SetBool("Walk", isMoving);
            }
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

    //private void ShowDialogue()
    //{
    //    ConversationManager.Instance.StartConversation(Sentakushi);
    //}

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


    private IEnumerator EnterStageAnimation()
    {
        isEnteringStage = true;

        if (animator != null)
        {
            animator.SetBool("Walk", true);
        }

        if (audioSource != null && enterSound != null)
        {
            audioSource.clip = enterSound;
            audioSource.Play();
        }

        float duration = 4.0f;
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 startPosition = transform.position;

        Camera mainCamera = Camera.main;
        Vector3 cameraStartPosition = mainCamera.transform.position;
        float startSize = mainCamera.orthographicSize; // 開始時のカメラサイズ
        float targetSize = startSize * 0.5f; // ズームイン後のサイズ（数値は調整可能）

        // 木の位置に向かって移動
        Vector3 woodDirection = (stageIndexs[currentIndex].transform.position - startPosition).normalized;
        Vector3 targetPosition = startPosition + (woodDirection * 2f);

        // カメラの目標位置（木の位置の方向へ）
        Vector3 cameraTargetPosition = Vector3.Lerp(cameraStartPosition,
            new Vector3(stageIndexs[currentIndex].transform.position.x,
                       stageIndexs[currentIndex].transform.position.y,
                       cameraStartPosition.z), 0.3f); // 0.3fは木の方向への移動度合い

        fade.FadeStart(() => {
            SceneManager.LoadScene(stageIndexs[currentIndex].StageName);
        });

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);

            // キャラクターのアニメーション
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, smoothT);
            transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);

            // カメラのズームインと移動
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, smoothT);
            mainCamera.transform.position = Vector3.Lerp(cameraStartPosition, cameraTargetPosition, smoothT);

            yield return null;
        }

        isEnteringStage = false;
    }


}
