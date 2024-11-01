using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
    maxXSpeed = 20f,
    maxStartXSpeed = 2f,
    startYSpeed = 10f,
    halfSize = 0.5f,
    maxYSpeed = 20f;

    [SerializeField] ParticleSystem impactParticlesSystem, startParticleSystem, trailParticleSystem;
    [SerializeField] int impactParticleEmission = 20,
                         startParticleEmission = 100;

    Vector2 position, velocity;
    [SerializeField]
    bool stopTheGame = false;

    void Awake() => gameObject.SetActive(false);

    public void StartNewGame()
    {
        if(stopTheGame == false)
        {
            position = Vector2.zero;
            UpdateVisualization();
            velocity.x = Random.Range(-maxStartXSpeed, maxStartXSpeed);
            velocity.y = -startYSpeed;
            gameObject.SetActive(true);
            startParticleSystem.Emit(startParticleEmission);
            SetTrailEmission(true);
            trailParticleSystem.Play();
        }
    }

    public void EndGame()
    {
        position.x = 0f;
        gameObject.SetActive(false);
        SetTrailEmission(false);
    }

    public void UpdateVisualization() => trailParticleSystem.transform.localPosition =
        transform.localPosition = new Vector3(position.x, 0f, position.y);

    public void Move() => position += Time.deltaTime * velocity;

    public void BounceX(float boundary)
    {
        float durationAfterBounce = (position.x - boundary) / velocity.x;
        position.x = 2 * boundary - position.x;
        velocity.x = -velocity.x;
        EmitBounceParticles(
            boundary,
            position.y - velocity.y * durationAfterBounce,
            boundary < 0f ? 90f : 270f
        );
    }

    public void BounceY(float boundary, float chargeMultiplier = 1f)
    {
        Debug.Log("chargeMultiplier: " + chargeMultiplier);
        float durationAfterBounce = (position.y - boundary) / velocity.y;
        position.y = 2 * boundary - position.y;
        float newSpeed = 0f;
        if (velocity.y < 0f)
        {
            newSpeed = Mathf.Clamp(startYSpeed * chargeMultiplier, -maxYSpeed, maxYSpeed);
        }
        else
        {
            newSpeed = Mathf.Clamp(-startYSpeed * chargeMultiplier, -maxYSpeed, maxYSpeed);
        }
       
        Debug.Log("newSpeed: "+ newSpeed);
        velocity.y = newSpeed;
        EmitBounceParticles(
            position.x - velocity.x * durationAfterBounce,
            boundary,
            boundary < 0f ? 0f : 180f
        );
    }

    public void SetXPositionAndSpeed(float start, float speedFactor, float deltaTime)
    {
        velocity.x = maxXSpeed * speedFactor;
        position.x = start + velocity.x * deltaTime;
    }

    void EmitBounceParticles(float x, float z, float rotation)
    {
        ParticleSystem.ShapeModule shape = impactParticlesSystem.shape;
        shape.position = new Vector3(x, 0f, z);
        shape.rotation = new Vector3(0f, rotation, 0f);
        impactParticlesSystem.Emit(impactParticleEmission);
    }

    void SetTrailEmission(bool enabled)
    {
        ParticleSystem.EmissionModule emisson = trailParticleSystem.emission;
        emisson.enabled = enabled;
    }

    public float BallHalfSize => halfSize;
    public Vector2 Position => position;
    public Vector2 Velocity => velocity;
}
