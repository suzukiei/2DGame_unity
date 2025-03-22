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
    private bool isEnteringStage = false;  //ステージ遷移中か判定

    //効果音
    [SerializeField] private AudioClip enterSound; 
    private AudioSource audioSource;

    //UI用
    // UIコントローラーの参照を追加
    [SerializeField] private UIAnimation UIAnim;

    [Header("分岐点制御")]
    public CrossroadPoint currentCrossroad;
    private bool isAtCrossroad = false;
    private bool isJustCrossroad = false; //丁度到着してから立てるフラグ

    // スティックの入力状態を記録する静的な変数
    static bool wasStickDown = false;
    static bool wasStickUp = false;
    static bool wasStickLeft = false;
    static bool wasStickRight = false;
    private float inputDelay = 0.5f; // 入力の遅延時間（秒）
    private float lastInputTime = 0f; // 最後に入力が処理された時間
    private bool isUpdatedDirection = false;



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


            stageIndexs = FindObjectsOfType<SelectSceneManager>().OrderBy(obj => obj.StageIndex).ToArray();
            if (stageIndexs.Length > 0)
            {
                SetKyori(stageIndexs[currentIndex].transform.position);
                UpdateDirectionIndicators();
            }
        

        //最初にフェードインが必要なラムダ式 これしないと画面真っ暗なまま始まる
        fade.FadeStart(() => {     
        });

    }

    // Update is called once per frame
    void Update()
    {
       Debug.Log(currentIndex);
        //Debug.Log(stageIndexs[currentIndex].StageIndex);
        //Debug.Log(stageIndexs[currentIndex].transform.position);
        //ダイアログが出ているときはキー操作を受け付けない
        if (!isEnteringStage)
        {
            if (isAtCrossroad)
            {
                //Debug.Log("分岐点にいます。");
                // 分岐点での操作処理
                HandleCrossroadInput();
            }
            else
            {
                // 通常のステージ選択の操作処理
                HandleStageSelectionInput();
            }

            MoveToStagePoint();
        }      
           
        
    }

    // 分岐点での入力処理
    private void HandleCrossroadInput()
    {

        if (isJustCrossroad)
        {
            // 各方向キー入力の処理
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Vertical") >= 0.5f)
            {
                if (currentCrossroad.HasUp())
                {
                    MoveInDirection("up");
                }
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Vertical") <= -0.5f)
            {
                if (currentCrossroad.HasDown())
                {
                    MoveInDirection("down");
                }
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") <= -0.5f)
            {
                if (currentCrossroad.HasLeft())
                {
                Debug.Log("LEFT推しました");
                    MoveInDirection("left");
                }
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxis("Horizontal") >= 0.5f)
            {
                if (currentCrossroad.HasRight())
                {
                    MoveInDirection("right");
                }
            }
        }
    }

    // 方向に応じた移動処理
    private void MoveInDirection(string direction)
    {
        Direction targetDirection = null;

        if(isJustCrossroad) isJustCrossroad = !isJustCrossroad; 


        // 指定された方向のDirectionを取得
        switch (direction)
        {
            case "up": targetDirection = currentCrossroad.up; break;
            case "down": targetDirection = currentCrossroad.down; break;
            case "left": targetDirection = currentCrossroad.left; break;
            case "right": targetDirection = currentCrossroad.right; break;
        }

        if (targetDirection != null)
        {
            if (targetDirection.type == Direction.DirectionType.Stage)
            {
                // ステージポイントへ移動
                isAtCrossroad = false;

                // ステージインデックスを特定
                SelectSceneManager targetStage = targetDirection.stageSelector;
                for (int i = 0; i < stageIndexs.Length; i++)
                {
                    if (stageIndexs[i] == targetStage)
                    {
                        currentIndex = i;
                        break;
                    }
                }

                SetKyori(targetStage.transform.position);
            }
            else if (targetDirection.type == Direction.DirectionType.Crossroad)
            {
                // 別の分岐点へ移動
                currentCrossroad = targetDirection.crossRoad;
                SetKyori(currentCrossroad.transform.position);

                
            }

            
        }
    }

    private void HandleStageSelectionInput()
    {
        #region 旧方式操作
        //// 入力の遅延処理
        //if (Time.time < lastInputTime + inputDelay)
        //{
        //    return; // 一定時間は入力を受け付けない
        //}

        //// アナログスティックの入力をチェック
        //float horizontalInput = Input.GetAxis("Horizontal");
        //float verticalInput = Input.GetAxis("Vertical");

        //// キーボード入力またはスティック入力を判定
        //bool upInput = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        //bool downInput = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        //bool leftInput = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        //bool rightInput = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);

        //// スティック入力判定用の変数
        //bool stickDownInput = false;
        //bool stickUpInput = false;
        //bool stickLeftInput = false;
        //bool stickRightInput = false;



        //if (verticalInput <= -0.5f && !wasStickDown)
        //{
        //    stickDownInput = true;
        //    wasStickDown = true;
        //}
        //else if (verticalInput > -0.3f) // デッドゾーン
        //{
        //    wasStickDown = false;
        //}

        //if (verticalInput >= 0.5f && !wasStickUp)
        //{
        //    stickUpInput = true;
        //    wasStickUp = true;
        //}
        //else if (verticalInput < 0.3f) // デッドゾーン
        //{
        //    wasStickUp = false;
        //}

        //// 水平方向の処理（新規追加）
        //if (horizontalInput <= -0.5f && !wasStickLeft)
        //{
        //    stickLeftInput = true;
        //    wasStickLeft = true;
        //}
        //else if (horizontalInput > -0.3f) // デッドゾーン
        //{
        //    wasStickLeft = false;
        //}

        //if (horizontalInput >= 0.5f && !wasStickRight)
        //{
        //    stickRightInput = true;
        //    wasStickRight = true;
        //}
        //else if (horizontalInput < 0.3f) // デッドゾーン
        //{
        //    wasStickRight = false;
        //}

        //if (downInput || stickDownInput)
        //{
        //    //Indexは0から始まるため、範囲を超えないように
        //    if (stageIndexs.Length - 1 > currentIndex)
        //    {
        //        CrossroadPoint crossroad = FindCrossroad(currentIndex, currentIndex + 1);

        //        if (crossroad != null)
        //        {
        //            // 分岐点を経由
        //            currentCrossroad = crossroad;
        //            isAtCrossroad = true;
        //            SetKyori(currentCrossroad.transform.position);
        //            //UpdateDirectionIndicators();
        //            return;
        //        }

        //        // 分岐点がなければ直接前のステージへ（従来の動作）
        //        currentIndex++;
        //        SetKyori(stageIndexs[currentIndex].transform.position);
        //        maxmin = new Vector2(currentIndex, currentIndex + 1);
        //        lastInputTime = Time.time; // 入力処理時間を記録
        //    }
        //}

        ////if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Horizontal") >= 0.5f || Input.GetAxis("Vertical") <= -0.5f)
        ////{           

        ////}

        //// if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Horizontal") <= -0.5f || Input.GetAxis("Vertical") >= 0.5f)
        ////{
        ////}
        //if (upPressed || stickUpInput)
        //{
        //    if (currentIndex > 0)
        //    {
        //        CrossroadPoint crossroad = FindCrossroad(currentIndex, currentIndex - 1);

        //        if (crossroad != null)
        //        {
        //            // 分岐点を経由
        //            currentCrossroad = crossroad;
        //            isAtCrossroad = true;
        //            SetKyori(currentCrossroad.transform.position);
        //            //UpdateDirectionIndicators();
        //            return;
        //        }

        //        // 分岐点がなければ直接前のステージへ（従来の動作）
        //        currentIndex--;
        //        SetKyori(stageIndexs[currentIndex].transform.position);
        //        maxmin = new Vector2(currentIndex, currentIndex + 1);
        //        lastInputTime = Time.time;
        //    }
        //}
        #endregion


        #region 新方式操作

        // 前のフレームの入力状態を保存する変数
        bool prevUpKey = false;
        bool prevDownKey = false;
        bool prevLeftKey = false;
        bool prevRightKey = false;

        // 入力の遅延処理
        if (Time.time < lastInputTime + inputDelay)
        {
            return; // 一定時間は入力を受け付けない
        }

        // 各方向の入力を検出
        bool moveUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ||
                     (Input.GetAxis("Vertical") >= 0.5f && !wasStickUp);
        bool moveDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) ||
                       (Input.GetAxis("Vertical") <= -0.5f && !wasStickDown);
        bool moveLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ||
                       (Input.GetAxis("Horizontal") <= -0.5f && !wasStickLeft);
        bool moveRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) ||
                        (Input.GetAxis("Horizontal") >= 0.5f && !wasStickRight);


        // 現在のステージから移動先を決定
        if (moveUp && !prevUpKey && currentCrossroad.HasUp())
        {
            Debug.Log("UP押されました");
            // 上方向の隣接ステージまたは分岐点を探す
            NavigateInDirection("up");
            lastInputTime = Time.time;
        }
        else if (moveDown && !prevDownKey && currentCrossroad.HasDown())
        {
            Debug.Log("DOWN押されました");
            NavigateInDirection("down");
            lastInputTime = Time.time;
        }
        else if (moveLeft && !prevLeftKey) //&& currentCrossroad.HasLeft()) //今のところCrossroadの左方向にオブジェがないから効かなくなってしまう
        {
            Debug.Log("LEFT押されました");
            NavigateInDirection("left");
            lastInputTime = Time.time;
        }
        else if (moveRight && !prevRightKey && currentCrossroad.HasRight())
        {
            Debug.Log("RIGHT押されました");
            NavigateInDirection("right");
            lastInputTime = Time.time;
        }

        // 入力状態を更新
        prevUpKey = moveUp;
        prevDownKey = moveDown;
        prevLeftKey = moveLeft;
        prevRightKey = moveRight;
        #endregion

    }


    private CrossroadPoint FindCrossroad(int stage1Index, int stage2Index)
    {
        CrossroadPoint[] allCrossroads = FindObjectsOfType<CrossroadPoint>();

        foreach (CrossroadPoint crossroad in allCrossroads)
        {
            bool connectsToStage1 = false;
            bool connectsToStage2 = false;

            // 各方向をチェック
            Direction[] directions = { crossroad.up, crossroad.down, crossroad.left, crossroad.right };
            foreach (Direction dir in directions)
            {
                if (dir.type == Direction.DirectionType.Stage)
                {
                    if (dir.stageSelector == stageIndexs[stage1Index])
                        connectsToStage1 = true;

                    if (dir.stageSelector == stageIndexs[stage2Index])
                        connectsToStage2 = true;
                }
            }

            // 両方のステージに接続している分岐点を見つけた
            if (connectsToStage1 && connectsToStage2)
                return crossroad;
        }

        return null;
    }

    // UIインジケーターの更新
    private void UpdateDirectionIndicators()
    {
        //分岐点にいる場合
        if (isAtCrossroad)
        {
            // 分岐点での方向表示
            UIAnim.SetAvailableDirections(
                currentCrossroad.HasRight(),
                currentCrossroad.HasLeft(),
                currentCrossroad.HasUp(),
                currentCrossroad.HasDown()
            );
        }
        else
        {
            bool canMoveRight = CanMoveInDirection("right");
            bool canMoveLeft = CanMoveInDirection("left");
            bool canMoveUp = CanMoveInDirection("up");
            bool canMoveDown = CanMoveInDirection("down");

            // 実際に移動可能な方向のみ矢印を表示
            UIAnim.SetAvailableDirections(
                canMoveRight,
                canMoveLeft,
                canMoveUp,
                canMoveDown
            );
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
            if (currentCrossroad != null && isAtCrossroad)
            {
                isJustCrossroad = !isJustCrossroad;

            }

            if (!isEnteringStage && animator != null)  // 遷移中でない場合のみアニメーション更新
            {
                animator.SetBool("Walk", false);
                
                //UI表示の更新は1回だけにしたい
                if(!isUpdatedDirection)
                {
                    UpdateDirectionIndicators();
                    isUpdatedDirection = !isUpdatedDirection; 
                }

            }

            if (currentIndex > 0 && pathIndex > 0)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown("joystick button 0"))
                {
                    Debug.Log("エンターキーが押されました");
                    if(!isAtCrossroad)
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
                UIAnim.SetMovingState(!isMoving);
                
                //移動中ならばUI表示のフラグを立てる
                if(isMoving)
                {
                    isUpdatedDirection = false;                
                }
                
            }

            // UI表示を更新
            
            //Debug.Log(isMoving);
            
            
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

    //ステージ遷移時のアニメーションコルーチン
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


    private CrossroadPoint FindConnectedCrossroad()
    {
        CrossroadPoint[] allCrossroads = FindObjectsOfType<CrossroadPoint>();
        foreach (CrossroadPoint crossroad in allCrossroads)
        {
            // 各方向をチェックして、現在のステージセレクターが含まれるか確認
            if ((crossroad.up.type == Direction.DirectionType.Stage && crossroad.up.stageSelector == stageIndexs[currentIndex]) ||
                (crossroad.down.type == Direction.DirectionType.Stage && crossroad.down.stageSelector == stageIndexs[currentIndex]) ||
                (crossroad.left.type == Direction.DirectionType.Stage && crossroad.left.stageSelector == stageIndexs[currentIndex]) ||
                (crossroad.right.type == Direction.DirectionType.Stage && crossroad.right.stageSelector == stageIndexs[currentIndex]))
            {
                return crossroad;
            }
        }
        return null;
    }

    #region 試験

    

    // 指定方向に移動
    private void NavigateInDirection(string direction)
    {
        // 現在のステージ位置
        Vector2 currentPosition = stageIndexs[currentIndex].transform.position;

        // 各ステージをチェックして最も適切な移動先を見つける
        SelectSceneManager bestTarget = null;
        CrossroadPoint bestCrossroad = null;
        float closestDistance = float.MaxValue;

        // ステージが移動先の候補
        foreach (SelectSceneManager stage in stageIndexs)
        {
            // 現在のステージはスキップ
            if (stage == stageIndexs[currentIndex])
                continue;

            Vector2 targetPos = stage.transform.position;
            Vector2 directionVector = targetPos - currentPosition;

            // 方向に応じてフィルタリング
            if (IsInDirection(directionVector, direction))
            {
                float distance = Vector2.Distance(currentPosition, targetPos);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = stage;
                    bestCrossroad = null;
                }
            }
        }

        // 分岐点も移動先の候補
        foreach (CrossroadPoint crossroad in FindObjectsOfType<CrossroadPoint>())
        {
            Vector2 targetPos = crossroad.transform.position;
            Vector2 directionVector = targetPos - currentPosition;

            if (IsInDirection(directionVector, direction))
            {
                float distance = Vector2.Distance(currentPosition, targetPos);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = null;
                    bestCrossroad = crossroad;
                }
            }
        }

        // 移動先が見つかった場合、移動を実行
        if (bestTarget != null)
        {
            // 次のステージに直接移動
            for (int i = 0; i < stageIndexs.Length; i++)
            {
                if (stageIndexs[i] == bestTarget)
                {
                    currentIndex = i;
                    break;
                }
            }
            SetKyori(bestTarget.transform.position);
            lastInputTime = Time.time;
        }
        else if (bestCrossroad != null)
        {
            // 分岐点に移動
            currentCrossroad = bestCrossroad;
            isAtCrossroad = true;
            SetKyori(bestCrossroad.transform.position);
            lastInputTime = Time.time;
        }
    }

    // 指定方向に移動可能かどうかを判断する。
    private bool CanMoveInDirection(string direction)
    {
        Vector2 currentPosition = stageIndexs[currentIndex].transform.position;

        // 各方向ごとにチェック
        foreach (SelectSceneManager stage in stageIndexs)
        {
            // 現在のステージはスキップ
            if (stage == stageIndexs[currentIndex])
                continue;

            Vector2 targetPos = stage.transform.position;
            Vector2 directionVector = targetPos - currentPosition;

            // この方向に移動可能なステージがあるか
            if (IsInDirection(directionVector, direction))
                return true;
        }

        // 分岐点もチェック
        foreach (CrossroadPoint crossroad in FindObjectsOfType<CrossroadPoint>())
        {
            Vector2 targetPos = crossroad.transform.position;
            Vector2 directionVector = targetPos - currentPosition;

            if (IsInDirection(directionVector, direction))
                return true;
        }

        // 移動可能なオブジェクトが見つからなかった
        return false;
    }

    // 指定されたベクトルが特定の方向に向いているかどうか判定
    private bool IsInDirection(Vector2 vector, string direction)
    {
        //Debug.Log("ベクトルの計算実行:" + direction);
        vector = vector.normalized;

        switch (direction)
        {
            case "up":
                return vector.y > 0.8f;  // Y成分が大きく正の値
            case "down":
                return vector.y < -0.7f; // Y成分が大きく負の値
            case "left":
                return vector.x < -0.7f; // X成分が大きく負の値
            case "right":
                return vector.x > 0.8f;  // X成分が大きく正の値
            default:
                return false;
        }
    }

    #endregion

}
