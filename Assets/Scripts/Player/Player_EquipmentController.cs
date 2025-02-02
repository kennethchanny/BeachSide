using System.Collections;
using UnityEngine;

public class Player_EquipmentController : MonoBehaviour
{
    #region References
    public GameObject equipmentHolder;
    private Player_InspectController inspectorController;
    private Player_DigController digController;
    private Player_DetectController detectController;
    private Vector3 hiddenPosition;
    private Vector3 visiblePosition;
    #endregion

    #region Variables
    private bool isSwapping = false;
    private bool isEquipmentHidden = false; // New flag to track hidden state
    private EquippedTool lastEquippedTool = EquippedTool.None;
    private Coroutine equipmentMovementCoroutine;

    public float equipmentHideAndRevealSpeed = 15f;
    public float equipmentHideDist = 12f;

    private enum EquippedTool { None, Shovel, MetalDetector }
    #endregion

    void Start()
    {
        InitializePlayerReferences();
        InitializeEquipmentPositions();
    }

    void Update()
    {
        HandleEquipment();
    }

    #region InitFuncs
    private void InitializePlayerReferences()
    {
        inspectorController = GetComponent<Player_InspectController>();
        digController = GetComponent<Player_DigController>();
        detectController = GetComponent<Player_DetectController>();
    }

    private void InitializeEquipmentPositions()
    {
        hiddenPosition = equipmentHolder.transform.position + Vector3.down * equipmentHideDist;
        visiblePosition = equipmentHolder.transform.position;
    }
    #endregion

    #region HidingAndRevealingEquipment

    private void HandleEquipment()
    {
        if (inspectorController == null || digController == null || detectController == null)
            return;

        if (inspectorController.isInspecting)
        {
            StoreLastEquippedTool();
            SetEquipmentState(false, false);
            HideEquipment();
        }
        else
        {
            RestoreLastEquippedTool();
            RevealEquipment();
        }

        HandleEquipmentSwapping();
    }

    private void StoreLastEquippedTool()
    {
        if (digController.isEquipped)
            lastEquippedTool = EquippedTool.Shovel;
        else if (detectController.isEquipped)
            lastEquippedTool = EquippedTool.MetalDetector;
    }

    private void RestoreLastEquippedTool()
    {
        if (lastEquippedTool == EquippedTool.Shovel)
            SetEquipmentState(true, false);
        else if (lastEquippedTool == EquippedTool.MetalDetector)
            SetEquipmentState(false, true);
    }

    private void SetEquipmentState(bool digEnabled, bool detectEnabled)
    {
        digController.isEquipped = digEnabled;
        detectController.isEquipped = detectEnabled;
    }

    private void HideEquipment()
    {
        if (equipmentMovementCoroutine != null)
            StopCoroutine(equipmentMovementCoroutine);

        isEquipmentHidden = true; // Mark as hidden (disable swapping)
        equipmentMovementCoroutine = StartCoroutine(MoveEquipment(hiddenPosition, false));
    }

    private void RevealEquipment()
    {
        if (equipmentMovementCoroutine != null)
            StopCoroutine(equipmentMovementCoroutine);

        equipmentMovementCoroutine = StartCoroutine(MoveEquipment(visiblePosition, true));
    }

    private IEnumerator MoveEquipment(Vector3 targetPosition, bool isRevealing)
    {
        while (Vector3.Distance(equipmentHolder.transform.position, targetPosition) > 0.01f)
        {
            equipmentHolder.transform.position = Vector3.Lerp(
                equipmentHolder.transform.position,
                targetPosition,
                Time.deltaTime * equipmentHideAndRevealSpeed
            );
            yield return null;
        }

        equipmentHolder.transform.position = targetPosition; // Ensure final position snap

        if (isRevealing)
            isEquipmentHidden = false; // Re-enable swapping when revealed
    }

    #endregion

    #region SwappingEquipment
    private void HandleEquipmentSwapping()
    {
        if (isSwapping || isEquipmentHidden) return; // Prevent swapping when hidden

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isSwapping = true;
            lastEquippedTool = EquippedTool.MetalDetector;
            StartCoroutine(SwapToMetalDetector(0.3f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isSwapping = true;
            lastEquippedTool = EquippedTool.Shovel;
            StartCoroutine(SwapToShovel(0.3f));
        }
    }

    private IEnumerator SwapToShovel(float delay)
    {
        detectController.metalDetectorAnimator.SetBool("isEquipped", false);
        detectController.isEquipped = false;
        yield return new WaitForSeconds(delay);

        digController.shovelAnimator.SetBool("isEquipped", true);
        yield return new WaitForSeconds(delay);
        digController.isEquipped = true;

        isSwapping = false;
    }

    private IEnumerator SwapToMetalDetector(float delay)
    {
        digController.shovelAnimator.SetBool("isEquipped", false);
        digController.isEquipped = false;
        yield return new WaitForSeconds(delay);

        detectController.metalDetectorAnimator.SetBool("isEquipped", true);
        yield return new WaitForSeconds(delay);
        detectController.isEquipped = true;

        isSwapping = false;
    }
    #endregion
}
