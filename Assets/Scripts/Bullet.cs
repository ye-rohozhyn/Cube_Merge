using TMPro;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int _value = 1;
    private TMP_Text[] _valueFields;

    private void Awake()
    {
        _valueFields = GetComponentsInChildren<TMP_Text>();
    }

    private void OnCollisionStay(Collision collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();

        if (bullet)
        {
            if (_value.Equals(bullet.GetValue()))
            {
                transform.position = (transform.position + bullet.transform.position) / 2f;
                Destroy(bullet.gameObject);

                int newValue = _value + 1;
                SetValue(newValue);

                if (newValue > PlayerController.MaxBulletValueInGame)
                {
                    PlayerController.MaxBulletValueInGame = newValue;
                }
            }
        }
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
    }

    public int GetValue()
    {
        return _value;
    }
}
