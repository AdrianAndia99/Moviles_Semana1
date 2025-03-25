using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExampleClass : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] shapePrefabs;
    [SerializeField] private GameObject trailPrefab;

    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float doubleTapThreshold = 0.3f;
    [SerializeField] private float swipeThreshold = 50f; // En píxeles

    [Header("Current Selection")]
    [SerializeField] private Color _selectedColor = Color.white;
    [SerializeField] private GameObject _selectedShape;

    private float _lastTapTime = 0f;
    private GameObject draggedObject = null;
    private Vector2 touchStartPos;
    private bool isSwipe = false;
    private GameObject activeTrail;
    private List<GameObject> spawnedShapes = new List<GameObject>();

    private InputAction primaryTouchAction;
    private InputAction primaryPositionAction;

    private void Awake()
    {
        // Configurar acciones de input
        var touchMap = inputActions.FindActionMap("Touch");
        primaryTouchAction = touchMap.FindAction("PrimaryTouch");
        primaryPositionAction = touchMap.FindAction("PrimaryPosition");

        // Asignar callbacks
        primaryTouchAction.started += OnTouchStarted;
        primaryTouchAction.performed += OnTouchPerformed;
        primaryTouchAction.canceled += OnTouchCanceled;
    }

    private void OnEnable()
    {
        primaryTouchAction.Enable();
        primaryPositionAction.Enable();
    }

    private void OnDisable()
    {
        primaryTouchAction.Disable();
        primaryPositionAction.Disable();

        // Limpiar callbacks
        primaryTouchAction.started -= OnTouchStarted;
        primaryTouchAction.performed -= OnTouchPerformed;
        primaryTouchAction.canceled -= OnTouchCanceled;
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = primaryPositionAction.ReadValue<Vector2>();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
        worldPosition.z = 0;

        touchStartPos = touchPosition;
        isSwipe = false;

        Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition);
        if (hitCollider != null)
        {
            // Doble tap para eliminar
            if (Time.time - _lastTapTime < doubleTapThreshold)
            {
                TryDestroyShape(hitCollider.gameObject);
                return;
            }

            // Comenzar arrastre
            draggedObject = hitCollider.gameObject;
        }
        else
        {
            // Crear nueva forma si no se tocó un objeto existente
            SpawnShape(worldPosition);
            CreateTrail(worldPosition);
        }

        _lastTapTime = Time.time;
    }

    private void OnTouchPerformed(InputAction.CallbackContext context)
    {
        if (draggedObject != null || activeTrail != null)
        {
            Vector2 touchPosition = primaryPositionAction.ReadValue<Vector2>();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
            worldPosition.z = 0;

            if (draggedObject != null)
            {
                draggedObject.transform.position = worldPosition;
            }

            if (activeTrail != null)
            {
                activeTrail.transform.position = worldPosition;
            }

            // Detectar swipe
            if (Vector2.Distance(touchPosition, touchStartPos) > swipeThreshold)
            {
                isSwipe = true;
            }
        }
    }

    private void OnTouchCanceled(InputAction.CallbackContext context)
    {
        if (isSwipe)
        {
            DestroyAllShapes();
        }

        draggedObject = null;

        if (activeTrail != null)
        {
            Destroy(activeTrail, 0.5f);
            activeTrail = null;
        }
    }

    private void SpawnShape(Vector3 position)
    {
        if (_selectedShape != null)
        {
            GameObject newShape = Instantiate(_selectedShape, position, Quaternion.identity);
            SpriteRenderer renderer = newShape.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = _selectedColor;
            }
            spawnedShapes.Add(newShape);
        }
    }

    private void TryDestroyShape(GameObject shape)
    {
        if (spawnedShapes.Contains(shape))
        {
            spawnedShapes.Remove(shape);
            Destroy(shape);
        }
    }

    private void DestroyAllShapes()
    {
        foreach (var shape in spawnedShapes)
        {
            if (shape != null)
            {
                Destroy(shape);
            }
        }
        spawnedShapes.Clear();
    }

    private void CreateTrail(Vector3 position)
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
        if (shapeIndex >= 0 && shapeIndex < shapePrefabs.Length)
        {
            _selectedShape = shapePrefabs[shapeIndex];
        }
    }

    public void SetColor(Color newColor)
    {
        _selectedColor = newColor;
    }

    public void SetColorFromButton(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red":
            case "rojo":
                _selectedColor = Color.red;
                break;
            case "blue":
            case "azul":
                _selectedColor = Color.blue;
                break;
            case "green":
            case "verde":
                _selectedColor = Color.green;
                break;
            case "black":
            case "negro":
                _selectedColor = Color.black;
                break;
            case "white":
            case "blanco":
                _selectedColor = Color.white;
                break;
            default:
                Debug.LogWarning($"Color no reconocido: {colorName}, usando blanco por defecto.");
                _selectedColor = Color.white;
                break;
        }
    }
}