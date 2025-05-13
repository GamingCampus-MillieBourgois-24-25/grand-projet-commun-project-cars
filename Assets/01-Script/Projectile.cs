using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    private void Start()
    {
        // Vérifier et configurer le Rigidbody et le Collider
        if (!GetComponent<Rigidbody>())
        {
            Debug.LogWarning("Ajout automatique d'un Rigidbody au projectile");
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
        
        Collider col = GetComponent<Collider>();
        if (col)
        {
            col.isTrigger = true;
            Debug.Log("Collider configuré en trigger");
        }
        else
        {
            Debug.LogError("Le projectile n'a pas de Collider!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Projectile a touché : " + other.gameObject.name);
        ImpactPointManager.AddImpactPoint(transform.position);
        
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Projectile a touché un ennemi !");
        }
        
        Destroy(gameObject);
    }
    
    private void Update()
    {
        Debug.DrawRay(transform.position, GetComponent<Rigidbody>()?.velocity.normalized ?? Vector3.forward, Color.red);
    }
    
    // Méthode alternative au cas où les triggers ne fonctionnent pas
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision détectée avec : " + collision.gameObject.name);
        ImpactPointManager.AddImpactPoint(collision.contacts[0].point);
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Projectile a touché un ennemi (collision) !");
        }
        
        Destroy(gameObject);
    }
}