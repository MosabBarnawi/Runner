using System.Collections;
using BarnoGames.Runner2020;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BoostScript : MonoBehaviour
{
    public GameObject BoxPrefab;
    public GameObject SpheraPrefab;

    public Mesh boxMesh;
    public Mesh SphearMesh;

    public World world;

    private void Start()
    {
        world = new World(12, 12, 12);
        world.RandomizeMesh();

        for (int x = 0; x < world.XLength; x++)
        {
            for (int y = 0; y < world.YLength; y++)
            {
                for (int z = 0; z < world.ZLength; z++)
                {
                    Box myBox = world.GetBoxAt(x, y, z);

                    MeshType meshType = myBox.Type;

                    GameObject Boxobj = null;

                    if (meshType == MeshType.Box)
                        Boxobj = Instantiate(BoxPrefab, myBox.position, Quaternion.identity, transform);
                    else if (meshType == MeshType.Sphear)
                        Boxobj = Instantiate(SpheraPrefab, myBox.position, Quaternion.identity, transform);

                    Boxobj.name = myBox.position.x + "_" + myBox.position.y + "_" + myBox.position.z;


                    myBox.RegisterTypeChangeCallback((boxx) => { OnMeshTypeChange(boxx, Boxobj); });

                }
            }
        }

        //CombimeMeshes();
    }

    private void CombimeMeshes()
    {

        if (GetComponent<MeshFilter>().mesh != null)
        {
            GetComponent<MeshFilter>().mesh = null;

            var count = transform.childCount;

            for (int w = 0; w < count; w++)
            {
                transform.GetChild(w).gameObject.SetActive(true);
            }
            Debug.Log("rU");
        }

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            //Destroy(meshFilters[i].GetComponent<MeshFilter>());
            //Destroy(meshFilters[i].GetComponent<MeshRenderer>());

            meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine,true,true); ////// <<<=======
        transform.gameObject.SetActive(true);

        //gameObject.AddComponent<BoxCollider>();


        //for (int j = 0; j < combine.Length; j++)
        //{
        //    if (meshFilters[j] == GetComponent<MeshFilter>()) continue;

        //    Destroy(meshFilters[j].GetComponent<MeshFilter>());
        //    Destroy(meshFilters[j].GetComponent<MeshRenderer>());
        //}
    }

    private void OnMeshTypeChange(Box box, GameObject boxGameObject)
    {
        if (box.Type == MeshType.Box)
        {
            boxGameObject.GetComponent<MeshFilter>().mesh = boxMesh;
        }
        else if (box.Type == MeshType.Sphear)
        {
            boxGameObject.GetComponent<MeshFilter>().mesh = SphearMesh;
        }
        else
            Debug.Log("Somehting Went Wrong");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            //CombimeMeshes();
            world.RandomizeMesh();

            //Destroy(positionMatrix[(int)delecte.x, (int)delecte.y, (int)delecte.z].myObject);
            //for (int x = 0; x < 5; x++)
            //{
            //    for (int y = 0; y < 5; y++)
            //    {
            //        for (int z = 0; z < 5; z++)
            //        {
            //            Debug.Log(positionMatrix[x, y,z].name);

            //        }
            //    }
            //}
        }
    }



}

