using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    public GameObject[] shapePrefabs;
    private Color selectedColor = Color.white;
    private GameObject selectedShape;

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            touchPosition.z = 0;
            SpawnShape(touchPosition);
        }
    }

    void SpawnShape(Vector3 position)
    {
        if (selectedShape != null)
        {
            GameObject newShape = Instantiate(selectedShape, position, Quaternion.identity);
            newShape.GetComponent<SpriteRenderer>().color = selectedColor;
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