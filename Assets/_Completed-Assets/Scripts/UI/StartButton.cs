using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StartButton : MonoBehaviour
{
    [SerializeField] private Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        SceneManager.LoadScene(SceneNames.HomeScene);
    }
}
