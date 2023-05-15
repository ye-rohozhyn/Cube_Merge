using System.Collections;
using TMPro;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private float mergeForce = 20f;
    [SerializeField] private Renderer bulletRenderer;
    [SerializeField] private Color[] bulletColors;
    [SerializeField] private float explosionWaitTime = 2f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private ParticleSystem mergeEffect;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private AudioClip mergeSound;

    private TMP_Text[] _valueFields;
    private Rigidbody _bulletRb;
    private int _stayInTriggerCount;
    private GameManager _gameManager;

    private void Awake()
    {
        _valueFields = GetComponentsInChildren<TMP_Text>();
        _bulletRb = GetComponent<Rigidbody>();
        _gameManager = FindObjectOfType<GameManager>();

        if (_valueFields.Length > 0)
        {
            foreach (TMP_Text valueField in _valueFields)
            {
                valueField.text = value.ToString();
            }
        }

        bulletRenderer.material.color = bulletColors[value - 1];
    }

    private void OnCollisionStay(Collision collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();

        if (bullet)
        {
            if (value.Equals(bullet.GetValue()) & !value.Equals(PlayerController.MaxBulletValue))
            {
                Merge(bullet.transform);
            }
        }
    }

    public void Merge(Transform bullet)
    {
        if (!mergeSound & !mergeEffect) return;

        _gameManager.AddScore(value * 2);
        _gameManager.PlaySound(mergeSound);
        mergeEffect.Play();

        transform.position = Vector3.Distance(transform.position, Vector3.zero) > Vector3.Distance(bullet.position, Vector3.zero) ? transform.position : bullet.position;
        Destroy(bullet.gameObject);

        int newValue = value + 1;
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
            if (rb)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }

    public void SetValue(int value)
    {
        this.value = value;

        if (_valueFields.Length > 0)
        {
            foreach (TMP_Text valueField in _valueFields)
            {
                valueField.text = this.value.ToString();
            }
        }

        bulletRenderer.material.color = bulletColors[this.value - 1];
    }

    public int GetValue()
    {
        return value;
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
