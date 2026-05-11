using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;

    private CharacterController _cc;
    private Vector3 _velocity;
    private Vector2 _moveInput;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        _cc.Move(move * speed * Time.deltaTime);

        if (_cc.isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }
}
