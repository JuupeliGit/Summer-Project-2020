using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] GameObject target = null;
    SpriteRenderer targetRend;

    private SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Hide shadow if the target is not active;
        if (rend.enabled != target.activeSelf)
            rend.enabled = target.activeSelf;

        if (rend.enabled)
            UpdatePosition();
    }

    private void UpdatePosition()
    {
        // Align shadow with it's target on the X axis.
        transform.position = new Vector2(target.transform.position.x, transform.position.y);
    }
}
