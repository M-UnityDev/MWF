using System; 
using UnityEngine; 
using UnityEngine.InputSystem; 
using Unity.Burst; 
using UnityEngine.SceneManagement;
namespace Game.Input
{
    [BurstCompile] public class InputHandler : MonoBehaviour
    {
        public static event Action<InputActionMap> OnMapChanged;
        private static PlayerControl _input;

        private void Awake()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            _input = new PlayerControl();
            ToggleActionMap(_input.Player);
        }
        private void OnSceneUnloaded(Scene current)
        {  
            _input.Disable();
        }

        public static void ToggleActionMap(InputActionMap map)
        {
            if (map.enabled)
                return;
            
            _input.Disable();
            map.Enable();
            OnMapChanged?.Invoke(map);
        }
    }
}