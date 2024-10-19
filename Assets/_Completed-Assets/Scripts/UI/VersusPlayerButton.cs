using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class VersusPlayerButton : MonoBehaviour
{
   [SerializeField]private Button versusPlayerButton;

    void Start()
    {
        versusPlayerButton.onClick.AddListener(OnClicked);
    }
 private void OnClicked()
    {
        SceneManager.LoadScene(SceneNames.CompleteGameScene);
    }
}
