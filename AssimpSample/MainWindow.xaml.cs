﻿using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Camera"), "camera.3ds", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.M && m_world.m_startAnimation == true)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.F6: this.Close(); break;
                case Key.Up: if (m_world.RotationX <= 50) m_world.RotationX += 7.0f; break;
                case Key.Down: if (m_world.RotationX >= 50 || m_world.RotationX >= 0) m_world.RotationX -= 7.0f; break;
                case Key.Left: if (m_world.RotationY >= -50) m_world.RotationY -= 7.0f; break;
                case Key.Right: if (m_world.RotationY <= 50) m_world.RotationY += 7.0f; break;
                case Key.Add: m_world.SceneDistance -= 700.0f; break;
                case Key.Subtract: m_world.SceneDistance += 700.0f; break;

                case Key.M:
                    m_world.stop = 1;
                    if (m_world.m_startAnimation)
                        m_world.m_startAnimation = false;
                    else
                    {
                        m_world.m_startAnimation = true;
                    }

                    cageSlider.IsEnabled = !m_world.m_startAnimation;
                    cameraRotationSpeedSlider.IsEnabled = !m_world.m_startAnimation;
                    r.IsEnabled = !m_world.m_startAnimation;
                    g.IsEnabled = !m_world.m_startAnimation;
                    b.IsEnabled = !m_world.m_startAnimation;

                    break;
                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool)opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK);
                        }
                    }
                    break;
            }
        }

        private void cameraSpeedSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_world != null)
            {
                m_world.speedRotationCamera = (int)cameraRotationSpeedSlider.Value;
            }
        }

       

       
       

        private void r_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_world != null)
            {
                m_world.r = (int)r.Value;
            }
        }

        private void g_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_world != null)
            {
                m_world.g = (int)g.Value;
            }
        }

        private void b_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_world != null)
            {
                m_world.b = (int)b.Value;
            }
        }

        private void WidthOfCageSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_world != null)
            {
                m_world.widthCage = (int)cageSlider.Value;
            }
        }

     
    }
}
