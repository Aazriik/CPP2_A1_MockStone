using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T _instance;
    private static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                return null;
            }

            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    throw new System.Exception($"An instance of {typeof(T)} is needed in the scene, but there is none.");
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        _applicationIsQuitting = false;

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _applicationIsQuitting = true;
        }
    }
}