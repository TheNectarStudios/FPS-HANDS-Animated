using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Image healthBarFill; // Assign this in Inspector (Image with Fill Amount)
    public Camera playerCamera; // Assign Main Camera
    public CharacterController playerController; // Assign Player Movement Controller

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHealthUI();
        Debug.Log("Player took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (isDead) return;

        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
        Debug.Log("Player healed! Current health: " + currentHealth);
    }

    void UpdateHealthUI()
    {
        if (healthBarFill)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player Died!");
        
        // Disable movement
        if (playerController)
            playerController.enabled = false;

        // Disable all child objects of the camera
        DisableCameraChildren();

        // Simulate camera falling
        StartCoroutine(CameraFallEffect());

        // Restart game after 3 seconds
        Invoke(nameof(RestartGame), 3f);
    }

    void DisableCameraChildren()
    {
        if (playerCamera != null)
        {
            foreach (Transform child in playerCamera.transform)
            {
                child.gameObject.SetActive(false);
            }
            Debug.Log("All camera child objects disabled.");
        }
    }

    System.Collections.IEnumerator CameraFallEffect()
    {
        float duration = 1.5f;
        float elapsed = 0f;
        Quaternion startRotation = playerCamera.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(90, startRotation.eulerAngles.y, 0);

        while (elapsed < duration)
        {
            playerCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        playerCamera.transform.rotation = endRotation;
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
