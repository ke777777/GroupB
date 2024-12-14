using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemListButton : MonoBehaviour
{
    [SerializeField] private Button itemListButton;

    private void Start()
    {
        itemListButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.ItemlistScene);
    }
}
