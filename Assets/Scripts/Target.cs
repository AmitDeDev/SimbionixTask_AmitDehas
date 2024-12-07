using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private UIController uiController;

    #region General Properties

    private Renderer _renderer;
    private Color _originalColor; 
    public int health = 2; 
    public int scoreValue = 10; 

    #endregion

    [Header("Audio Settings")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField, Range(0f, 1f)] private float hitSoundVolume = 0.8f;
    [SerializeField] private AudioClip destroyedSound;
    [SerializeField, Range(0f, 1f)] private float destroyedSoundVolume = 1f;
    private AudioSource audioSource;
    
    [Header("Movement Settings")]
    public Vector3 movementDirection = Vector3.up; 
    public float amplitude = 2f; 
    public float frequency = 1f; 
    public float speed = 2f; 
    private Vector3 _startPosition;

    private bool isDestroyed; 

    private void Start()
    {
        _startPosition = transform.position;
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false; 
    }

    private void Update()
    {
        MoveInCurve();
    }

    private void MoveInCurve()
    {
        float offset = Mathf.Sin(Time.time * frequency) * amplitude;
        Vector3 curveMotion = movementDirection * offset;
        Vector3 forwardMotion = transform.forward * speed * Time.deltaTime;
        
        transform.position = _startPosition + curveMotion + forwardMotion;
        _startPosition += forwardMotion; 
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Projectile") && !isDestroyed)
        {
            Projectile projectile = other.gameObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                health -= projectile.damage; 
            }

            _renderer.material.color = Color.red; 
            Invoke(nameof(ResetColor), 0.5f);
            
            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound, hitSoundVolume);
            }
            
            if (health <= 0)
            {
                DestroyTarget();
            }
        }
    }

    private void DestroyTarget()
    {
        if (isDestroyed) return; 
        isDestroyed = true;
        
        if (destroyedSound != null)
        {
            audioSource.PlayOneShot(destroyedSound, destroyedSoundVolume);
        }
        
        uiController.UpdateScore(uiController.Score + scoreValue);
        uiController.UpdateTargetsKilled(uiController.TargetsKilled + 1);
        
        _renderer.material.color = Color.yellow; 
        Destroy(gameObject, 1.5f);
    }

    private void ResetColor()
    {
        _renderer.material.color = _originalColor;
    }
}
