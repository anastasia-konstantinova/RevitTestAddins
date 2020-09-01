#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace RevitAddins
{
    [Transaction(TransactionMode.Manual)]
    public class SetParameter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                if (pickedObj != null)
                {
                    ElementId elementId = pickedObj.ElementId;
                    Element element = doc.GetElement(elementId);

                    Parameter param = element.get_Parameter(BuiltInParameter.REBAR_ELEM_LENGTH);

                    TaskDialog.Show("Parameter Values", string.Format("Parameter storage type {0} and value {1}",
                        param.StorageType.ToString(), param.AsDouble()));

                  using (Transaction trans = new Transaction(doc, "Set Parameter"))
                  {
                        trans.Start();

                        param.Set(7.5);

                        trans.Commit();
                  }
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