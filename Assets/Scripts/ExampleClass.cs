using System.Collections.Generic;
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    [Header("Simple Tap")]
    [SerializeField] private GameObject[] shapePrefabs;
    private Color _selectedColor = Color.white;
    private GameObject _selectedShape;

    [Header("Double Tap")]
    private float _lastTapTime = 0f;
    private float _doubleTapThreshold = 0.3f;

    [Header("Press and Drag")]
    private GameObject draggedObject = null;

    [Header("Swipe")]
    private float swipeThreshold = 0.3f;
    private Vector2 touchStartPos;
    private bool isSwipe = false;
    [SerializeField] private GameObject trailPrefab;
    private GameObject activeTrail;

    private List<GameObject> spawnedShapes = new List<GameObject>();

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isSwipe = false;

                    Collider2D hitCollider = Physics2D.OverlapPoint(touchPosition);
                    if (hitCollider != null)
                    {
                        if (Time.time - _lastTapTime < _doubleTapThreshold)
                        {
                            TryDestroyShape(hitCollider.gameObject);
                            return;
                        }

                        draggedObject = hitCollider.gameObject;
                    }
                    else
                    {
                        SpawnShape(touchPosition);
                    }

                    _lastTapTime = Time.time;

                    CreateTrail(touchPosition);
                    break;

                case TouchPhase.Moved:
                    if (draggedObject != null)
                    {
                        draggedObject.transform.position = touchPosition;
                    }
                    if (activeTrail != null)
                    {
                        activeTrail.transform.position = touchPosition;
                    }
                    if (Vector2.Distance(touch.position, touchStartPos) > Screen.width * swipeThreshold)
                    {
                        isSwipe = true;
                    }
                    break;
                case TouchPhase.Ended:
                    if (isSwipe)
                    {
                        DestroyAllShapes();
                    }
                    draggedObject = null;

                    if (activeTrail != null)
                    {
                        Destroy(activeTrail, 0.5f);
                    }
                    break;
            }
        }
    }

    void SpawnShape(Vector3 position)
    {
        if (_selectedShape != null)
        {
            GameObject newShape = Instantiate(_selectedShape, position, Quaternion.identity);
            newShape.GetComponent<SpriteRenderer>().color = _selectedColor;
            spawnedShapes.Add(newShape);
        }
    }

    void TryDestroyShape(GameObject shape)
    {
        if (spawnedShapes.Contains(shape))
        {
            spawnedShapes.Remove(shape);
            Destroy(shape);
        }
    }

    void DestroyAllShapes()
    {
        for (int i = spawnedShapes.Count - 1; i >= 0; i--)
        {
            Destroy(spawnedShapes[i]);
        }
        spawnedShapes.Clear();
    }

    void CreateTrail(Vector3 position)
    {
        if (trailPrefab != null)
        {
            activeTrail = Instantiate(trailPrefab, position, Quaternion.identity);
            TrailRenderer trail = activeTrail.GetComponent<TrailRenderer>();

            if (trail != null)
            {
                trail.startColor = _selectedColor;
                trail.endColor = new Color(_selectedColor.r, _selectedColor.g, _selectedColor.b, 0);
            }
        }
    }

    public void SetShape(int shapeIndex)
    {
        _selectedShape = shapePrefabs[shapeIndex];
    }

    public void SetColorFromButton(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "rojo":
                _selectedColor = Color.red;
                break;
            case "azul":
                _selectedColor = Color.blue;
                break;
            case "verde":
                _selectedColor = Color.green;
                break;
            case "negro":
                _selectedColor = Color.black;
                break;
            case "blanco":
                _selectedColor = Color.white;
                break;
            default:
                Debug.Log("Color no reconocido, usando blanco por defecto.");
                _selectedColor = Color.white;
                break;
        }
    }
}