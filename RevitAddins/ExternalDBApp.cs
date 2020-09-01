using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddins
{
    class ExternalDBApp : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnShutdown(ControlledApplication a)
        {
            a.DocumentChanged -= elementChangedEvent;
            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnStartup(ControlledApplication a)
        {
            try
            {
                //registered event
                a.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(elementChangedEvent);
            }
            catch (Exception)
            {
               return  ExternalDBApplicationResult.Failed;
            }
            return ExternalDBApplicationResult.Succeeded;
        }

        public void elementChangedEvent(Object sender, DocumentChangedEventArgs args)
        {
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Furniture);
            ElementId element = args.GetModifiedElementIds(filter).First();
            string name = args.GetTransactionNames().First();

            TaskDialog.Show("Modified Element", element.ToString() + " changed by " + name);
        }
    }
}
