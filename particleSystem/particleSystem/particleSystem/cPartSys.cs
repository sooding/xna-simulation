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
    class cPartSys
    {
        public double nu_epsilon = 10E-15;
        public bool runNotStep;
        public int partCount;
        public float timeStep;
        public Texture2D rain, water, smoke, fire;
        public List<cPart> pS0;
        cPart ps;
        cForcer fo;
        List<cPart> pS1;
        List<cPart> pS0dot;
        List<cPart> pS1dot;
        List<cPart> pSMdot;
        List<cForcer> pF0;
        public SpriteFont Font;
        public int forcerCount;
        public int wallCount;
        public int type = 3;//1-smoke,2-fire,3-fountain,4-rain

        


        public cPartSys()
        {
            pS0 =new List<cPart>();
            pS1 =new List<cPart>();
            pS0dot =new List<cPart>();
            pS1dot =new List<cPart>();
            pSMdot =new List<cPart>();

            pF0 = new List<cForcer>();
            ps = new cPart();
            fo = new cForcer();
            

            partCount = 3000;           
            forcerCount = 0;
            wallCount = 0;
            runNotStep = false;             
            timeStep = 0.001f;
            fo.gravConst = 10;
            Random randi = new Random();


            
            double rad = 0;

            for (int i = 0; i < partCount; i++)
            {
                cPart t = new cPart();

                t.age = randi.NextDouble();

                t.Position = new Vector3(0, -1, 0); //initial position of smoke,fire,fountain;


                #region position values for other effetcts

                ///////rain/////////////////
                //t.Position = new Vector3((float)(-2 + 4 * randi.NextDouble()),
                //                         (float)(-2 + 4 * randi.NextDouble()),
                //                         (float)(-2 + 4 * randi.NextDouble()));
                /////////////////////////// 

                /////fall/////////////////
                //t.Position = new Vector3(-2.5f,
                //                         (float)(-0.12 + 0.25 * randi.NextDouble()),
                //                         (float)(-0.12 + 0.25 * randi.NextDouble()));
                //////////////////////////
               
                #endregion


                rad = 1;
                t.mass = rad;

                pS0.Add(t);
            }

            cForcer f = new cForcer();
            forcerCount = 1;
            f.gravConst = 10;
            f.downDir = new Vector3(0,-1,0);//this vector sets the direction in which the particles fall;

            pF0.Add(f);
        }


       public void stateVecZero(ref List<cPart> pS)
        {
            if (pS.Count <= 0)
                return;

            else
            {
                pS.RemoveRange (0, partCount);
            }
        }

       public void Initialize(ContentManager Content)
       {
           rain = Content.Load<Texture2D>("water");
           water = Content.Load<Texture2D>("water");
           smoke = Content.Load<Texture2D>("smoke");
           fire = Content.Load<Texture2D>("fire");
           Font = Content.Load<SpriteFont>("speedFont");
       }

       public void Draw(SpriteBatch spriteBatch)
       {

           
           if (type == 1) spriteBatch.Draw(smoke, new Rectangle(0, 0, 1, 1), Color.Snow);
           if (type == 2) spriteBatch.Draw(fire, new Rectangle(0, 0, 1, 1), Color.Snow);
           if (type == 3) spriteBatch.Draw(water, new Rectangle(0, 0, 1, 1), Color.White);
           if (type == 4) spriteBatch.Draw(rain, new Rectangle(0, 0, 1, 1), Color.Orange);
       }

        public void constraint(GameTime gameTime)
        {

            float elasped = (float)gameTime.ElapsedGameTime.Milliseconds;
            Random randi = new Random();
            
            for (int i = 0; i < partCount; i++)
            {
                if (pS0[i].age <= 0)
                {
                    
                    //////fountain///////////
                    float VelVarX = (float)(-0.5 + randi.NextDouble()),
                          VelVarY = 4f,
                          VelVarZ = (float)(-0.5 + randi.NextDouble());


                    pS0[i].Velocity = new Vector3(VelVarX, VelVarY, VelVarZ);
                    pS0[i].Position = new Vector3(0, -1, 0);
                    /////////////////////////////////

                    ////fire and smoke///////////////
                    //float VelVarX = (float)(-0.5 + randi.NextDouble()),
                    //      VelVarY = 1f,
                    //      VelVarZ = (float)(-0.5 + randi.NextDouble());


                    //pS0[i].Velocity = new Vector3(VelVarX, VelVarY, VelVarZ);
                    //pS0[i].Position = new Vector3(0, -1, 0);
                    /////////////////////////////////

                    #region other effects
                    /////////rain////////////////////
                    //float PosVarX = (float)(-2 + 4 * randi.NextDouble()),
                    //      PosVarY = (float)(-2 + 4 * randi.NextDouble()),
                    //      PosVarZ = (float)(-2 + 4 * randi.NextDouble());
                    //pS0[i].Position = new Vector3(PosVarX, PosVarY, PosVarZ);
                    ////////////////////////////////

                    ///////fall///////////////////
                    //float PosVarX = -2.5f,
                    //      PosVarY = (float)(-0.12 + 0.25 * randi.NextDouble()),
                    //      PosVarZ = (float)(-0.12 + 0.25 * randi.NextDouble());
                    //pS0[i].Position = new Vector3(PosVarX, PosVarY, PosVarZ);

                    //float VelVarX = 0.8f,
                    //      VelVarY = 0,
                    //      VelVarZ = (float)(-0.05 + randi.NextDouble() / 10);
                    //pS0[i].Velocity = new Vector3(VelVarX, VelVarY, VelVarZ);
                    //////////////////////////////
                    #endregion


                    pS0[i].age = randi.NextDouble();
                }
            }
        }


        public void updateParticles(GameTime gameTime)
        {
            Random r = new Random();
            constraint(gameTime);
            ps.acc = new Vector3(0, -1, 0) * 10;//new vector3(2,-1,0) for fall; new Vector3(0,-1,0) for rain; new Vector3(0,1,0);
            for (int i = 0; i < partCount; i++)
            {
                pS0[i].age -= 0.001;//0.003 for rain,smoke and fire,0.001 for fountain
                pS0[i].Velocity += ps.acc * timeStep; 
                pS0[i].Position += pS0[i].Velocity * timeStep;

            }

        }

        #region euler solver
        public void stateVecAPlusBTimesC(ref List<cPart> pDest, List<cPart> A, List<cPart> B, float C)
        {
            if (pDest.Count != 0)
            {
                pDest.RemoveRange(0, partCount);
            }

            for (int i = 0; i < partCount; i++)
            {
                cPart temp = new cPart();


                temp.Position = A[i].Position + B[i].Position * C;
                temp.Velocity = A[i].Velocity + B[i].Velocity * C;
                temp.Force = A[i].Force + B[i].Force * C;

                temp.age = A[i].age + B[i].age * C;
                temp.mass = A[i].mass + B[i].mass * C;
                pDest.Add(temp);
            }

        }

        public void applyAllForces(ref List<cPart> pS)
        {
            for (int i = 0; i < partCount; i++)
            {
                pS[i].Force = new Vector3(0,0,0);
            }

            Random rand = new Random();
            double mag;
            for (int j = 0; j < forcerCount; j++)
            {
                for (int i = 0; i < partCount; i++)
                {
                    mag = pS[i].mass * pF0[j].gravConst * 0.01;
                    pS[i].Force += (float)mag * pF0[j].downDir;
                }
            }

        }

        

        public void stateVecSwap(ref List<cPart> to, ref List<cPart> from)
        {
            List<cPart> temp1 = new List<cPart>();
            List<cPart> temp2 = new List<cPart>();
            for (int i = 0; i < partCount; i++)
            {
                cPart a = new cPart();
                cPart b = new cPart();

                a.Position = from[i].Position;
                a.Velocity = from[i].Velocity;
                a.Force = from[i].Force;
                a.age = from[i].age;
                a.mass = from[i].mass;

                b.Position = to[i].Position;
                b.Velocity = to[i].Velocity;
                b.Force = to[i].Force;
                b.age = to[i].age;
                b.mass = to[i].mass;

                from.Add(b);
                to.Add(a);
            }

            from.RemoveRange(0, partCount);
            to.RemoveRange(0, partCount);

        }

        public void dotMaker(ref List<cPart> pDotDest, ref List<cPart> pSrc)
        {
            applyAllForces(ref pSrc);
            stateVecZero(ref pDotDest);;
            for (int i = 0; i < partCount; i++)
            {


                cPart temp = new cPart();

                temp.type = pSrc[i].type;
                temp.age -= 0.08;
                temp.Position = pSrc[i].Velocity;

                temp.Velocity = new Vector3((float)(pSrc[i].Force.X / (nu_epsilon + pSrc[i].mass)), 
                                            (float)(pSrc[i].Force.Y / (nu_epsilon + pSrc[i].mass)), 
                                            (float)(pSrc[i].Force.Z / (nu_epsilon + pSrc[i].mass)));

                pDotDest.Add(temp);
            }
        }

        public void solver(GameTime gameTime)
        {
            dotMaker(ref pS0dot, ref pS0);
            stateVecAPlusBTimesC(ref pS1, pS0, pS0dot, (float)timeStep);
            stateVecSwap(ref pS0, ref pS1);
            constraint(gameTime);
        }

        #endregion
    }
}
