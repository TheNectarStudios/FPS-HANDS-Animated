using UnityEngine;
using TMPro; // For UI bullet count display
using System.Collections;
public class BulletManager : MonoBehaviour
{
    public int maxBullets = 30; // Bullets per magazine
    public int currentBullets; // Tracks current bullets
    public int totalAmmo = 90; // Total reserve bullets
    public float reloadTime = 2.5f; // Reload duration

    public TextMeshProUGUI bulletText; // Assign in Inspector
    public Animator animator; // Assign in Inspector

    public bool isReloading = false;

    void Start()
    {
        currentBullets = maxBullets; // Initialize with full magazine
        UpdateBulletUI();
    }

    public bool CanShoot()
    {
        return currentBullets > 0 && !isReloading;
    }

    public void FireBullet()
    {
        if (currentBullets > 0)
        {
            currentBullets--; // Reduce bullet count
            UpdateBulletUI();

            if (currentBullets <= 0)
            {
                animator.SetBool("OutOfAmmo", true);
            }
        }
    }

    public IEnumerator Reload()
    {
        if (isReloading || totalAmmo <= 0 || currentBullets == maxBullets)
            yield break;

        isReloading = true;
        animator.SetBool("Reload", true);
        
        yield return new WaitForSeconds(reloadTime);

        int bulletsNeeded = maxBullets - currentBullets;
        int bulletsToReload = Mathf.Min(bulletsNeeded, totalAmmo);

        currentBullets += bulletsToReload;
        totalAmmo -= bulletsToReload;

        isReloading = false;
        animator.SetBool("Reload", false);
        UpdateBulletUI();
    }

    void UpdateBulletUI()
    {
        if (bulletText != null)
        {
            bulletText.text = currentBullets + " / " + totalAmmo;
        }
    }
}
