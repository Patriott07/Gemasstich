using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Restart
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
