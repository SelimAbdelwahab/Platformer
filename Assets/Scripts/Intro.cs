using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    private Button[] _button;
    // Start is called before the first frame update
    void Awake()
    {
        _button = GameObject.FindObjectsOfType<Button>();

        foreach (Button button in _button)
        {
            Button btn = button;
            
            button.onClick.AddListener(delegate {
                SwitchScene(btn); 
            });
        }
    }

    void SwitchScene(Button btn)
    {
        SceneManager.LoadScene(btn.gameObject.name);
    }
}
