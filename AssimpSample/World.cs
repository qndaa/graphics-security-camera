// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = -3000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width = 0;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height = 0;


        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            // Testiranje dubine
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            // Nevidljive povrsine
            gl.Enable(OpenGL.GL_CULL_FACE_MODE);

            m_scene.LoadScene();
            m_scene.Initialize();

       
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);          

            gl.PushMatrix();

            gl.Translate(0.0f, -50.0f, m_sceneDistance);

            // Ugao rotacije
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            drawGround(gl);

            drawWalls(gl);

            drawCage(gl);

            drawCamera(gl);


            drawText(gl);

            gl.PopMatrix();
            // Oznaci kraj iscrtavanja
            gl.Flush();
        }

        public void drawGround(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Color(0.6f, 0.6f, 0.6f);

            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(500.0f, -300.0f, -500.0f);
            gl.Vertex(-500.0f, -300.0f, -500.0f);
            gl.Vertex(-500.0f, -300.0f, 500.0f);
            gl.Vertex(500.0f, -300.0f, 500.0f);
            gl.End();
            gl.PopMatrix();

        }

        public void drawWalls(OpenGL gl)
        {
            Cube cube = new Cube();

            gl.PushMatrix();
            gl.Color(0.4f, 0.4f, 0.4f);
            gl.Translate(0.0f, 100.0f, -500.0f);
            gl.Scale(500.0f, 400.0f, 10.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.3f, 0.3f, 0.3f);
            gl.Translate(500.0f, 100.0f, 0.0f);
            gl.Rotate(0.0f, 90.0f, 0.0f);
            gl.Scale(500.0f, 400.0f, 10.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.3f, 0.3f, 0.3f);
            gl.Translate(-500.0f, 100.0f, 0.0f);
            gl.Rotate(0.0f, -90.0f, 0.0f);
            gl.Scale(500.0f, 400.0f, 10.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

        }

        // Metoda za iscrtavanje kaveza
        public void drawCage(OpenGL gl)
        {
            Cylinder cage = new Cylinder();

            gl.PushMatrix();
            gl.Translate(100.0f, -300.0f, 0.0f);
            cage.CreateInContext(gl);

            cage.NormalGeneration = Normals.Smooth;
            cage.NormalOrientation = Orientation.Outside;

            gl.Scale(160.0f, 300.0f, 300.0f);
            cage.TopRadius = 1;
            gl.Rotate(-90.0f, 0.0f, 0.0f);
            
            gl.LineWidth(3.0f);
            cage.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            
        }



        public void drawCamera(OpenGL gl)
        {
            gl.PushMatrix();
                gl.Color(1.0f, 0.0f, 0.0f);
                gl.Translate(-400.0f, 100.0f, -350.0f);
                gl.Rotate(0.0f, 60.0f, 0.0f);
                gl.Scale(0.05f, 0.05f, 0.05f);
                m_scene.Draw();
            gl.PopMatrix();
        }



        public void drawText(OpenGL gl)
        {     
            gl.PushMatrix();

            gl.Viewport(0, m_height - m_height/4, m_width / 5, m_height /4);

            gl.PushMatrix();
            gl.DrawText3D("Helvetica Bold", 12, 0, 0, "");
            gl.PopMatrix();
            gl.DrawText(50, m_height - 50, 1.0f, 0.0f, 0.0f, "Helvetica Bold", 12, "Predmet: Racunarska grafika");
            gl.DrawText(50, m_height - 100, 1.0f, 0.0f, 0.0f, "Helvetica Bold", 12, "Sk.god: 2020/21.");
            gl.DrawText(50, m_height - 150, 1.0f, 0.0f, 0.0f, "Helvetica Bold", 12, "Ime: Djordjije");
            gl.DrawText(50, m_height - 200, 1.0f, 0.0f, 0.0f, "Helvetica Bold", 12, "Prezime: Kundacina");
            gl.DrawText(50, m_height - 250, 1.0f, 0.0f, 0.0f, "Helvetica Bold", 12, "Sifra zad: 4.2");
           
            gl.PopMatrix();
            Resize(gl, m_width, m_height);
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 0.5f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
