using UnityEngine; 
using UnityEngine.SceneManagement; 

public class PlayerDeath : MonoBehaviour 
{ 
    private void OnTriggerEnter2D(Collider2D other) 
    { 
        if (other.CompareTag("Hazard")) 
        { 
            Die(); 
        } 
    } 

    void Die() 
    { 
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(currentScene.name); 
    } 
}