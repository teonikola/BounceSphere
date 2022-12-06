using Kitware.VTK;
using System;
using System.Windows.Threading;

namespace SphereBounce
{
    class Program
    {

        public DispatcherTimer gameTimer = new DispatcherTimer();
        static vtkSphereSource sphere;
        static vtkShrinkPolyData shrink;
        static vtkPolyDataMapper mapper;
        static vtkActor actor;
        static vtkRenderer ren1;
        static vtkRenderWindow renWin;
        static vtkRenderWindowInteractor iren;
        static vtkButtonRepresentation btnRep;
        static vtkButtonWidget buttonWidget;
        static vtkCoordinate coordinate;
        static vtkCamera camera;
        static double moveX = 0.03;
        static double moveY = 0.01;

        static double limitX = 1.35;
        static double limitY = 1.35;
        static void Main(string[] args)
        {
            DispatcherTimer gameTimer = new DispatcherTimer();
            gameTimer.Tick += GameTimerEvent;
            gameTimer.Interval = TimeSpan.FromMilliseconds(10);
            gameTimer.Start();

            sphere = vtkSphereSource.New();
            sphere.SetThetaResolution(30);
            sphere.SetPhiResolution(20);

            shrink = vtkShrinkPolyData.New();
            shrink.SetInputConnection(sphere.GetOutputPort());
            shrink.SetShrinkFactor(1);
            mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(shrink.GetOutputPort());

            actor = vtkActor.New();
            actor.SetMapper(mapper); actor.GetProperty().SetColor(1, 0, 0);

            btnRep = vtkTexturedButtonRepresentation2D.New();
            btnRep.SetNumberOfStates(2);

            buttonWidget = vtkButtonWidget.New();

            ren1 = vtkRenderer.New();

            renWin = vtkRenderWindow.New();
            renWin.AddRenderer(ren1);
            iren = vtkRenderWindowInteractor.New();
            iren.SetRenderWindow(renWin);

            buttonWidget.SetInteractor(iren);
            buttonWidget.SetRepresentation(btnRep);

            coordinate = vtkCoordinate.New();

            ren1.SetBackground(1, 2, 1);
            ren1.AddViewProp(actor);
            renWin.SetSize(900, 900);
            renWin.Render();

            coordinate.SetCoordinateSystemToNormalizedDisplay();
            coordinate.SetValue(1, 1);

            double[] bds = new double[6];
            double sz = 50;
            bds[0] = coordinate.GetComputedDisplayValue(ren1)[0] - sz;
            bds[1] = bds[0] + sz;
            bds[2] = coordinate.GetComputedDisplayValue(ren1)[1] - sz;
            bds[3] = bds[2] + sz;
            bds[4] = bds[5] = 0;
            //this didn't seem to display the button..

            btnRep.SetPlaceFactor(1);
            camera = ren1.GetActiveCamera();
            camera.Zoom(0.5);

            iren.Initialize();

            iren.Start();
            deleteAllVTKObjects();
        }


        private static void GameTimerEvent(object sender, EventArgs e)
        {

            MoveBall();
            iren.GetRenderWindow().Render();
        }


        private static void MoveBall()
        {
            var postitionX = actor.GetPosition()[0];

            var postitionY = actor.GetPosition()[1];

            if (postitionX + moveX < -limitX)
            {
                actor.SetPosition(-limitX, postitionY + moveY, 0);
                moveX *= -1;
            }
            if (postitionX + moveX > limitX)
            {
                actor.SetPosition(limitX, postitionY + moveY, 0);
                moveX *= -1;

            }
            if (postitionY + moveY < -limitY)
            {
                actor.SetPosition(postitionX + moveX, -limitY, 0);
                moveY *= -1;
            }
            if (postitionY + moveY > limitY)
            {

                actor.SetPosition(postitionX + moveX, limitY, 0);
                moveY *= -1;
            }
            else
            {
                actor.SetPosition(postitionX + moveX, postitionY + moveY, 0);
            }
        }
        static void deleteAllVTKObjects()
        {
            //clean up vtk objects
            if (sphere != null) { sphere.Dispose(); }
            if (shrink != null) { shrink.Dispose(); }
            if (mapper != null) { mapper.Dispose(); }
            if (actor != null) { actor.Dispose(); }
            if (ren1 != null) { ren1.Dispose(); }
            if (renWin != null) { renWin.Dispose(); }
            if (iren != null) { iren.Dispose(); }
            if (camera != null) { camera.Dispose(); }
        }


    }

}
