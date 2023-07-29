using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWall : MonoBehaviour
{
    // Adjust this value if your objects are going through the walls
    public float ColliderThickness = 0.1f;

    private EdgeCollider2D topCollider;
    private EdgeCollider2D bottomCollider;
    private EdgeCollider2D leftCollider;
    private EdgeCollider2D rightCollider;

    void Start()
    {
        topCollider = CreateCollider();
        bottomCollider = CreateCollider();
        leftCollider = CreateCollider();
        rightCollider = CreateCollider();

        Vector2 lowerLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        Vector2 upperLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
        Vector2 upperRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Vector2 lowerRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));

        UpdateCollider(topCollider, upperLeft, upperRight);
        UpdateCollider(bottomCollider, lowerLeft, lowerRight);
        UpdateCollider(leftCollider, lowerLeft, upperLeft);
        UpdateCollider(rightCollider, lowerRight, upperRight);
    }

    private EdgeCollider2D CreateCollider()
    {
        GameObject colliderObject = new GameObject("ScreenEdgeCollider");
        colliderObject.tag = "Bound";
        colliderObject.transform.parent = transform;

        EdgeCollider2D collider = colliderObject.AddComponent<EdgeCollider2D>();
        collider.edgeRadius = ColliderThickness;

        return collider;
    }

    private void UpdateCollider(EdgeCollider2D collider, Vector2 pointA, Vector2 pointB)
    {
        collider.points = new Vector2[] { pointA, pointB };
    }
}

