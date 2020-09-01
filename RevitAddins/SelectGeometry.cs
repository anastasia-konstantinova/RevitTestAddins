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
    [Transaction(TransactionMode.ReadOnly)]
    public class SelectGeometry : IExternalCommand
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

                    Options gOptions = new Options();
                    gOptions.DetailLevel = ViewDetailLevel.Fine;
                    GeometryElement geom = element.get_Geometry(gOptions);

                    foreach(GeometryObject gObj in geom)
                    {
                        Solid gSolid = gObj as Solid;

                        int faces = 0;
                        double area = 0.0;

                        foreach(Face gFace in gSolid.Faces)
                        {
                            area += gFace.Area;
                            faces++;
                        }

                        area = UnitUtils.ConvertFromInternalUnits(area, 
                            DisplayUnitType.DUT_SQUARE_METERS);

                        TaskDialog.Show("Geometry",
                            string.Format("Number of Faces: {0}" +
                            Environment.NewLine + "Totel Area: {1}",
                            faces, area));
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

