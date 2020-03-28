using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    public static DDOL Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        SceneManager.sceneLoaded += DDOL_SceneInit;
        DontDestroyOnLoad(this);
    }

    private void DDOL_SceneInit(Scene scene, LoadSceneMode mode)
    {
        DontDestroyOnLoad(this);
    }
}
