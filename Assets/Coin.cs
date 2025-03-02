using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1; // Value of the coin
    public float rotationSpeed = 100f; // Rotation speed (adjustable in Inspector)

    private void Update()
    {
        // Rotate around the local Z-axis (since the coin is rotated 90 degrees on X)
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure only the player can collect it
        {
            GameManager.Instance.AddScore(coinValue); // Update score
            Destroy(gameObject); // Remove coin from scene
        }
    }
}
