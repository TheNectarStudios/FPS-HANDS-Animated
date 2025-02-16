using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveGames
{
    public class ItemChange : MonoBehaviour
    {
        [Header("Item Change")]
        [SerializeField] public Animator ani;
        [SerializeField] Image ItemCanvasLogo;
        [SerializeField] bool LoopItems = true;
        [SerializeField, Tooltip("You can add your new item here.")] GameObject[] Items;
        [SerializeField, Tooltip("These logos must have the same order as the items.")] Sprite[] ItemLogos;
        [SerializeField] GameObject Medikit; // Separate Medikit GameObject
        [SerializeField] int ItemIdInt;
        int MaxItems;
        int ChangeItemInt;
        [HideInInspector] public bool DefiniteHide;
        bool ItemChangeLogo;
        bool usingMedikit = false; // Track if the player is using the medikit

        private BulletManager bulletManager; // Reference to BulletManager

        private void Start()
        {
            if (ani == null && GetComponent<Animator>()) ani = GetComponent<Animator>();

            // Ensure arrays are not empty
            if (Items.Length == 0 || ItemLogos.Length == 0)
            {
                Debug.LogError("Items or ItemLogos array is empty! Assign them in the Inspector.");
                return;
            }

            if (ItemIdInt >= Items.Length || ItemIdInt >= ItemLogos.Length)
            {
                Debug.Log("ItemIdInt is out of range! Resetting to 0.");
                ItemIdInt = 0;
            }

            Color OpacityColor = ItemCanvasLogo.color;
            OpacityColor.a = 0;
            ItemCanvasLogo.color = OpacityColor;

            ItemChangeLogo = false;
            DefiniteHide = false;
            ChangeItemInt = ItemIdInt;
            ItemCanvasLogo.sprite = ItemLogos[ItemIdInt];
            MaxItems = Items.Length - 1;
            StartCoroutine(ItemChangeObject());

            bulletManager = FindObjectOfType<BulletManager>(); // Find BulletManager

            if (Medikit != null) Medikit.SetActive(false); // Ensure medikit is disabled at start
        }

        private void Update()
        {
            // Prevent switching while using medikit or reloading
            if (usingMedikit || (bulletManager != null && bulletManager.isReloading))
                return;

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                ItemIdInt++;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                ItemIdInt--;
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                if (ani.GetBool("Hide")) Hide(false);
                else Hide(true);
            }

            if (ItemIdInt < 0) ItemIdInt = LoopItems ? MaxItems : 0;
            if (ItemIdInt > MaxItems) ItemIdInt = LoopItems ? 0 : MaxItems;

            if (ItemIdInt != ChangeItemInt)
            {
                ChangeItemInt = ItemIdInt;
                StartCoroutine(ItemChangeObject());
            }

            // Press B to use Medikit
            if (Input.GetKeyDown(KeyCode.B))
            {
                StartCoroutine(UseMedikit());
            }
        }

        public void Hide(bool Hide)
        {
            DefiniteHide = Hide;
            ani.SetBool("Hide", Hide);
        }

        IEnumerator ItemChangeObject()
        {
            if (!DefiniteHide) ani.SetBool("Hide", true);
            yield return new WaitForSeconds(0.3f);
            for (int i = 0; i < (MaxItems + 1); i++)
            {
                Items[i].SetActive(false);
            }
            Items[ItemIdInt].SetActive(true);
            if (!ItemChangeLogo) StartCoroutine(ItemLogoChange());

            if (!DefiniteHide) ani.SetBool("Hide", false);
        }

        IEnumerator ItemLogoChange()
        {
            ItemChangeLogo = true;
            yield return new WaitForSeconds(0.5f);
            ItemCanvasLogo.sprite = ItemLogos[ItemIdInt];
            yield return new WaitForSeconds(0.1f);
            ItemChangeLogo = false;
        }

        private void FixedUpdate()
        {
            Color OpacityColor = ItemCanvasLogo.color;
            if (ItemChangeLogo)
            {
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 0, 20 * Time.deltaTime);
            }
            else
            {
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 1, 6 * Time.deltaTime);
            }
            ItemCanvasLogo.color = OpacityColor;
        }

        IEnumerator UseMedikit()
        {
            usingMedikit = true;

            // Hide the current item
            ani.SetBool("Hide", true);
            yield return new WaitForSeconds(0.3f);
            Items[ItemIdInt].SetActive(false);

            // Show the Medikit
            Medikit.SetActive(true);
            ani.SetBool("Hide", false);
            yield return new WaitForSeconds(1.2f); // Show unhide animation
            yield return new WaitForSeconds(2.45f); // Wait for 1 min 20 sec (80 seconds)

            // Hide Medikit
            ani.SetBool("Hide", true);
            yield return new WaitForSeconds(0.3f);
            Medikit.SetActive(false);

            // Show previous item
            Items[ItemIdInt].SetActive(true);
            ani.SetBool("Hide", false);

            usingMedikit = false;
        }
    }
}
