#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Windows.Markup;

#endregion

namespace ArchSmarter_Addin_Bootcamp_v2
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
            // 1. Create ribbon tab name
            string tabName = "Revit Add-In Bootcamp";
            try
            {
                app.CreateRibbonTab(tabName);
            }
            catch (Exception)
            {
                Debug.Print("Tab already exists.");
            }

            // 2a. Create ribbon panel 
            RibbonPanel panel = Utils.CreateRibbonPanel(app, tabName, "Revit Tools");
            
            // 2b.adds tools to the Add-in tab instead of to the custom Ribbon
            //RibbonPanel panelmore = app.CreateRibbonPanel("More Revit Tools");

            // 3. Create button data instances
            PushButtonData btnData1 = Module01Challenge.GetButtonData();
            PushButtonData btnData2 = Module02Challenge.GetButtonData();
            PushButtonData btnData3 = Module03Challenge.GetButtonData();
            PushButtonData btnData4 = Command1.GetButtonData();
            PushButtonData btnData5 = Command2.GetButtonData();

            // 4. Create buttons
            //push buttons
            PushButton myButton1 = panel.AddItem(btnData1) as PushButton;
            PushButton myButton2 = panel.AddItem(btnData2) as PushButton;
            

                //split buttons - doesnt need an image since it splits
                //swops the main button for the one you clicked out of a vertical list
                //!!create the splitbutton container, add to panel, add the push buttons to the panel
            SplitButtonData splitButtonData = new SplitButtonData("split1", "split\rButton");
            SplitButton splitButton = panel.AddItem(splitButtonData) as SplitButton;
            splitButton.AddPushButton(btnData1);
            splitButton.AddPushButton(btnData2);

                //pull down for push buttons(needs images)
                //main button does not change from the main one in the vertial list 
                // pulldown container, add large and small images to it, add buttons
            PulldownButtonData pulldownData = new PulldownButtonData("Pulldown1", "More Tools");
            pulldownData.LargeImage = ButtonDataClass.BitmapToImageSource(Properties.Resources.icons8_tools_cute_color_32);
            pulldownData.Image = ButtonDataClass.BitmapToImageSource(Properties.Resources.icons8_tools_cute_color_16);
                //create pulldown button, reference the panel, add data to it. create buttons
            PulldownButton pulldownButton = panel.AddItem(pulldownData) as PulldownButton;
            pulldownButton.AddPushButton(btnData1);
            pulldownButton.AddPushButton(btnData2);
            pulldownButton.AddPushButton(btnData3);

            //Stacked button - vertical style
            //stacks up to 3 buttons within the ribbon
            panel.AddStackedItems(btnData3, btnData4, btnData5);


            // NOTE:
            // To create a new tool, copy lines 35 and 39 and rename the variables to "btnData3" and "myButton3". 
            // Change the name of the tool in the arguments of line 

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }


    }
}
