using UnityEngine;
using TMPro;

public class NumberCube : MonoBehaviour
{
    [SerializeField] private TextMeshPro[] labels;
    [SerializeField] private Renderer cubeRenderer;
    [SerializeField] private Material[] materialsByValue;

    public int value { get; private set; }
    
    public void SetValue(int newValue)
    {
        value = newValue;

        // Оновлюємо текст на всіх гранях
        foreach (var label in labels)
        {
            label.text = value.ToString();
        }

        // Змінюємо матеріал за індексом
        int index = Mathf.RoundToInt(Mathf.Log(value, 2)) - 1;
        if (index >= 0 && index < materialsByValue.Length)
        {
            cubeRenderer.material = materialsByValue[index];
        }
    }
}