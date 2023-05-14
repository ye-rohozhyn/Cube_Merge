using System.Collections;
using TMPro;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float mergeForce = 20f;
    [SerializeField] private Renderer bulletRenderer;
    [SerializeField] private Color[] bulletColors;
    [SerializeField] private float explosionWaitTime = 2f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private ParticleSystem explosionEffect;

    private int _value = 1;
    private TMP_Text[] _valueFields;
    private Rigidbody _bulletRb;
    private int _stayInTriggerCount;
    private GameManager _gameManager;

    private void Awake()
    {
        _valueFields = GetComponentsInChildren<TMP_Text>();
        _bulletRb = GetComponent<Rigidbody>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionStay(Collision collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();

        if (bullet)
        {
            if (_value.Equals(bullet.GetValue()))
            {
                Merge(bullet);
            }
        }
    }

    public void Merge(Bullet bullet)
    {
        transform.position = (transform.position + bullet.transform.position) / 2f;
        Destroy(bullet.gameObject);

        int newValue = _value + 1;
        SetValue(newValue);

        if (newValue > PlayerController.MaxBulletValueInGame & newValue <= PlayerController.MaxSpawnBulletValue)
        {
            PlayerController.MaxBulletValueInGame = newValue;
        }

        if (newValue < PlayerController.MaxBulletValue)
        {
            _bulletRb.AddForce(Vector3.up * mergeForce, ForceMode.Impulse);
        }
        else
        {
            StartCoroutine(Explosion());
        }
    }

    private IEnumerator Explosion()
    {
        yield return new WaitForSeconds(explosionWaitTime);

        explosionEffect.transform.SetParent(null);
        explosionEffect.Play();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }

    public void SetValue(int value)
    {
        _value = value;

        if (_valueFields.Length > 0)
        {
            foreach (TMP_Text valueField in _valueFields)
            {
                valueField.text = _value.ToString();
            }
        }

        bulletRenderer.material.color = bulletColors[_value - 1];
    }

    public int GetValue()
    {
        return _value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Untagged") & other.CompareTag("LoseZone"))
        {
            _gameManager.ShowLosePanel();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!CompareTag("IgnoreTrigger") & other.CompareTag("LoseZone"))
        {
            _stayInTriggerCount++;

            if (_stayInTriggerCount > 50)
            {
                _gameManager.ShowLosePanel();
            }
        }
    }
}
