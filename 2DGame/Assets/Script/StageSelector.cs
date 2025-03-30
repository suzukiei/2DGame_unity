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
    // �o�H�v�Z�p
     [Header("Steering")]
    public float speed = 1.0f;
    public float stoppingDistance = 0.1f; // �ړI�n�ɓ��B�Ƃ݂Ȃ�����

    private Vector2 traceArea = Vector2.zero;
    private NavMeshPath path;
    private int pathIndex;
    [SerializeField]
    private Vector2 maxmin =new Vector2(0,1);
    //�X�e�[�W�|�C���g�Ǘ�
    private SelectSceneManager[] stageIndexs;
    private int currentIndex;

    //�A�j��
    private Animator animator;
    private Fade fade;
    // private bool bStart;
    private bool isEnteringStage = false;  //�X�e�[�W�J�ڒ�������

    //���ʉ�
    [SerializeField] private AudioClip enterSound; 
    private AudioSource audioSource;

    //UI�p
    // UI�R���g���[���[�̎Q�Ƃ�ǉ�
    [SerializeField] private UIAnimation UIAnim;

    [Header("����_����")]
    public CrossroadPoint currentCrossroad;
    private bool isAtCrossroad = false;
    private bool isJustCrossroad = false; //���x�������Ă��痧�Ă�t���O

    // �X�e�B�b�N�̓��͏�Ԃ��L�^����ÓI�ȕϐ�
    static bool wasStickDown = false;
    static bool wasStickUp = false;
    static bool wasStickLeft = false;
    static bool wasStickRight = false;
    private float inputDelay = 0.5f; // ���͂̒x�����ԁi�b�j
    private float lastInputTime = 0f; // �Ō�ɓ��͂��������ꂽ����
    private bool isUpdatedDirection = false;



    // Start is called before the first frame update
    void Start()
    {

       
        //FindObjectsOfType<SelectSceneManager>()�������Ɣz��ɓ����Ă��鏇�Ԃ��s���ĂȂ��߁A
        //������ƃX�e�[�W�ԍ����ŊǗ�����
        stageIndexs = FindObjectsOfType<SelectSceneManager>().OrderBy(obj => obj.StageIndex).ToArray();

        // NavMeshPath ��������
        path = new NavMeshPath();

        animator = GetComponent<Animator>();

        fade = FindObjectOfType<Fade>();
        maxmin = new Vector2(0, 1);
        // bStart = false;

        //Sentakushi = GameObject.Find("Sentakushi_Dialogue").GetComponent<NPCConversation>();

        // �ۑ����ꂽ�ʒu������ꍇ | �f�t�H�l��0�Ƃ���
        currentIndex = PlayerPrefs.GetInt("CurrentStagePosition", 0);

        audioSource = GetComponent<AudioSource>();


            stageIndexs = FindObjectsOfType<SelectSceneManager>().OrderBy(obj => obj.StageIndex).ToArray();
            if (stageIndexs.Length > 0)
            {
                SetKyori(stageIndexs[currentIndex].transform.position);
                UpdateDirectionIndicators();
            }
        

        //�ŏ��Ƀt�F�[�h�C�����K�v�ȃ����_�� ���ꂵ�Ȃ��Ɖ�ʐ^���ÂȂ܂܎n�܂�
        fade.FadeStart(() => {     
        });

    }

    // Update is called once per frame
    void Update()
    {
       Debug.Log(currentIndex);
        //Debug.Log(stageIndexs[currentIndex].StageIndex);
        //Debug.Log(stageIndexs[currentIndex].transform.position);
        //�_�C�A���O���o�Ă���Ƃ��̓L�[������󂯕t���Ȃ�
        if (!isEnteringStage)
        {
            if (isAtCrossroad)
            {
                //Debug.Log("����_�ɂ��܂��B");
                // ����_�ł̑��쏈��
                HandleCrossroadInput();
            }
            else
            {
                // �ʏ�̃X�e�[�W�I���̑��쏈��
                HandleStageSelectionInput();
            }

            MoveToStagePoint();
        }      
           
        
    }

    // ����_�ł̓��͏���
    private void HandleCrossroadInput()
    {

        if (isJustCrossroad)
        {
            // �e�����L�[���͂̏���
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
                Debug.Log("LEFT�����܂���");
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

    // �����ɉ������ړ�����
    private void MoveInDirection(string direction)
    {
        Direction targetDirection = null;

        if(isJustCrossroad) isJustCrossroad = !isJustCrossroad; 


        // �w�肳�ꂽ������Direction���擾
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
                // �X�e�[�W�|�C���g�ֈړ�
                isAtCrossroad = false;

                // �X�e�[�W�C���f�b�N�X�����
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
                // �ʂ̕���_�ֈړ�
                currentCrossroad = targetDirection.crossRoad;
                SetKyori(currentCrossroad.transform.position);

                
            }

            
        }
    }

    private void HandleStageSelectionInput()
    {
        #region ����������
        //// ���͂̒x������
        //if (Time.time < lastInputTime + inputDelay)
        //{
        //    return; // ��莞�Ԃ͓��͂��󂯕t���Ȃ�
        //}

        //// �A�i���O�X�e�B�b�N�̓��͂��`�F�b�N
        //float horizontalInput = Input.GetAxis("Horizontal");
        //float verticalInput = Input.GetAxis("Vertical");

        //// �L�[�{�[�h���͂܂��̓X�e�B�b�N���͂𔻒�
        //bool upInput = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        //bool downInput = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        //bool leftInput = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        //bool rightInput = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);

        //// �X�e�B�b�N���͔���p�̕ϐ�
        //bool stickDownInput = false;
        //bool stickUpInput = false;
        //bool stickLeftInput = false;
        //bool stickRightInput = false;



        //if (verticalInput <= -0.5f && !wasStickDown)
        //{
        //    stickDownInput = true;
        //    wasStickDown = true;
        //}
        //else if (verticalInput > -0.3f) // �f�b�h�]�[��
        //{
        //    wasStickDown = false;
        //}

        //if (verticalInput >= 0.5f && !wasStickUp)
        //{
        //    stickUpInput = true;
        //    wasStickUp = true;
        //}
        //else if (verticalInput < 0.3f) // �f�b�h�]�[��
        //{
        //    wasStickUp = false;
        //}

        //// ���������̏����i�V�K�ǉ��j
        //if (horizontalInput <= -0.5f && !wasStickLeft)
        //{
        //    stickLeftInput = true;
        //    wasStickLeft = true;
        //}
        //else if (horizontalInput > -0.3f) // �f�b�h�]�[��
        //{
        //    wasStickLeft = false;
        //}

        //if (horizontalInput >= 0.5f && !wasStickRight)
        //{
        //    stickRightInput = true;
        //    wasStickRight = true;
        //}
        //else if (horizontalInput < 0.3f) // �f�b�h�]�[��
        //{
        //    wasStickRight = false;
        //}

        //if (downInput || stickDownInput)
        //{
        //    //Index��0����n�܂邽�߁A�͈͂𒴂��Ȃ��悤��
        //    if (stageIndexs.Length - 1 > currentIndex)
        //    {
        //        CrossroadPoint crossroad = FindCrossroad(currentIndex, currentIndex + 1);

        //        if (crossroad != null)
        //        {
        //            // ����_���o�R
        //            currentCrossroad = crossroad;
        //            isAtCrossroad = true;
        //            SetKyori(currentCrossroad.transform.position);
        //            //UpdateDirectionIndicators();
        //            return;
        //        }

        //        // ����_���Ȃ���Β��ڑO�̃X�e�[�W�ցi�]���̓���j
        //        currentIndex++;
        //        SetKyori(stageIndexs[currentIndex].transform.position);
        //        maxmin = new Vector2(currentIndex, currentIndex + 1);
        //        lastInputTime = Time.time; // ���͏������Ԃ��L�^
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
        //            // ����_���o�R
        //            currentCrossroad = crossroad;
        //            isAtCrossroad = true;
        //            SetKyori(currentCrossroad.transform.position);
        //            //UpdateDirectionIndicators();
        //            return;
        //        }

        //        // ����_���Ȃ���Β��ڑO�̃X�e�[�W�ցi�]���̓���j
        //        currentIndex--;
        //        SetKyori(stageIndexs[currentIndex].transform.position);
        //        maxmin = new Vector2(currentIndex, currentIndex + 1);
        //        lastInputTime = Time.time;
        //    }
        //}
        #endregion


        #region �V��������

        // �O�̃t���[���̓��͏�Ԃ�ۑ�����ϐ�
        bool prevUpKey = false;
        bool prevDownKey = false;
        bool prevLeftKey = false;
        bool prevRightKey = false;

        // ���͂̒x������
        if (Time.time < lastInputTime + inputDelay)
        {
            return; // ��莞�Ԃ͓��͂��󂯕t���Ȃ�
        }

        // �e�����̓��͂����o
        bool moveUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ||
                     (Input.GetAxis("Vertical") >= 0.5f && !wasStickUp);
        bool moveDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) ||
                       (Input.GetAxis("Vertical") <= -0.5f && !wasStickDown);
        bool moveLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ||
                       (Input.GetAxis("Horizontal") <= -0.5f && !wasStickLeft);
        bool moveRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) ||
                        (Input.GetAxis("Horizontal") >= 0.5f && !wasStickRight);


        // ���݂̃X�e�[�W����ړ��������
        if (moveUp && !prevUpKey && currentCrossroad.HasUp())
        {
            Debug.Log("UP������܂���");
            // ������̗אڃX�e�[�W�܂��͕���_��T��
            NavigateInDirection("up");
            lastInputTime = Time.time;
        }
        else if (moveDown && !prevDownKey && currentCrossroad.HasDown())
        {
            Debug.Log("DOWN������܂���");
            NavigateInDirection("down");
            lastInputTime = Time.time;
        }
        else if (moveLeft && !prevLeftKey) //&& currentCrossroad.HasLeft()) //���̂Ƃ���Crossroad�̍������ɃI�u�W�F���Ȃ���������Ȃ��Ȃ��Ă��܂�
        {
            Debug.Log("LEFT������܂���");
            NavigateInDirection("left");
            lastInputTime = Time.time;
        }
        else if (moveRight && !prevRightKey && currentCrossroad.HasRight())
        {
            Debug.Log("RIGHT������܂���");
            NavigateInDirection("right");
            lastInputTime = Time.time;
        }

        // ���͏�Ԃ��X�V
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

            // �e�������`�F�b�N
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

            // �����̃X�e�[�W�ɐڑ����Ă��镪��_��������
            if (connectsToStage1 && connectsToStage2)
                return crossroad;
        }

        return null;
    }

    // UI�C���W�P�[�^�[�̍X�V
    private void UpdateDirectionIndicators()
    {
        //����_�ɂ���ꍇ
        if (isAtCrossroad)
        {
            // ����_�ł̕����\��
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

            // ���ۂɈړ��\�ȕ����̂ݖ���\��
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
        // �v���C���[�̈ʒu���X�e�[�W�|�C���g�̈ʒu�Ɉړ�
        //transform.position = stageIndexs[Index].transform.position;


        //�o�H�����݂��Ȃ����A���B�ς݂̏ꍇ
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

            if (!isEnteringStage && animator != null)  // �J�ڒ��łȂ��ꍇ�̂݃A�j���[�V�����X�V
            {
                animator.SetBool("Walk", false);
                
                //UI�\���̍X�V��1�񂾂��ɂ�����
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
                    Debug.Log("�G���^�[�L�[��������܂���");
                    if(!isAtCrossroad)
                        StartCoroutine(EnterStageAnimation());

                }
            }

            return;
        }

        Vector2 currentPos = transform.position;
        Vector2 targetPos = path.corners[pathIndex];

        #region �A�j���[�V��������
        // �ړ������ǂ����𔻒�       
        // �A�j���[�^�[���A�^�b�`����Ă���ꍇ�AWalk �p�����[�^��ݒ�
        if (!isEnteringStage)  // �J�ڒ��łȂ��ꍇ�̂݃A�j���[�V�����X�V
        {
            bool isMoving = Vector2.Distance(currentPos, targetPos) > stoppingDistance;
            if (animator != null)
            {
                animator.SetBool("Walk", isMoving);
                UIAnim.SetMovingState(!isMoving);
                
                //�ړ����Ȃ��UI�\���̃t���O�𗧂Ă�
                if(isMoving)
                {
                    isUpdatedDirection = false;                
                }
                
            }

            // UI�\�����X�V
            
            //Debug.Log(isMoving);
            
            
        }
        #endregion

        #region ���菈���֌W
        // ���݈ʒu�� NavMesh �͈͓̔����`�F�b�N
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(currentPos, out hit, 0.1f, NavMesh.AllAreas))
        {
            // NavMesh �͈̔͊O�̏ꍇ�A�ł��߂��L���Ȉʒu�ɖ߂�
            if (NavMesh.SamplePosition(currentPos, out hit, 1.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                currentPos = hit.position;
            }
        }

        // �^�C���̒��S�ɃX�i�b�v����O�ɁA�ڕW�ʒu�� NavMesh ��ɂ��邩�m�F
        if (NavMesh.SamplePosition(targetPos, out hit, 0.1f, NavMesh.AllAreas))
        {
            targetPos = hit.position;
        }

        // ���̌�Ń^�C���̒��S�ɃX�i�b�v
        targetPos = TileCenterKeisan(targetPos);

        float distanceToTarget = Vector2.Distance(currentPos, targetPos);
        if (distanceToTarget <= stoppingDistance)
        {
            pathIndex++;
            return;
        }

        // ��x�Ɉړ����鋗���𐧌�
        float maxMoveDistance = speed * Time.deltaTime;
        Vector2 moveDirection = (targetPos - currentPos).normalized;
        Vector2 newPosition = currentPos + moveDirection * Mathf.Min(maxMoveDistance, distanceToTarget);

        // �ړ��O�ɐV�����ʒu�� NavMesh ��ɂ��邩�m�F
        if (NavMesh.SamplePosition(newPosition, out hit, 0.1f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        #endregion
    }

    private void KyoriKeisan(Vector2 current, Vector2 target)
    {
        // NavMesh �Ɋ�Â��Čo�H�����߂�
        path.ClearCorners();
        pathIndex = 0;

        if (NavMesh.CalculatePath(current, target, NavMesh.AllAreas, path))
        {
            Debug.Log("�o�H�v�Z����");
        }
        else
        {
            Debug.LogError("�o�H�v�Z���s");
        }
    }

    public void SetKyori(Vector2 target)
    {
        traceArea = target;
        KyoriKeisan(transform.position, target);
    }

    //�^�C���̒��S�����߂�
    private Vector2 TileCenterKeisan(Vector2 position)
    {
        // �^�C���T�C�Y���擾 (��: �^�C����1�}�X��1x1���j�b�g�̏ꍇ)
        float tileSize = 1f;

        // ���W���^�C���̒��S�ɃX�i�b�v
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
    //    // NavMeshPath ���Čv�Z
    //    path.ClearCorners();
    //    pathIndex = 0;

    //    Vector3 targetPosition = stageIndexs[Index].transform.position;

    //    if (NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
    //    {
    //        Debug.Log("�o�H�v�Z�����I");
    //    }
    //    else
    //    {
    //        Debug.LogError("�o�H�v�Z���s�I");
    //    }
    //}

    //�X�e�[�W�J�ڎ��̃A�j���[�V�����R���[�`��
    private IEnumerator EnterStageAnimation()
    {
        isEnteringStage = true;
        UIAnim.SetMovingState(false);

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
        float startSize = mainCamera.orthographicSize; // �J�n���̃J�����T�C�Y
        float targetSize = startSize * 0.5f; // �Y�[���C����̃T�C�Y�i���l�͒����\�j

        // �؂̈ʒu�Ɍ������Ĉړ�
        Vector3 woodDirection = (stageIndexs[currentIndex].transform.position - startPosition).normalized;
        Vector3 targetPosition = startPosition + (woodDirection * 2f);

        // �J�����̖ڕW�ʒu�i�؂̈ʒu�̕����ցj
        Vector3 cameraTargetPosition = Vector3.Lerp(cameraStartPosition,
            new Vector3(stageIndexs[currentIndex].transform.position.x,
                       stageIndexs[currentIndex].transform.position.y,
                       cameraStartPosition.z), 0.3f); // 0.3f�͖؂̕����ւ̈ړ��x����

        fade.FadeStart(() => {
            SceneManager.LoadScene(stageIndexs[currentIndex].StageName);
        });

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);

            // �L�����N�^�[�̃A�j���[�V����
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, smoothT);
            transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);

            // �J�����̃Y�[���C���ƈړ�
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
            // �e�������`�F�b�N���āA���݂̃X�e�[�W�Z���N�^�[���܂܂�邩�m�F
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

    #region ����

    

    // �w������Ɉړ�
    private void NavigateInDirection(string direction)
    {
        // ���݂̃X�e�[�W�ʒu
        Vector2 currentPosition = stageIndexs[currentIndex].transform.position;

        // �e�X�e�[�W���`�F�b�N���čł��K�؂Ȉړ����������
        SelectSceneManager bestTarget = null;
        CrossroadPoint bestCrossroad = null;
        float closestDistance = float.MaxValue;

        // �X�e�[�W���ړ���̌��
        foreach (SelectSceneManager stage in stageIndexs)
        {
            // ���݂̃X�e�[�W�̓X�L�b�v
            if (stage == stageIndexs[currentIndex])
                continue;

            Vector2 targetPos = stage.transform.position;
            Vector2 directionVector = targetPos - currentPosition;

            // �����ɉ����ăt�B���^�����O
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

        // ����_���ړ���̌��
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

        // �ړ��悪���������ꍇ�A�ړ������s
        if (bestTarget != null)
        {
            // ���̃X�e�[�W�ɒ��ڈړ�
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
            // ����_�Ɉړ�
            currentCrossroad = bestCrossroad;
            isAtCrossroad = true;
            SetKyori(bestCrossroad.transform.position);
            lastInputTime = Time.time;
        }
    }

    // �w������Ɉړ��\���ǂ����𔻒f����B
    private bool CanMoveInDirection(string direction)
    {
        Vector2 currentPosition = stageIndexs[currentIndex].transform.position;

        // �e�������ƂɃ`�F�b�N
        foreach (SelectSceneManager stage in stageIndexs)
        {
            // ���݂̃X�e�[�W�̓X�L�b�v
            if (stage == stageIndexs[currentIndex])
                continue;

            Vector2 targetPos = stage.transform.position;
            Vector2 directionVector = targetPos - currentPosition;

            // ���̕����Ɉړ��\�ȃX�e�[�W�����邩
            if (IsInDirection(directionVector, direction))
                return true;
        }

        // ����_���`�F�b�N
        foreach (CrossroadPoint crossroad in FindObjectsOfType<CrossroadPoint>())
        {
            Vector2 targetPos = crossroad.transform.position;
            Vector2 directionVector = targetPos - currentPosition;

            if (IsInDirection(directionVector, direction))
                return true;
        }

        // �ړ��\�ȃI�u�W�F�N�g��������Ȃ�����
        return false;
    }

    // �w�肳�ꂽ�x�N�g��������̕����Ɍ����Ă��邩�ǂ�������
    private bool IsInDirection(Vector2 vector, string direction)
    {
        //Debug.Log("�x�N�g���̌v�Z���s:" + direction);
        vector = vector.normalized;

        switch (direction)
        {
            case "up":
                return vector.y > 0.8f;  // Y�������傫�����̒l
            case "down":
                return vector.y < -0.7f; // Y�������傫�����̒l
            case "left":
                return vector.x < -0.7f; // X�������傫�����̒l
            case "right":
                return vector.x > 0.8f;  // X�������傫�����̒l
            default:
                return false;
        }
    }

    #endregion

}
