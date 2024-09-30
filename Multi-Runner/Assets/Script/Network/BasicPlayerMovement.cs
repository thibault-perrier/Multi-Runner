using UnityEngine;

public class BasicPlayerMovement : MonoBehaviour
{
    BoxCollider _col;

    Color baseColor;

    public float Speed = 7.0f;
    private Vector3 direction;

    void Start()
    {
        _col = GetComponent<BoxCollider>();
        baseColor = GetComponent<Renderer>().material.color;
    }
    void Update()
    {
        // Old input backends are enabled.
        if (Input.GetKey(KeyCode.A))
            direction += new Vector3(-1, 0, 0);

        if (Input.GetKey(KeyCode.D))
            direction += new Vector3(1, 0, 0);

        if (Input.GetKey(KeyCode.W))
            direction += new Vector3(0, 0, 1);

        if (Input.GetKey(KeyCode.S))
            direction += new Vector3(0, 0, -1);

        if (Input.GetKey(KeyCode.Space))
            direction += new Vector3(0, 1, 0);

        if (Input.GetKey(KeyCode.LeftShift))
            direction += new Vector3(0, -1, 0);

        var multiplier = Speed * Time.deltaTime;
        transform.position += direction.normalized * multiplier;

        direction = Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV();
    }

    void OnTriggerExit(Collider other)
    {
        GetComponent<Renderer>().material.color = baseColor;
    }
}
