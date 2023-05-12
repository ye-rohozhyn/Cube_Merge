using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveSensetivity = 2f;

    [Header("Bullets")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float spawnBulletDelay = 2f;
    [SerializeField] private float force = 10f;
    [SerializeField] private int maxBulletValue = 13;
    
    [Header("Aim")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float lineLength;

    private float _fingerPositionX;
    private Vector3 _targetPosition;
    private Transform _transform;
    private float _leftLimit = -2f, _rightLimit = 2f;

    private Transform _bullet;
    public static int MaxBulletValueInGame { set; get; }
    public static int MaxSpawnBulletValue { set; get; }
    bool _readyToShoot;

    private void Start()
    {
        _transform = transform;
        _bullet = GetComponentInChildren<Transform>().GetChild(0);

        MaxBulletValueInGame = 1;
        MaxSpawnBulletValue = (maxBulletValue / 2) + 1;

        _readyToShoot = true;
    }

    private void Update()
    {
        if (!_readyToShoot) return;

        if (Input.GetMouseButton(0))
        {
            SetPointLineRenderer();
            OnDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DisableLineRenderer();
            OnDragEnd();
        }
    }


    private void SetPointLineRenderer()
    {
        Vector3 startPosition = _transform.position;
        startPosition.y = 0.55f;
        Vector3 endPosition = startPosition + Vector3.forward * lineLength;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    private void DisableLineRenderer()
    {
        lineRenderer.enabled = false;
    }

    private void OnDrag()
    {
        // Получаем относительную позицию пальца относительно центра экрана (-moveSensetivity/2 до moveSensetivity/2)
        _fingerPositionX = (Input.mousePosition.x - (Screen.width / 2f)) / (Screen.width / moveSensetivity);

        _targetPosition = playerCamera.transform.TransformPoint(Vector3.right * _fingerPositionX);
        _targetPosition.x = Mathf.Clamp(_targetPosition.x, _leftLimit, _rightLimit);
        _targetPosition.y = _transform.position.y;
        _targetPosition.z = _transform.position.z;

        float distance = Vector3.Distance(_transform.position, _targetPosition);
        float normalizedMoveSpeed = moveSpeed / distance;

        _transform.position = Vector3.Lerp(_transform.position, _targetPosition, normalizedMoveSpeed * Time.deltaTime);
    }

    private void OnDragEnd()
    {
        if (!_bullet) return;

        _readyToShoot = false;

        Rigidbody bullet = _bullet.GetComponent<Rigidbody>();
        
        if (bullet)
        {
            bullet.transform.SetParent(null);
            bullet.AddForce(bullet.transform.forward * force, ForceMode.Impulse);
            bullet.tag = "Untagged";
        }

        StartCoroutine(SpawnBullet());
    }

    private IEnumerator SpawnBullet()
    {
        yield return new WaitForSeconds(spawnBulletDelay);

        _bullet = Instantiate(bulletPrefab, _transform.position, Quaternion.identity).transform;
        _bullet.GetComponent<Bullet>().SetValue(Random.Range(1, MaxBulletValueInGame + 1));
        _bullet.SetParent(_transform);

        _readyToShoot = true;
        lineRenderer.enabled = true;
    }
}
