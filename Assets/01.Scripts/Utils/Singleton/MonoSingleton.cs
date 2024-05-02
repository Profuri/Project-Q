using UnityEngine;

namespace Singleton
{
    public class MonoSingleton<T> : ExtendedMono where T : MonoBehaviour
    {
        private static bool _shutDown = false;
        private static object _locker = new object();
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_shutDown)
                {
                    Debug.LogWarning($"Instance \"{typeof(T)}\" already destroyed.");
                    _instance = null;
                    return null;
                }

                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (_instance == null)
                        {
                            _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                            _instance.transform.SetParent(GameManager.Instance.transform);
                            Debug.Log($"Create new singleton object \"{typeof(T)}\"");
                        }
                        else
                        {
                            if (_instance.transform.parent != GameManager.Instance.transform)
                            {
                                _instance.transform.SetParent(GameManager.Instance.transform);
                            }
                        }
                    }

                    return _instance;
                }
            }
        }

        private void OnApplicationQuit()
        {
            _shutDown = true;
        }

        private void OnDestroy()
        {
            _shutDown = true;
        }
    }
}