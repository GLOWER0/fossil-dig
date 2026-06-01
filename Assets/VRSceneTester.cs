using UnityEngine;

public class VRSceneTester : MonoBehaviour
{
    public FossilInfoCard infoCard;
    public FossilEncyclopedia encyclopedia;
    
    [Header("Test Data")]
    public string testName = "티라노사우루스 이빨";
    public string testPeriod = "백악기 후기 · 약 6,800만 년 전";
    [TextArea(3, 6)]
    public string testDescription = "육식 공룡의 이빨로, 강력한 턱 힘으로 사냥감의 뼈까지 부술 수 있었습니다. 발견된 이빨의 길이는 약 15cm이며, 표면에 톱니 형태의 미세한 돌기가 보입니다.";
    
    void Start()
    {
        // 시작 시 둘 다 숨김
        if (infoCard != null) infoCard.gameObject.SetActive(false);
        if (encyclopedia != null) encyclopedia.gameObject.SetActive(false);
    }
    
    void Update()
    {
        // Space → 카드 띄우기 (카메라 앞 1.5m)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
            spawnPos.y = 1.5f;
            infoCard.Show(testName, testPeriod, testDescription, spawnPos);
        }
        
        // E → 도감 열기 (카메라 앞 2m)
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            spawnPos.y = 1.5f;
            encyclopedia.transform.position = spawnPos;
            encyclopedia.Open();
        }
        
        // ESC → 둘 다 닫기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (infoCard.gameObject.activeSelf) infoCard.Hide();
            if (encyclopedia.gameObject.activeSelf) encyclopedia.Close();
        }
    }
}