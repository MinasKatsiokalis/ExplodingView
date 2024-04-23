using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleScenesLoader : MonoBehaviour
{
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    } 
}
