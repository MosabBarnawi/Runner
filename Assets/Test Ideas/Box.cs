using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    public enum MeshType { Empty, Box, Sphear }
    public class Box
    {

        public int touchCount;

        World world;

        Action<Box> cbTypeChanged;

        private MeshType meshType = MeshType.Empty;

        public MeshType Type
        {
            get { return meshType; }
            set
            {
                //if(value != meshType)
                //{
                //    meshType = value;
                //    cbTypeChanged?.Invoke(this);
                //}

                MeshType oldType = meshType;

                meshType = value;

                if (oldType != meshType)
                    cbTypeChanged?.Invoke(this);
            }
        }

        public Vector3 position;
        public Box(World world, Vector3 position)
        {
            this.world = world;
            this.position = position;
        }


        public void RegisterTypeChangeCallback(Action<Box> callback)
        {
            cbTypeChanged += callback;
        }
        public void UnrgisterTypeChangeCallback(Action<Box> callback)
        {
            cbTypeChanged -= callback;
        }
        //public void ChangeColor()
        //{

        //}

        //public GameObject myObject;
    }
}