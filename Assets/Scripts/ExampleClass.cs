using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    public GameObject[] shapePrefabs;
    private Color selectedColor = Color.white;
    private GameObject selectedShape;

    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.3f;

    private GameObject draggedObject = null;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;

            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                if (hit.collider != null)
                {
                    draggedObject = hit.collider.gameObject;
                }
                else
                {
                    if (Time.time - lastTapTime < doubleTapThreshold)
                    {
                        TryDestroyShape(touchPosition);
                    }
                    else
                    {
                        SpawnShape(touchPosition);
                    }
                    lastTapTime = Time.time;
                }
            }
            else if (touch.phase == TouchPhase.Moved && draggedObject != null)
            {
                draggedObject.transform.position = touchPosition;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                draggedObject = null;
            }
        }
    }

    void SpawnShape(Vector3 position)
    {
        if (selectedShape != null)
        {
            GameObject newShape = Instantiate(selectedShape, position, transform.rotation);
            newShape.GetComponent<SpriteRenderer>().color = selectedColor;
        }
    }
    void TryDestroyShape(Vector3 position)
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(position);
        if (hitCollider != null)
        {
            Destroy(hitCollider.gameObject);
        }
    }

    public void SetShape(int shapeIndex)
    {
        selectedShape = shapePrefabs[shapeIndex];
    }

    public void SetColorFromButton(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "rojo":
                selectedColor = Color.red;
                break;
            case "azul":
                selectedColor = Color.blue;
                break;
            case "verde":
                selectedColor = Color.green;
                break;
            case "negro":
                selectedColor = Color.black;
                break;
            case "blanco":
                selectedColor = Color.white;
                break;
            default:
                Debug.LogWarning("Color no reconocido, usando blanco por defecto.");
                selectedColor = Color.white;
                break;
        }
    }
}