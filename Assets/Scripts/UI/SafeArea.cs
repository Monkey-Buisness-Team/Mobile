using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    RectTransform rect;
    Vector2 minAnchor;
    Vector2 maxAnchor;

    void Awake() 
    {
        rect = GetComponent<RectTransform>();
        minAnchor = Screen.safeArea.position;
        maxAnchor = minAnchor + Screen.safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rect.anchorMin = minAnchor;
        rect.anchorMax = maxAnchor;
    }
}
