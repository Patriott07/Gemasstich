using UnityEngine;
using UnityEngine.AI;

public class SetTargetPlayer : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        SetTargetDest();
        Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, 
              Camera.main.ScreenPointToRay(Input.mousePosition).direction * 100, Color.red);
    }

    void SetTargetDest()
    {
        // 0 artinya klik kiri mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Membuat laser dari posisi mouse di layar ke dunia 3D
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Jika laser mengenai sesuatu (lantai/objek)
            if (Physics.Raycast(ray, out hit))
            {
                // Suruh agent pergi ke titik sentuhan laser tersebut
                Debug.Log(hit.transform.gameObject.name);
                agent.SetDestination(hit.point);
            }
        }
    }
}
