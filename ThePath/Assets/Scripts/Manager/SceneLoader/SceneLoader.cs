using Com.IsartDigital.F2P.LoadScene;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public SceneEnum sceneToLoad;

    public void LoadScene()
    {
        LoadingScreenManager.Instance.LoadScene(sceneToLoad);
    }
}