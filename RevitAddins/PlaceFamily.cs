﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitAddins
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class PlaceFamily : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            FamilySymbol symbol = collector.OfClass(typeof(FamilySymbol))
                .WhereElementIsElementType().Cast<FamilySymbol>()
                .First(x => x.Name == "1525 x 762mm");

           
            try
            {
                using (Transaction trans = new Transaction(doc, "Place Family"))
                {
                    trans.Start();
                    if (!symbol.IsActive)
                        symbol.Activate();
                    doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    trans.Commit();
                }
                return Result.Succeeded;
            }

            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
}
