using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class World
    {
        int xLength;
        int yLength;
        int zLength;

        Box[,,] boxes;

        List<Box> BoxesList = new List<Box>();

        public World(int xLength, int yLength, int zLength)
        {
            this.xLength = xLength;
            this.yLength = yLength;
            this.zLength = zLength;

            boxes = new Box[xLength * 3, yLength * 3, zLength * 3];

            Vector3 lastPosition = Vector3.zero;

            Vector3 position;

            for (int x = 0; x < xLength; x++)
            {
                if (x == 0) position.x = 0;
                else position.x = lastPosition.x + 2;

                for (int y = 0; y < yLength; y++)
                {
                    if (y == 0) position.y = 0;
                    else position.y = lastPosition.y + 2;

                    for (int z = 0; z < zLength; z++)
                    {
                        if (z == 0) position.z = 0;
                        else position.z = lastPosition.z + 2;


                        Box newBox = new Box(this, position);
                        boxes[x, y, z] = newBox;
                        BoxesList.Add(newBox);
                        //Debug.Log(position.x + "_" + position.y + "_" + position.z);
                        lastPosition = position;
                    }
                }
            }

            //RandomizeMesh();
        }

        public int XLength { get => xLength; }
        public int YLength { get => yLength; }
        public int ZLength { get => zLength; }

        public void RandomizeMesh()
        {
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        if (Random.Range(0, 2) == 0)
                            boxes[x, y, z].Type = MeshType.Box;
                        else
                            boxes[x, y, z].Type = MeshType.Sphear;
                    }
                }
            }
        }

        public Box GetBoxAt(int x, int y, int z)
        {
            return boxes[x, y, z];
        }
        public Box GetBoxAt(Vector3 position)
        {
            foreach (Box item in BoxesList)
            {
                if(item.position == position)
                {
                    return item;
                }
            }
            return null;
        }
    }
}