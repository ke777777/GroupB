using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContactUSButton : MonoBehaviour
{
    [SerializeField] private Button contactusButton;

    private void Start()
    {
        contactusButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.ContactusScene);
    }
}
