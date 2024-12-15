using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ExitLobby : MonoBehaviour
{
    [SerializeField] private Button exitButton;

    void Start()
    {
        if (exitButton == null)
        {
            return;
        }

        exitButton.onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        SceneManager.LoadScene(SceneNames.HomeScene);
    }
}

