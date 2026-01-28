using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // [게임 시작] 버튼에 연결
    public void OnClickStartGame()
    {
        // 작업한 메인 게임 씬 이름으로 이동
        SceneManager.LoadScene("GameScene");
    }

    // [게임 종료] 버튼에 연결
    public void OnClickExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 끄기
#else
        Application.Quit(); // 실제 빌드된 게임에서 끄기
#endif
    }
}
