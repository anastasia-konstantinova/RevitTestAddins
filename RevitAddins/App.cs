#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;
#endregion

namespace RevitAddins
{
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            //create a ribbon tab
            a.CreateRibbonTab("BIMPROVE");

            string path = Assembly.GetExecutingAssembly().Location;

            PushButtonData button = new PushButtonData("Button1", "Select Geometry",
                path, "RevitAddins.SelectGeometry");
            PushButtonData button2 = new PushButtonData("Button2", "Print PDF", path, "RevitAddins.Print");
            PushButtonData button3 = new PushButtonData("Button3", "Name Bars", path, "RevitAddins.NamedBars");

            RibbonPanel panel = a.CreateRibbonPanel("BIMPROVE", "Select");
            RibbonPanel panel2 = a.CreateRibbonPanel("BIMPROVE", "Print");
            RibbonPanel panel3 = a.CreateRibbonPanel("BIMPROVE", "Assign");

            Uri imagePath = new Uri(@"C:\Users\Lenovo\Desktop\RevitAddins\icon.png");
            BitmapImage image = new BitmapImage(imagePath);
            Uri imagepath2 = new Uri(@"C:\Users\Lenovo\Desktop\RevitAddins\icons32.png");
            BitmapImage image2 = new BitmapImage(imagepath2);
            Uri imagepath3 = new Uri(@"C:\Users\Lenovo\Desktop\RevitAddins\barName.png");
            BitmapImage image3 = new BitmapImage(imagepath3);

            PushButton pushButton = panel.AddItem(button) as PushButton;
            pushButton.LargeImage = image;
            PushButton pushButton2 = panel2.AddItem(button2) as PushButton;
            pushButton2.LargeImage = image2;
            PushButton pushButton3 = panel3.AddItem(button3) as PushButton;
            pushButton3.LargeImage = image3;


            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
