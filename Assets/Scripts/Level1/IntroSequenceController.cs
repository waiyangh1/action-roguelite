using UnityEngine;
using System.Collections;
using static SceneController;

public class IntroSequenceController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform walkToPoint;
    [SerializeField] private Transform gateDoor; // The moving part of the gate

    [Header("Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float gateOpenOffset = 4f; // How far the gate moves up/side
    [SerializeField] private float gateSpeed = 2f;

    [Header("Audio")]
    [SerializeField] private SoundDataSO gateSound;

    private Vector3 _gateClosedPos;
    private Vector3 _gateOpenPos;
    private MonoBehaviour _playerMovementScript;

    private void Awake()
    {
        // 1. Initial State: Snap player to start, find movement script
        player.transform.position = startPoint.position;
        _gateClosedPos = gateDoor.localPosition;
        _gateOpenPos = _gateClosedPos + new Vector3(0, gateOpenOffset, 0); // Adjust axis as needed

        // Disable player control immediately
        _playerMovementScript = player.GetComponent<MonoBehaviour>(); // Replace with your actual PlayerMove script type
        if (_playerMovementScript != null) _playerMovementScript.enabled = false;
    }

    private void OnEnable() => EventBus.Subscribe<StartLevelSequenceEvent>(e => StartCoroutine(PlaySequence()));

    private IEnumerator PlaySequence()
    {
        // A small delay to let the scene fade-in finish
        yield return new WaitForSeconds(0.2f);

        // 2. Open Gate
        if (gateSound != null) EventBus.Publish(new PlaySoundEvent(gateSound));
        yield return MoveTransform(gateDoor, _gateOpenPos, gateSpeed);

        // 3. Move Player to Point
        yield return MovePlayerToPoint();

        // 4. Close Gate
        if (gateSound != null) EventBus.Publish(new PlaySoundEvent(gateSound));
        yield return MoveTransform(gateDoor, _gateClosedPos, gateSpeed * 1.5f); // Close faster for impact

        // 5. Hand over control to player
        if (_playerMovementScript != null) _playerMovementScript.enabled = true;

        Debug.Log("Sequence Complete: Player has control.");
    }

    // High performance Lerp for low-end devices
    private IEnumerator MoveTransform(Transform target, Vector3 destination, float speed)
    {
        while (Vector3.Distance(target.localPosition, destination) > 0.01f)
        {
            target.localPosition = Vector3.MoveTowards(target.localPosition, destination, speed * Time.deltaTime);
            yield return null;
        }
        target.localPosition = destination;
    }

    private IEnumerator MovePlayerToPoint()
    {
        while (Vector3.Distance(player.transform.position, walkToPoint.position) > 0.05f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, walkToPoint.position, walkSpeed * Time.deltaTime);
            // Optional: If you have an animator, set a 'Speed' float here
            // playerAnimator.SetFloat("Speed", 1f); 
            yield return null;
        }
        // playerAnimator.SetFloat("Speed", 0f);
    }
}