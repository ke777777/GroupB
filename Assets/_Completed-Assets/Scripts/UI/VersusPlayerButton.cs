using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VersusPlayerButton : MonoBehaviour
{
    [SerializeField] private Button versusplayerButton;

    private void Start()
    {
        versusplayerButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.GameScene);
    }
}
