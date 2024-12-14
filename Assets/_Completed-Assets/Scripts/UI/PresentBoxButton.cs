using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PresentBoxButton : MonoBehaviour
{
    [SerializeField] private Button presentBoxButton;

    private void Start()
    {
        presentBoxButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.PresentScene);
    }
}
