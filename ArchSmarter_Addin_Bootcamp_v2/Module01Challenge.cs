#region Namespaces
using ArchSmarter_Addin_Bootcamp_v2;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;

#endregion

namespace ArchSmarter_Addin_Bootcamp_v2
{
    [Transaction(TransactionMode.Manual)]
    public class Module01Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // module 01 - creates 250 levels, at 15 feet height, and renames thems.
            // then creates floorplans if divided by 3, ceiling plans if it divides by 5
            // then if it is divided by both 3 and 5 it creates a sheet.


            //declare a number variable and set it to 250
            int num1 = 250;
            //declare a starting elevation variable and set it to 0
            double ele1 = 0;
            //declare a floor height variable and set it to 15
            double floorHeight1 = 15;

            //create filters to collect the floor, ceiling plan views
            FilteredElementCollector ViewCollector = new FilteredElementCollector(doc);
            ViewCollector.OfClass(typeof(ViewFamilyType));

            ViewFamilyType floorPlanVFT = null;
            foreach (ViewFamilyType curVFT in ViewCollector)
            {
                if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    floorPlanVFT = curVFT;
                }
            }

            ViewFamilyType ceilingPlanVFT = null;
            foreach (ViewFamilyType curVFT in ViewCollector)
            {
                if (curVFT.ViewFamily == ViewFamily.CeilingPlan)
                {
                    ceilingPlanVFT = curVFT;
                }
            }

            //create filter to collect the right titleblock to create the sheet
            FilteredElementCollector sheetCollector = new FilteredElementCollector(doc);
            sheetCollector.OfCategory(BuiltInCategory.OST_TitleBlocks);

            //create a transaction to lock the model
            Transaction t = new Transaction(doc);
            t.Start("Create 250 floor levels");


            //for every number in 230, create a floor plan         

            for (int i = 0; i <= num1; i++)
            {
                ele1 = i * floorHeight1;
                Level newLevel = Level.Create(doc, ele1); //this is not working                          
                newLevel.Name = "New Level_" + i.ToString();



                if (i % 3 == 0 && i % 5 == 0) //this is not working
                {
                    //create a sheet 
                    ViewSheet newSheet = ViewSheet.Create(doc, sheetCollector.FirstElement().Id);
                    newSheet.Name = "FIZZBUZZ_" + i;
                    //create floorplan 
                    ViewPlan floorPlan = ViewPlan.Create(doc, floorPlanVFT.Id, newLevel.Id);
                    floorPlan.Name = "FIZZBUZZ_" + i;
                    XYZ p = new XYZ(1, 1, 1);
                    Viewport.Create(doc, newSheet.Id, floorPlan.Id, p);

                }
                else if (i % 3 == 0) //not by 5 as well
                {
                    //create floorplan 
                    ViewPlan floorPlan = ViewPlan.Create(doc, floorPlanVFT.Id, newLevel.Id);
                    floorPlan.Name = "FIZZ_" + i;
                }
                else if (i % 5 == 0) //not by 3 as well
                {
                    //create a ceilingplan
                    ViewPlan ceilingPlan = ViewPlan.Create(doc, ceilingPlanVFT.Id, newLevel.Id);
                    ceilingPlan.Name = "BUZZ_" + i;
                }

            }

            //make a change in the revit model
            t.Commit();
            t.Dispose();



            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnMod01";
            string buttonTitle = "Tool 1";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.icons8_paint_roller_cute_color_32,
                Properties.Resources.icons8_paint_roller_cute_color_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }
    }
}