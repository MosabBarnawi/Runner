using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Combime : MonoBehaviour
    {
        public GameObject game;


        void Start()
        {
            //MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            //CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            //int i = 0;
            //while (i < meshFilters.Length)
            //{
            //    combine[i].mesh = meshFilters[i].sharedMesh;
            //    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            //    meshFilters[i].gameObject.SetActive(false);

            //    i++;
            //}
            //transform.GetComponent<MeshFilter>().mesh = new Mesh();
            //transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine); ////// <<<=======
            //transform.gameObject.SetActive(true);
        }

        //void Update()
        //{
        //    if(Input.GetKeyDown(KeyCode.H))
        //    {
        //        var decal = Instantiate(game);
        //        decal.GetComponent<DecalMesh>().GenerateProjectedMeshImmediate();
        //    }
        //}
    }
}