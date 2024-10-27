using UnityEngine;

public class RenameParent : MonoBehaviour
{
    private void Start()
    {
        if (transform.parent != null)
        {
            string newName = gameObject.name.Replace("(Clone)", "").Trim();
            
            transform.parent.name = newName;
        }
        else
        {
            Debug.LogWarning("This object has no parent to rename.");
        }
    }
}