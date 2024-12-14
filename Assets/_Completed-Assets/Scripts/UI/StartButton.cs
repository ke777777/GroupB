using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        // PlayerPrefsにUserIDが保存されているか確認
        if (PlayerPrefs.HasKey("UserID"))
        {
            // UserIDが保存されている場合、HomeSceneに遷移
            SceneManager.LoadScene(SceneNames.HomeScene);
        }
        else
        {
            // UserIDが未保存の場合、UserID入力シーンに遷移
            SceneManager.LoadScene(SceneNames.UserIDScene);
        }

        // SceneManager.LoadScene(SceneNames.HomeScene);
    }
}
