using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Playtemp : MonoBehaviour
{
   [SerializeField]private Button PlayButton;

    void Start()
    {
        PlayButton.onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        SceneManager.LoadScene(SceneNames.CompleteGameScene);
    }
}

