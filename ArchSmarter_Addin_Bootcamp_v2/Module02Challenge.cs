#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media.Animation;

#endregion

namespace ArchSmarter_Addin_Bootcamp_v2
{
    [Transaction(TransactionMode.Manual)]
    public class Module02Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // this is a variable to collect the current active document
            UIDocument uidoc = uiapp.ActiveUIDocument;
            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select Elements");

            //filter more specificially 
            List<CurveElement> modelCurves = new List<CurveElement>();
            foreach (Element elem in pickList)
            {
                if (elem is CurveElement)
                {
                    // cast Element type to a CurveElement type 
                    // they already are Curve elements because of the If Statement
                    // casting them to their more specific CurveElement Type gives us
                    // access different parameters
                    // CurveElement curveElem = (CurveElementr)elem;

                    CurveElement curveElem = elem as CurveElement;
                    if (curveElem.CurveElementType == CurveElementType.ModelCurve)
                    {
                        Curve currentCurve = curveElem.GeometryCurve;
                        modelCurves.Add(curveElem);
                    }
                }
            }

            TaskDialog.Show("Selected Elements", "  Selected in total: " + pickList.Count.ToString()
               + "Selected ModelLines: " + modelCurves.Count.ToString());
            //TaskDialog.Show("Selected Elements", $"You have selected " {modelCurves.Count} "ModelLines");
            //why is this not working?

            //Get type using methods
            Level theLevel = Utils.GetLevelbyName(doc, "Level !");

            Element projDuct = Utils.GetDuctTypeByName(doc,"Default");
            Element projPipe = Utils.GetPipeTypeByName(doc, "Default");                       

            WallType genWall = Utils.GetWallTypeByName(doc, @"Generic - 8""");
            WallType storeWall = Utils.GetWallTypeByName(doc, "Storefront");
            MEPSystemType ductSystemType = Utils.GetSystemTypeByName(doc, "Supply Air");
            MEPSystemType pipeSystemType = Utils.GetSystemTypeByName(doc, "Domestic Hot Water");

            //create transaction with using statement

            using (Transaction t = new Transaction(doc))
            {
                t.Start("create revit elements");

                foreach (CurveElement curCurve in modelCurves)
                {
                    Curve curve = curCurve.GeometryCurve;
                    GraphicsStyle curStyle = curCurve.LineStyle as GraphicsStyle;

                    //skip the circles that dont have endpoints
                    if (curve.IsBound == false)
                        continue;

                    XYZ startPoint = curve.GetEndPoint(0);
                    XYZ endPoint = curve.GetEndPoint(1);

                    switch (curStyle.Name)
                    {
                        case "A-GLAZ":
                            Wall.Create(doc, curve, storeWall.Id, theLevel.Id, 20, 0, false, false);
                            break;

                        case "A-WALL":
                            Wall.Create(doc, curve, genWall.Id, theLevel.Id, 20, 0, false, false);
                            break;

                        case "M-DUCT":                            
                            Duct.Create(doc, ductSystemType.Id, projDuct.Id, theLevel.Id, startPoint, endPoint);
                            break;

                        case "P-PIPE":                            
                            Pipe.Create(doc, pipeSystemType.Id, projPipe.Id, theLevel.Id, startPoint, endPoint);
                            break;

                        default:
                            break;
                    }
                }
                t.Commit();

            }           

            return Result.Succeeded;
        }


        //my methods below - create duct, pipe, wall, and systemtypes
        
        

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }
    }
}
