using UnityEngine;
using System.Collections;

public class VisibleTest : MonoBehaviour
{

    [ContextMenuItem("ShowVisible", "ShowVisible")]
    public bool ShowVisibleProp;
    // Use this for initialization
    void ShowVisible()
    {
        Debug.Log("Visible: " + GetComponent<Renderer>().isVisible);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
