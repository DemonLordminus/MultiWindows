using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShowConsole : MonoBehaviour
{
    [SerializeField] private IngameDebugConsole.DebugLogManager _console;
    [SerializeField] private Canvas _canvas;
    

    private bool isOn = false;

    private bool isPoP = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await UniTask.Yield();
        // _console.gameObject.SetActive(false);
        _console.enabled = false;
        _canvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalInput.GetKeyDown(GlobalKeyCode.F1) || Input.GetKeyDown(KeyCode.F1))
        {
            if (isOn)
            {
                _console.HideLogWindow();
            }
            else
            {
                _console.ShowLogWindow();
            }

            isOn = !isOn;
        }
        if (GlobalInput.GetKeyDown(GlobalKeyCode.F2) || Input.GetKeyDown(KeyCode.F2))
        {
            if (isPoP)
            {
                // _console.gameObject.SetActive(false);
                _console.enabled = false;
                _canvas.enabled = false;
            }
            else
            {
                // _console.gameObject.SetActive(true);
                _console.enabled = true;
                _canvas.enabled = true;
            }

            isPoP = !isPoP;
        }
    }
}
