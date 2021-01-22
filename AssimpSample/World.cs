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
using System.Drawing;
using System.Drawing.Imaging;
using SharpGL.Enumerations;

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

        private enum TextureObjects { Bricks = 0, Concentrate, Rust }
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private string[] m_textureFiles = { "C://Users//kundacina//Documents//godina-4//semestar-7//grafika//graphics-security-camera//AssimpSample//images//bricks.jpg", "C://Users//kundacina//Documents//godina-4//semestar-7//grafika//graphics-security-camera//AssimpSample//images//stone.jpg", "C://Users//kundacina//Documents//godina-4//semestar-7//grafika//graphics-security-camera//AssimpSample//images//rust.jpg" };
        private uint[] m_textures = null;


        public float speedRotation = 0;

        public float r = 0;
        public float g = 0;
        public float b = 0;

        public float widthCage = 160;

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
            this.m_textures = new uint[m_textureCount];
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
            // Color tracking mehanizam
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_NORMALIZE);
            gl.Enable(OpenGL.GL_AUTO_NORMAL);



            gl.Enable(OpenGL.GL_TEXTURE_2D);
            // Stapanje teksture sa materijalom
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            // Ucitavanje slika i kreiranje tekstura
            gl.GenTextures(m_textureCount, m_textures);

            for (int i = 0; i < m_textureCount; i++)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);
                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // Rotiramo sliku zbog koordinatnog sistema OpenGL-a
                image.RotateFlip(RotateFlipType.Rotate90FlipX);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljava providnost slike)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                //MipMap linearno filtriranje
                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                // Podesavanje Wrapinga da bude GL_REPEAT po obema osama
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                image.UnlockBits(imageData);
                image.Dispose();
            }


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

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            drawGround(gl);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Bricks]);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            gl.Scale(1.0f, 1.0f, 1.0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            drawWalls(gl);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_CULL_FACE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Rust]);
            drawCage(gl);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            drawCamera(gl);

            drawLight(gl);

            drawRedLight(gl);

            drawText(gl);

            gl.PopMatrix();
            // Oznaci kraj iscrtavanja
            gl.Flush();
        }

        private void SetupLighting(OpenGL gl)
        {
            float[] ambijent = { 0.7f, 0.7f, 0.7f, 1.0f };
            float[] difuz = { 0.5f, 0.5f, 0.5f, 1.0f };
            // Pridruži komponente svetlosnom izvoru 0
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT,
            ambijent);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, difuz);
            // Podesi parametre tackastog svetlosnog izvora
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f); //tackasti izvor cutoff 180
            // Ukljuci svetlosni izvor
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_LIGHTING);
            // Pozicioniraj svetlosni izvor
            float[] pozicija = { -40f, 0f, 0f, 1f }; //negativna x-osa, levo od kaveza; kec za reflektivno
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, pozicija);
        }


        public void drawRedLight(OpenGL gl)
        {
            gl.PushMatrix();
            Sphere sfera = new Sphere();

            gl.Translate(-325.0f, 320.0f, -310.0f);
            gl.Scale(1.0f, 1.0f, 1.0f);
            gl.Color(1.0f, 0.0f, 0.0f);

            float[] ambient = { r, g, b, 1.0f }; //setovanje ambijentalne komponente reflektroskog svetlosnog izvora
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 25.0f); // cut off 25 
            gl.Enable(OpenGL.GL_LIGHT1);

            sfera.Radius = 7f;

            sfera.CreateInContext(gl);
            sfera.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();


        }

        public void drawLight(OpenGL gl)
        {
            gl.PushMatrix();
            Sphere spfera = new Sphere();

            gl.Translate(-200.0f, 400.0f, 100.0f);
            gl.Scale(5.0f, 5.0f, 5.0f);
            gl.Color(255f, 255f, 0f);
            spfera.Radius = 7f;

            spfera.CreateInContext(gl);
            SetupLighting(gl);
            spfera.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

        }



        public void drawGround(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Color(0.6f, 0.6f, 0.6f);


            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Concentrate]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

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

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Rust]);

            // Skaliranje teksture
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            gl.Scale(5.0f, 5.0f, 5.0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);


            cage.NormalGeneration = Normals.Smooth;
            cage.NormalOrientation = Orientation.Outside;
            cage.TextureCoords = true;

            gl.Scale(widthCage, 300.0f, 300.0f);
            cage.TopRadius = 1;
            gl.Rotate(-90.0f, 0.0f, 0.0f);
            
            gl.LineWidth(3.0f);
            cage.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);

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
