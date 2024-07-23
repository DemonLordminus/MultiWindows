using UnityEngine;
using UnityEngine.SceneManagement;

public class MakeGameStartInInit : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        SceneManager.LoadScene("Init_New");
    }
}
