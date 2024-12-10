using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContactBackButton : MonoBehaviour
{
    [SerializeField] private Button contactbackButton;

    private void Start()
    {
        contactbackButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.ContactusScene);
    }
}
