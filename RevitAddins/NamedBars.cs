using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitAddins
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class NamedBars : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Rebar);
            IList<Element> bars = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            try
            {  
                    //check
                    if (bars.Count == 0)
                    {
                        TaskDialog.Show("Warning!", "No bars in the current document!");
                        return Result.Cancelled;
                    }
                    else
                    {
                        foreach (Element e in bars)
                        {
                            Parameter len = e.LookupParameter("L");
                            Parameter dia = e.LookupParameter("ØАрматуры");
                            Parameter name = e.LookupParameter("Наименование");

                            using (Transaction trans = new Transaction(doc, "Set Valid Names"))
                            {
                                trans.Start();
                                name.Set(string.Format("Ø{0}, L={1}", dia.AsValueString(), len.AsValueString()));
                                trans.Commit();
                            }
                        }
                        TaskDialog.Show("Assign names in Document.", 
                            string.Format("{0} bars found in the current document. Valid names assigned.", bars.Count));

                        return Result.Succeeded;
                    }    
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }
}

