using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VersusPlayerButton : MonoBehaviour
{
    [SerializeField] private Button versusPlayerButton;

    private void Start()
    {
        versusplayerButton.onClick.AddListener(OnStartButtonClicked);
    }
    private void OnClicked()
    {
        SceneManager.LoadScene(SceneNames.LobbyScene);
    }
}
