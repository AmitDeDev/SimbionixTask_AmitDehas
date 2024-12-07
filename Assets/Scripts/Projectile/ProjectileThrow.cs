using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class ProjectileThrow : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private Rigidbody[] projectilePrefabs;
    private int currentProjectileIndex = 0;

    [SerializeField, Range(0.0f, 50.0f)] private float force;

    [SerializeField] private Transform StartPosition;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField, Range(0f, 1f)] private float fireSoundVolume = 0.8f;
    [SerializeField] private AudioClip reloadSound;

    [Header("Fire Rate Settings")]
    [SerializeField, Range(1f, 5f)] private float fireRate = 1f;
    private float lastFireTime;

    private AudioSource audioSource;

    [Header("Trajectory Settings")]
    [SerializeField] private TrajectoryPredictor trajectoryPredictor;
    [SerializeField] private UnknownComponent unknownComponent; 

    public InputAction fire;

    private void OnEnable()
    {
        if (StartPosition == null)
            StartPosition = transform;

        fire.Enable();
        fire.performed += TryFire;

        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        fire.Disable();
        fire.performed -= TryFire;
    }

    private void Update()
    {
        Predict();

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SwitchProjectile();
        }
    }

    private void Predict()
    {
        var currentProjectile = projectilePrefabs[currentProjectileIndex];
        Rigidbody r = currentProjectile.GetComponent<Rigidbody>();
        
        Vector3 velocity = StartPosition.forward * (force / r.mass);
        var trajectory = TrajectoryCalculator.CalculateTrajectory(
            StartPosition.position,
            velocity,
            Physics.gravity,
            trajectoryPredictor.maxPoints
        );

        // Render trajectory using UnknownComponent
        if (unknownComponent != null)
        {
            unknownComponent.UnknownMethod(trajectory);
        }

        // Render trajectory using TrajectoryPredictor (optional, commented out)
        /*
        trajectoryPredictor.PredictTrajectory(new ProjectileProperties
        {
            direction = StartPosition.forward,
            initialPosition = StartPosition.position,
            initialSpeed = force,
            mass = r.mass,
            drag = r.drag
        });
        */
    }

    private void TryFire(InputAction.CallbackContext ctx)
    {
        if (Time.time - lastFireTime >= fireRate)
        {
            FireProjectile();
        }
        else
        {
            Debug.Log("Cannon reloading...");
        }
    }

    private void FireProjectile()
    {
        lastFireTime = Time.time;
        
        var selectedProjectile = projectilePrefabs[currentProjectileIndex];
        Rigidbody thrownObject = Instantiate(selectedProjectile, StartPosition.position, StartPosition.rotation);
        thrownObject.AddForce(StartPosition.forward * force, ForceMode.Impulse);
        
        if (fireSound != null)
        {
            audioSource.PlayOneShot(fireSound, fireSoundVolume);
        }
        
        if (reloadSound != null)
        {
            audioSource.clip = reloadSound;
            audioSource.PlayDelayed(fireSound.length);
        }
        
        Destroy(thrownObject.gameObject, 5);
    }

    private void SwitchProjectile()
    {
        currentProjectileIndex = (currentProjectileIndex + 1) % projectilePrefabs.Length;
        Debug.Log($"Switched to: {projectilePrefabs[currentProjectileIndex].name}");
    }
}
