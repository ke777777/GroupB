using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InformationButton : MonoBehaviour
{
    [SerializeField] private Button informationButton;

    private void Start()
    {
        informationButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.InformationScene);
    }
}
