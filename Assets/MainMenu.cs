using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "SampleScene";
    
    public void OnStartGame()
    {
        Debug.Log("게임 시작!");
        SceneManager.LoadScene(gameSceneName);
    }
    
    public void OnOpenCatalog()
    {
        Debug.Log("도감 열기 (아직 구현 안 됨)");
        // TODO: 도감 화면 열기
    }
    
    public void OnQuitGame()
    {
        Debug.Log("게임 종료");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}