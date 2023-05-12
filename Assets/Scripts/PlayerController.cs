using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveSensetivity = 2f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float spawnBulletDelay = 2f;
    [SerializeField] private float force = 10f;
    [SerializeField] private int maxBulletValue = 13;

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
            OnDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnDragEnd();
        }
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
        _readyToShoot = false;

        Rigidbody bullet = _bullet.GetComponent<Rigidbody>();
        
        if (bullet)
        {
            bullet.transform.SetParent(null);
            bullet.AddForce(bullet.transform.forward * force, ForceMode.Impulse);
        }

        StartCoroutine(SpawnBullet());
    }

    private IEnumerator SpawnBullet()
    {
        yield return new WaitForSeconds(spawnBulletDelay);

        _bullet = Instantiate(bulletPrefab, _transform.position, Quaternion.identity).transform;
        _bullet.GetComponent<Bullet>().SetValue(Random.Range(1, MaxBulletValueInGame));
        _bullet.SetParent(_transform);

        _readyToShoot = true;
    }
}
