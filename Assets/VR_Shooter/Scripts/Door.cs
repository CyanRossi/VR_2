//using UnityEngine;
//using UnityEngine.Events;
////using DG.Tweening;
//public class Door : MonoBehaviour
//{
//    public enum DoorType { Rotate, Move }

//    [Header("Door Settings")]
//    public Transform door;
//    public DoorType doorType = DoorType.Rotate;

//    [Header("Movement Settings")]
//    public Vector3 moveOffset = new Vector3(0, 5, 0);
//    public Vector3 rotateOffset = new Vector3(0, 90, 0);
//    public float duration = 1f;

//    [Header("Unity Events")]
//    public UnityEvent onTriggerEnter;
//    public UnityEvent onTriggerExit;

//    private Vector3 initialPosition;
//    private Quaternion initialRotation;
//    private bool isOpen = false;

//    private void Start()
//    {
//        if (door == null)
//        {
//            Debug.LogError("Door reference not assigned.");
//            enabled = false;
//            return;
//        }

//        initialPosition = door.localPosition;
//        initialRotation = door.localRotation;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!isOpen)
//        {
//            OpenDoor();
//            onTriggerEnter?.Invoke();
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (isOpen)
//        {
//            CloseDoor();
//            onTriggerExit?.Invoke();
//        }
//    }

//    private void OpenDoor()
//    {
//        isOpen = true;
//        if (doorType == DoorType.Move)
//        {
//            door.DOLocalMove(initialPosition + moveOffset, duration).SetEase(Ease.OutCubic);
//        }
//        else if (doorType == DoorType.Rotate)
//        {
//            door.DOLocalRotate(initialRotation.eulerAngles + rotateOffset, duration).SetEase(Ease.OutCubic);
//        }
//    }

//    private void CloseDoor()
//    {
//        isOpen = false;
//        if (doorType == DoorType.Move)
//        {
//            door.DOLocalMove(initialPosition, duration).SetEase(Ease.InCubic);
//        }
//        else if (doorType == DoorType.Rotate)
//        {
//            door.DOLocalRotate(initialRotation.eulerAngles, duration).SetEase(Ease.InCubic);
//        }
//    }
//}
