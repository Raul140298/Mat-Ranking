using UnityEngine;
using UnityEngine.UI;

public class NpcDialogueAreaScript : MonoBehaviour
{
    [SerializeField] private Button btn;

    private void Start()
    {
        btn.interactable = false;
    }

    private void resetBtn()
    {
        btn.interactable = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        btn.interactable = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        btn.interactable = false;
    }

    public Button Btn
    {
        get { return btn; }
        set { btn = value; }
    }
}
