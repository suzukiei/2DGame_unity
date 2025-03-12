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
    private bool isEnteringStage = false;  //�t���O

    //���ʉ�
    [SerializeField] private AudioClip enterSound; 
    private AudioSource audioSource;


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

        if (stageIndexs.Length > 0)
        {
            SetKyori(stageIndexs[currentIndex].transform.position);
        }

        //�ŏ��Ƀt�F�[�h�C�����K�v�ȃ����_�� ���ꂵ�Ȃ��Ɖ�ʐ^���ÂȂ܂܎n�܂�
        fade.FadeStart(() => {     
        });

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentIndex);
        //Debug.Log(stageIndexs[currentIndex].StageIndex);
        //Debug.Log(stageIndexs[currentIndex].transform.position);
        //�_�C�A���O���o�Ă���Ƃ��̓L�[������󂯕t���Ȃ�
        if (!isEnteringStage)
        {

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Horizontal") >=0.5f || Input.GetAxis("Vertical") <= -0.5f)
            {
                //Index��0����n�܂邽�߁A�͈͂𒴂��Ȃ��悤��
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

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Horizontal") <= -0.5f || Input.GetAxis("Vertical") >= 0.5f)
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
        // �v���C���[�̈ʒu���X�e�[�W�|�C���g�̈ʒu�Ɉړ�
        //transform.position = stageIndexs[Index].transform.position;

        //�o�H�����݂��Ȃ����A���B�ς݂̏ꍇ
        if (path.corners.Length == 0 || pathIndex >= path.corners.Length)
        {
            if (maxmin.y - maxmin.x == 1)
            {
                maxmin = new Vector2(currentIndex-1, currentIndex + 1);
            }
            if (!isEnteringStage && animator != null)  // �J�ڒ��łȂ��ꍇ�̂݃A�j���[�V�����X�V
            {
                animator.SetBool("Walk", false);
            }

            if (currentIndex > 0 && pathIndex > 0)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown("joystick button 0"))
                {
                    Debug.Log("�G���^�[�L�[��������܂���");

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
            }
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


}
