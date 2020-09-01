#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace RevitAddins
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class PrintSheet : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                PrintManager pm = doc.PrintManager;
                
                Selection selection = uidoc.Selection;
                ICollection<ElementId> ids = selection.GetElementIds();
                FilteredElementCollector collector = new FilteredElementCollector(doc, ids);
                ElementClassFilter wantedElements = new ElementClassFilter(typeof(ViewSheet));
                collector.WherePasses(wantedElements);
                List<Element> printElems = collector.ToElements() as List<Element>;
                ViewSet viewSet = new ViewSet();
                foreach (Element e in printElems)
                {
                    ViewSheet viewSheet = e as ViewSheet;
                    viewSet.Insert(viewSheet);
                }
                
                
                pm.SelectNewPrintDriver("Microsoft Print to PDF");
                pm.PrintRange = PrintRange.Select;

                
                ViewSheetSetting vss = pm.ViewSheetSetting;

                vss.CurrentViewSheetSet.Views = viewSet;

                using (Transaction trans = new Transaction(doc, "Print ViewSet"))
                {

                    trans.Start();
                    string setName = "Temp";
                    vss.SaveAs(setName);
                    pm.PrintSetup.CurrentPrintSetting.PrintParameters.PageOrientation = PageOrientationType.Landscape;
                    pm.PrintSetup.CurrentPrintSetting.PrintParameters.PaperPlacement = PaperPlacementType.Center;
                    pm.PrintSetup.CurrentPrintSetting.PrintParameters.ZoomType = ZoomType.Zoom;
                    pm.PrintSetup.CurrentPrintSetting.PrintParameters.Zoom = 100;
                    
                   // pm.Apply();
                    //pm.PrintSetup.Save();

                    pm.CombinedFile = true;
                    pm.SubmitPrint();
                    //vss.Delete();
                    trans.Commit();


                }
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }
}
