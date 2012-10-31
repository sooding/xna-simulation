using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace particleSystem
{
    //public enum ptype
    //{
    //    ptype_dead, ptype_alive, ptype_dust, ptype_ball, ptype_sun,
    //    ptye_streak, ptype_sprite, ptype_blobby, ptype_maxvar
    //}

    


    class cPart
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Force;
        public Vector3 acc;
        public double age;
        public double mass;
        public double type;
        
        
        
        public cPart() 
        { 
        }
        

        
    }
}
