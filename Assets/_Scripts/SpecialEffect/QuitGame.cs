using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public class QuitGame : MonoBehaviour
{
    public void QuitGameFunction()
    {
        if (Application.isEditor)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
        else
        {
            Application.Quit();
        }
    }

    private async void Update()
    {

        if (GlobalInput.GetKeyDown(GlobalKeyCode.ESCAPE) || Input.GetKeyDown(KeyCode.Escape))
        {
            OpenNewApplication.isCanNotClose = false;
            await UniTask.Delay(100); //保险起见
            QuitGameFunction();
        }
    }
}


