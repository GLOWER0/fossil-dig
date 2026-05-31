using UnityEngine;

public class CardTester : MonoBehaviour
{
    public FossilInfoCard card;
    
    [Header("Test Data")]
    public string testName = "티라노사우루스 이빨";
    public string testPeriod = "백악기 후기 · 약 6,800만 년 전";
    [TextArea(3, 6)]
    public string testDescription = "육식 공룡의 이빨로, 강력한 턱 힘으로 사냥감의 뼈까지 부술 수 있었습니다. 발견된 이빨의 길이는 약 15cm이며, 표면에 톱니 형태의 미세한 돌기가 보입니다.";
    public Vector3 spawnPosition = new Vector3(0, 1.5f, 2);
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            card.Show(testName, testPeriod, testDescription, spawnPosition);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            card.Hide();
        }
    }
}