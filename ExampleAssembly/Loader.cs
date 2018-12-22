using UnityEngine;

namespace Cheat
{
    public class Injector
    {
        private static GameObject _gameObject;

        public static void Inject()
        {
            _gameObject = new GameObject();
            _gameObject.AddComponent<Cheat>();
            Object.DontDestroyOnLoad(_gameObject);
        }

        public static void Unload()
        {
            Object.Destroy(_gameObject);
        }
    }
}