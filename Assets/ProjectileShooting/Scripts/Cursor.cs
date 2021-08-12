using UnityEngine;

public class Cursor : MonoBehaviour 
{
    Transform originParent;
    void Start()
    {
        originParent = transform.parent;
        transform.parent = null;
        transform.localScale = Vector3.one;
        transform.parent = originParent;
    }
    void Update () 
	{
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1 << LayerMask.NameToLayer("Ground")))
        {
            transform.position = hit.point;
        }

        transform.rotation = Quaternion.identity;
    }
}
