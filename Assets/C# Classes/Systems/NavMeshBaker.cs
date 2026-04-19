using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using NavMeshPlus.Components;


namespace C__Classes.Systems
{
    public class NavMeshBaker : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface navmesh;

        private IEnumerator Start()
        {
            if (ReferenceEquals(navmesh, null))
            {
                navmesh = GameObject.FindWithTag("navmesh").GetComponent<NavMeshSurface>();
            }

            if (ReferenceEquals(navmesh, null))
            {
                Debug.LogError("NavMeshSurface not found");
                yield break;
            }

            // czekamy aż:
            // - budynki się zinstancjują
            // - collidery się zarejestrują
            yield return null;
            yield return new WaitForFixedUpdate();

            navmesh.BuildNavMesh();
        }
    }
}