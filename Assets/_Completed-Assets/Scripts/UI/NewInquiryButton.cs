using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewInquiryButton : MonoBehaviour
{
    [SerializeField] private Button newinquiryButton;

    private void Start()
    {
        newinquiryButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.NewInquiryScene);
    }
}
