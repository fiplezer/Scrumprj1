using UnityEngine;

public class PickUpRay : MonoBehaviour
{
    [SerializeField] private LayerMask pickUpLayer;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float kracht = 10f;
    [SerializeField] private float pickUpRange = 5f;
    [SerializeField] private Transform hand;

    private Rigidbody currentObjectRb;
    private Collider currentObjectCol;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray pickUpRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(pickUpRay, out RaycastHit hitInfo, pickUpRange, pickUpLayer))
            {
                // Als je al iets vast hebt, laat het eerst los
                if (currentObjectRb)
                {
                    DropObject();
                }

                PickUpObject(hitInfo.rigidbody, hitInfo.collider);
            }
            else
            {
                // Niks geraakt, laat huidig object los (als vast)
                if (currentObjectRb)
                {
                    DropObject();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentObjectRb)
            {
                ThrowObject();
            }
        }
    }

    private void FixedUpdate()
    {
        if (currentObjectRb)
        {
            currentObjectRb.MovePosition(hand.position);
            currentObjectRb.MoveRotation(hand.rotation);
        }
    }

    private void PickUpObject(Rigidbody rb, Collider col)
    {
        currentObjectRb = rb;
        currentObjectCol = col;

        currentObjectRb.isKinematic = true;
        currentObjectRb.interpolation = RigidbodyInterpolation.Interpolate;
        currentObjectCol.enabled = false;
    }

    private void DropObject()
    {
        currentObjectRb.isKinematic = false;
        currentObjectRb.interpolation = RigidbodyInterpolation.None;
        currentObjectCol.enabled = true;

        currentObjectRb = null;
        currentObjectCol = null;
    }

    private void ThrowObject()
    {
        currentObjectRb.isKinematic = false;
        currentObjectRb.interpolation = RigidbodyInterpolation.None;
        currentObjectCol.enabled = true;

        currentObjectRb.AddForce(playerCamera.transform.forward * kracht, ForceMode.Impulse);

        currentObjectRb = null;
        currentObjectCol = null;
    }
}
