using UnityEngine;

public class DirtDigger : MonoBehaviour
{
    [Header("References")]
    public RenderTexture maskRT;        // DirtMaskRT 연결
    public Camera digCamera;             // 메인 카메라 연결
    
    [Header("Dig Settings")]
    public float brushSize = 0.05f;      // UV 공간에서 브러시 크기 (0~1)
    public Color digColor = Color.black; // 깎을 색 (검은색 = 마스크 0)
    
    [Header("Debug Mode (Mac)")]
    public bool useMouseDebug = true;    // VR 없이 마우스로 테스트
    
    private Material brushMat;
    
    void Start()
    {
        // 마스크를 흰색으로 초기화 (안 깎인 상태)
        ClearMask();
        
        // 브러시 그릴 때 쓸 임시 머티리얼
        brushMat = new Material(Shader.Find("Hidden/Internal-Colored"));
    }
    
    void Update()
    {
        if (useMouseDebug && Input.GetMouseButton(0))
        {
            TryDigWithMouse();
        }
    }
    
    void TryDigWithMouse()
    {
        Ray ray = digCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            // 흙 블록을 맞췄을 때만 작동
            if (hit.collider.gameObject == this.gameObject)
            {
                DigAtUV(hit.textureCoord);
            }
        }
    }
    
    // UV 좌표에 검은색 원을 그려서 마스크에 구멍을 냄
    public void DigAtUV(Vector2 uv)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = maskRT;
        
        GL.PushMatrix();
        GL.LoadOrtho();  // 0~1 좌표계로 그리기
        
        brushMat.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        GL.Color(digColor);
        
        // 원 모양 그리기 (간단한 다각형 근사)
        int segments = 24;
        for (int i = 0; i < segments; i++)
        {
            float a1 = (i / (float)segments) * Mathf.PI * 2f;
            float a2 = ((i + 1) / (float)segments) * Mathf.PI * 2f;
            
            GL.Vertex3(uv.x, uv.y, 0);
            GL.Vertex3(uv.x + Mathf.Cos(a1) * brushSize, uv.y + Mathf.Sin(a1) * brushSize, 0);
            GL.Vertex3(uv.x + Mathf.Cos(a2) * brushSize, uv.y + Mathf.Sin(a2) * brushSize, 0);
        }
        
        GL.End();
        GL.PopMatrix();
        
        RenderTexture.active = prev;
    }
    
    // 마스크를 흰색으로 초기화 (게임 시작 시)
    void ClearMask()
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = maskRT;
        GL.Clear(true, true, Color.white);
        RenderTexture.active = prev;
    }
}