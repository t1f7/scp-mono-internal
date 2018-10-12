using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;



namespace Cheat
{
    public class Injector
    {
        static UnityEngine.GameObject gameObject;

        public static void Inject()
        {
            gameObject = new UnityEngine.GameObject();
            gameObject.AddComponent<Cheat>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }

        public static void Unload()
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
