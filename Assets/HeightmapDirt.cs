using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class HeightmapDirt : MonoBehaviour
{
    [Header("Mesh Settings")]
    public int resolution = 80;        // 격자 해상도 (80x80 = 6400 정점)
    public float size = 2f;            // 흙판 가로/세로 크기
    
    [Header("Dig Settings")]
    public float brushRadius = 0.15f;  // 브러시 반경 (월드 단위)
    public float digSpeed = 1.5f;       // 한 번 클릭당 파이는 깊이
    public float maxDepth = 0.6f;       // 최대 깊이
    
    [Header("Debug")]
    public bool useMouseDebug = true;
    public Camera digCamera;
    
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    private MeshCollider meshCollider;
    
    void Start()
    {
        GenerateGridMesh();
        meshCollider = GetComponent<MeshCollider>();
    }
    
    void Update()
    {
        if (useMouseDebug && Input.GetMouseButton(0))
        {
            TryDigWithMouse();
        }
    }
    
    void GenerateGridMesh()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // 정점 많을 때 필요
        
        int vertCount = (resolution + 1) * (resolution + 1);
        vertices = new Vector3[vertCount];
        uvs = new Vector2[vertCount];
        
        float step = size / resolution;
        float half = size / 2f;
        
        // 정점 생성
        for (int y = 0; y <= resolution; y++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                int i = y * (resolution + 1) + x;
                vertices[i] = new Vector3(x * step - half, 0, y * step - half);
                uvs[i] = new Vector2((float)x / resolution, (float)y / resolution);
            }
        }
        
        // 삼각형 인덱스
        triangles = new int[resolution * resolution * 6];
        int t = 0;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = y * (resolution + 1) + x;
                triangles[t++] = i;
                triangles[t++] = i + resolution + 1;
                triangles[t++] = i + 1;
                triangles[t++] = i + 1;
                triangles[t++] = i + resolution + 1;
                triangles[t++] = i + resolution + 2;
            }
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        
        GetComponent<MeshFilter>().mesh = mesh;
    }
    
    void TryDigWithMouse()
    {
        Ray ray = digCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                DigAtPosition(hit.point);
            }
        }
    }
    
    public void DigAtPosition(Vector3 worldPos)
    {
        // 월드 좌표를 로컬 좌표로 변환
        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        
        // 모든 정점을 확인해서 브러시 반경 안의 정점들 낮춤
        bool changed = false;
        for (int i = 0; i < vertices.Length; i++)
        {
            float distXZ = Mathf.Sqrt(
                (vertices[i].x - localPos.x) * (vertices[i].x - localPos.x) +
                (vertices[i].z - localPos.z) * (vertices[i].z - localPos.z)
            );
            
            if (distXZ < brushRadius)
            {
                // 가까울수록 더 많이 파임 (브러시 강도 분포)
                float falloff = 1f - (distXZ / brushRadius);
                float digAmount = digSpeed * falloff * Time.deltaTime;
                
                vertices[i].y -= digAmount;
                
                // 최대 깊이 제한
                if (vertices[i].y < -maxDepth)
                    vertices[i].y = -maxDepth;
                
                changed = true;
            }
        }
        
        if (changed)
        {
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            // 콜라이더도 업데이트 (안 하면 raycast가 옛 위치로 감)
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }
}