using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace BarnoGames.Runner2020
{
    class FasterCode: MonoBehaviour
    {
        private int iterations;

        private void Start()
        {
            var result = new char[iterations];

            //unsafe
            //{
            //    fixed (char* fixedPointer = result)
            //    {
            //        var pointer = fixedPointer;

            //        for (int i = 0; i < iterations; i++)
            //        {
            //            *(pointer++) = '*';
            //        }
            //    }
            //}
        }
    }
}
