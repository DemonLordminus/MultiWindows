using UnityEngine;

public class ShowConsole : MonoBehaviour
{
    [SerializeField] private IngameDebugConsole.DebugLogManager _console;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalInput.GetKeyDown(GlobalKeyCode.TAB))
        {
            _console.ShowLogWindow();
        }
        if (GlobalInput.GetKeyDown(GlobalKeyCode.B))
        {
            _console.HideLogWindow();
        }
    }
}
