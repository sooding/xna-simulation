using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace particleSystem
{
    //public enum force
    //{
    //    f_none,f_mouse,f_grav_e,f_grav_p,f_wind,f_bubble,f_drag,f_spring,f_springset,f_charge,f_maxkinds
    //}

    class cForcer
    {
            public int forceType;
            public double gravConst;
            public Vector3 downDir;  
            public double[] bub_ctr=new double[3];  
            public cForcer() 
            { 
            
            }
    }
}
