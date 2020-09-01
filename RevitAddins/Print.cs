using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitAddins
{
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Print : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Selection selection = uidoc.Selection;
            PrintManager pm = doc.PrintManager;
            pm.PrintRange = PrintRange.Select;
            ViewSheetSetting vss = pm.ViewSheetSetting;
            ICollection<ElementId> id;
            FilteredElementCollector collector;
            ElementClassFilter wantedElements;
            PaperSizeSet paperSize = pm.PaperSizes;
            List<Element> printElemes = null;
           

            using (Transaction trans = new Transaction(doc, "Print ViewSet"))
            {
                try
                {
                    id = selection.GetElementIds();
                    collector = new FilteredElementCollector(doc, id);
                    wantedElements = new ElementClassFilter(typeof(ViewSheet));
                    collector.WherePasses(wantedElements);
                    printElemes = collector.ToElements() as List<Element>;
                }
                catch (Autodesk.Revit.Exceptions.ApplicationException e)
                {
                    TaskDialog.Show("Error!", e.Message);
                    return Result.Cancelled;
                }
                catch(Exception e)
                {
                    TaskDialog.Show("Error!", e.Message);
                }

                ViewSet viewSet = new ViewSet();

                foreach(var elem in printElemes)
                {
                    ViewSheet viewSheet = elem as ViewSheet;
                    viewSet.Insert(viewSheet);
                }

                trans.Start();

                try
                {
                    vss.CurrentViewSheetSet.Views = viewSet;
                    vss.Save();

                    pm.SelectNewPrintDriver("Microsoft Print to PDF");
                    pm.Apply();
                    //pm.PrintSetup.Save();

                    pm.PrintSetup.CurrentPrintSetting.PrintParameters.PageOrientation = PageOrientationType.Landscape;
                    pm.PrintSetup.CurrentPrintSetting.PrintParameters.PaperPlacement = PaperPlacementType.Center;
                    pm.PrintSetup.CurrentPrintSetting.PrintParameters.ZoomType = ZoomType.Zoom;
                    pm.PrintSetup.CurrentPrintSetting.PrintParameters.Zoom = 100;
                    pm.CombinedFile = true;
                    //pm.PrintToFile = true;
                    //pm.PrintToFileName = @"C:\Users\Lenovo\Desktop\file.pdf";
                    pm.Apply();
                    //pm.PrintSetup.Save();
                    pm.SubmitPrint();
                }
                catch(Exception e)
                {
                    message = e.Message;
                    return Result.Failed;
                }
                trans.Commit();
               // return Result.Succeeded;
                
            }

            return Result.Succeeded;
        }
    }
}
