using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (this.gameObject.name.Contains("Strawberry"))
        {
            var strs = GameObject.Find("Player").GetComponent<PlayerController>().strawberries;
            if (strs.Contains(this.gameObject.name))
            {
                Destroy(this.gameObject);
            }
        }
        else if (this.gameObject.name.Contains("Box"))
        {

        }
    }
}
