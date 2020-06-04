using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralCurve : MonoBehaviour
{
    public Transform target;
    public float timer;

    public float timerSpeed;
    public float offset;

    // Start is called before the first frame update
    void Start()
    {
        transform.position += Vector3.up * timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0.0f)
            if (Input.GetButton("Jump"))
                timer -= timerSpeed * Time.deltaTime * 2.0f;
            else
                timer -= timerSpeed * Time.deltaTime;
        else
            timer = 0.0f;

        transform.position = new Vector3(Mathf.Sin(timer) * Mathf.Pow(1.0f + timer, 2.0f), timer, -Mathf.Cos(timer) * Mathf.Pow(1.0f + timer, 2.0f)) + target.position;

        transform.LookAt(target.transform.position);
        transform.eulerAngles = new Vector3(25.0f, transform.eulerAngles.y, transform.eulerAngles.z);
        transform.position = transform.position - (transform.forward * offset) + Vector3.forward;
    }
}
