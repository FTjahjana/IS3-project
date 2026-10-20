using Unity.VisualScripting;
using UnityEngine;

public class CreateGear : MonoBehaviour
{

    [SerializeField] GameObject gearTooth;
    [SerializeField] int teethAmount;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Create Gear")]
    void InstantiateGear()
    {
        for(int i = 0; i < teethAmount; i++)
        {
            GameObject tooth = Instantiate(gearTooth, gameObject.transform);
            tooth.transform.SetParent(transform, true);
            tooth.transform.localRotation = Quaternion.Euler(new Vector3(0,0,(360/teethAmount)*i));

        }
        
    }
}
