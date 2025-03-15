using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugMode : MonoBehaviour
{
    [SerializeField] private bool visualizeColliders = true;
    [SerializeField] private Color boxColliderColor = new Color(0, 1, 0, 0.3f);    // �΁i�������j
    [SerializeField] private Color circleColliderColor = new Color(0, 0, 1, 0.3f); // �i�������j
    [SerializeField] private Color capsuleColliderColor = new Color(1, 0, 0, 0.3f); // �ԁi�������j
    [SerializeField] private Color triggerColor = new Color(1, 1, 0, 0.3f);
    [SerializeField] private Color edgeColor = Color.yellow;
    [SerializeField] private float edgeThickness = 0.1f;

    private Vector3 InitTransform; //���X�|�[���ʒu���i�[����ꏊ
    GameObject PlayerObject;
    
    [Header("�f�o�b�O�ؑ֐ݒ�")]
    [SerializeField] private KeyCode debugToggleModifier = KeyCode.LeftShift; // Shift�L�[
    [SerializeField] private KeyCode debugToggleKey = KeyCode.D; // D�L�[

    private Camera mainCamera;
    private Material lineMaterial;

    

    private void Start()
    {
        
        InitTransform = PlayerObject.gameObject.transform.position;
        PlayerObject = GameObject.FindGameObjectWithTag("Player");
        mainCamera = Camera.main;
        CreateLineMaterial();


    }

    private void Update()
    {
        if (PlayerObject == null)
        {
            PlayerObject = GameObject.FindGameObjectWithTag("Player");
            if (PlayerObject == null) return; // ������Ȃ��ꍇ�͏������X�L�b�v
        }

        // Shift + D�Ńf�o�b�O���[�h�؂�ւ�
        if (Input.GetKey(debugToggleModifier) && Input.GetKeyDown(debugToggleKey))
        {
            visualizeColliders = !visualizeColliders;
            Debug.Log($"�f�o�b�O���[�h: {(visualizeColliders ? "ON" : "OFF")}");
        }

        if(Input.GetKey(KeyCode.F2))
        {
            SceneManager.LoadScene("Title");
        }

        if(Input.GetKey(KeyCode.F3))
        {
            PlayerObject.transform.position = InitTransform;
        }
    }

    //�Q�[���r���[�̕`�揈��
    private void OnPostRender() //�J�����̃����_�����O��ɌĂ΂�����ȃR�[���o�b�N�炵��
    {
        if (!visualizeColliders) return;

        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();

        //�ȉ�GL�֘A�̐ݒ�
        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.LoadProjectionMatrix(mainCamera.projectionMatrix);
        GL.modelview = mainCamera.worldToCameraMatrix;

        //�R���C�_�[�̃^�C�v�ɉ������Ăяo��������
        foreach (Collider2D collider in allColliders)
        {
            if (!collider.enabled || !collider.gameObject.activeInHierarchy) continue;

            Color color = collider.isTrigger ? triggerColor :
                     (collider is BoxCollider2D) ? boxColliderColor :
                     (collider is CircleCollider2D) ? circleColliderColor :
                     new Color(1, 0, 1, 0.3f);

            DrawCollider(collider, color);
        }

        GL.PopMatrix();
    }

    private void DrawCollider(Collider2D collider, Color color)
    {
        //Box�R���C�_�[�̏ꍇ
        if (collider is BoxCollider2D)
        {
            BoxCollider2D boxCollider = collider as BoxCollider2D;
            Vector2 size = boxCollider.size;
            size.x *= collider.transform.lossyScale.x;
            size.y *= collider.transform.lossyScale.y;

            Matrix4x4 matrix = Matrix4x4.TRS(
                collider.transform.TransformPoint(boxCollider.offset),
                collider.transform.rotation,
                Vector3.one
            );

            DrawBox(matrix, size, color);
        } //�T�[�N���R���C�_�[�̏ꍇ
        else if (collider is CircleCollider2D)
        {
            CircleCollider2D circleCollider = collider as CircleCollider2D;
            float radius = circleCollider.radius * Mathf.Max(
                collider.transform.lossyScale.x,
                collider.transform.lossyScale.y
            );

            Vector2 position = collider.transform.TransformPoint(circleCollider.offset);
            DrawCircle(position, radius, color);
        }
        // ���̃R���C�_�[�^�C�v�����l�Ɏ�������\��ł���B
    }

    private void DrawBox(Matrix4x4 matrix, Vector2 size, Color color)
    {
        GL.Begin(GL.LINES);
        GL.Color(color);

        // ����
        GL.Vertex(matrix.MultiplyPoint(new Vector3(-size.x / 2, -size.y / 2, 0)));
        GL.Vertex(matrix.MultiplyPoint(new Vector3(size.x / 2, -size.y / 2, 0)));

        // �E��
        GL.Vertex(matrix.MultiplyPoint(new Vector3(size.x / 2, -size.y / 2, 0)));
        GL.Vertex(matrix.MultiplyPoint(new Vector3(size.x / 2, size.y / 2, 0)));

        // ���
        GL.Vertex(matrix.MultiplyPoint(new Vector3(size.x / 2, size.y / 2, 0)));
        GL.Vertex(matrix.MultiplyPoint(new Vector3(-size.x / 2, size.y / 2, 0)));

        // ����
        GL.Vertex(matrix.MultiplyPoint(new Vector3(-size.x / 2, size.y / 2, 0)));
        GL.Vertex(matrix.MultiplyPoint(new Vector3(-size.x / 2, -size.y / 2, 0)));

        GL.End();
    }

    private void DrawCircle(Vector2 center, float radius, Color color)
    {
        GL.Begin(GL.LINES);
        GL.Color(color);

        const int segments = 36;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 point1 = new Vector3(
                center.x + radius * Mathf.Cos(angle1),
                center.y + radius * Mathf.Sin(angle1),
                0
            );

            Vector3 point2 = new Vector3(
                center.x + radius * Mathf.Cos(angle2),
                center.y + radius * Mathf.Sin(angle2),
                0
            );

            GL.Vertex(point1);
            GL.Vertex(point2);
        }

        GL.End();
    }

    private void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    //�i�V�[���r���[�p�̕\���j�ق��̃X�N���v�g�ł�����Ă镔���͂��邪�c
    void OnDrawGizmos()
    {
        if (!visualizeColliders || !Application.isEditor) return;

        // �V�[�����̂��ׂẴR���C�_�[���擾
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();

        foreach (Collider2D collider in allColliders)
        {
            if (!collider.enabled || !collider.gameObject.activeInHierarchy) continue;

            // �g���K�[�̏ꍇ�͐F��ς���
            Color color = collider.isTrigger ? triggerColor :
                         (collider is BoxCollider2D) ? boxColliderColor :
                         (collider is CircleCollider2D) ? circleColliderColor :
                         new Color(1, 0, 1, 0.3f);

            Gizmos.color = color;

            // �R���C�_�[�^�C�v�ɉ������`��
            if (collider is BoxCollider2D)
            {
                BoxCollider2D boxCollider = collider as BoxCollider2D;
                Vector2 position = collider.transform.TransformPoint(boxCollider.offset);
                Vector2 size = boxCollider.size;
                size.x *= collider.transform.lossyScale.x;
                size.y *= collider.transform.lossyScale.y;

                Matrix4x4 oldMatrix = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(position, collider.transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, size);
                Gizmos.matrix = oldMatrix;
            }
            else if (collider is CircleCollider2D)
            {
                CircleCollider2D circleCollider = collider as CircleCollider2D;
                Vector2 position = collider.transform.TransformPoint(circleCollider.offset);
                float radius = circleCollider.radius * Mathf.Max(
                    collider.transform.lossyScale.x,
                    collider.transform.lossyScale.y
                );
                Gizmos.DrawWireSphere(position, radius);
            }
            // ���̃R���C�_�[�^�C�v�����l�Ɏ���
        }
    }
}
