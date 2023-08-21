using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchSmarter_Addin_Bootcamp_v2
{
    internal static class Utils
    {
        internal static Level GetLevelbyName(Document doc, string LevelName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Level));
            collector.WhereElementIsElementType();
            foreach (Level level in collector)
            {
                if (level.Name == LevelName)
                    return level;
            }
            return null;
        }

        internal static WallType GetWallTypeByName(Document doc, String typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(WallType));

            foreach (WallType curType in collector)
            {
                if (curType.Name == typeName)
                {
                    return curType;
                }
            }
            return null;
        }

        internal static MEPSystemType GetSystemTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(MEPSystemType));

            foreach (MEPSystemType curType in collector)
            {
                if (curType.Name == typeName)
                {
                    return curType;
                }
            }
            return null;
        }

        internal static MEPSystemType GetDuctTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(DuctType));

            foreach (MEPSystemType curType in collector)
            {
                if (curType.Name == typeName)
                {
                    return curType;
                }
            }
            return null;
        }

        internal static MEPSystemType GetPipeTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PipeType));

            foreach (MEPSystemType curType in collector)
            {
                if (curType.Name == typeName)
                {
                    return curType;
                }
            }
            return null;
        }
        internal static List<ViewSheet> FilterSheetListByDoesNotEqual(List<ViewSheet> sheetlist, string sheetFilter)
        {
            List<ViewSheet> returnList = new List<ViewSheet>();

            foreach (ViewSheet sheet in sheetlist)
            {
                if (sheet.Name != sheetFilter)
                    returnList.Add(sheet);
            }

            return returnList;

        }

        internal static string GetParameterValueAsString(Element element, string paramName)
        {
            IList<Parameter> paramList = element.GetParameters(paramName);
            Parameter myParam = paramList.First();

            return myParam.AsString();

        }

        internal static void SetParameterValueAsString(Element element, string paramName, string value)
        {
            IList<Parameter> paramList = element.GetParameters(paramName);
            Parameter param = paramList.First();

            param.Set(value);

        }


        internal static List<ViewSheet> getAllSheets(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Sheets);

            List<ViewSheet> returnList = new List<ViewSheet>();
            foreach (ViewSheet curSheet in collector)
                returnList.Add(curSheet);

            return returnList;
        }
        internal static RibbonPanel CreateRibbonPanel(UIControlledApplication app, string tabName, string panelName)
        {
            RibbonPanel currentPanel = GetRibbonPanelByName(app, tabName, panelName);

            if (currentPanel == null)
                currentPanel = app.CreateRibbonPanel(tabName, panelName);

            return currentPanel;
        }

        internal static RibbonPanel GetRibbonPanelByName(UIControlledApplication app, string tabName, string panelName)
        {
            foreach (RibbonPanel tmpPanel in app.GetRibbonPanels(tabName))
            {
                if (tmpPanel.Name == panelName)
                    return tmpPanel;
            }

            return null;
        }

        internal static FamilySymbol GetFamilySymbolByName(Document doc, string famName, string fsName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(FamilySymbol));

            foreach (FamilySymbol fs in collector)
            {
                if (fs.Name == fsName && fs.FamilyName == famName)
                    return fs;
            }

            return null;
        }

       
        internal static void SetParameterValue(Element element, string paramName, string value)
        {
            IList<Parameter> paramlist = element.GetParameters(paramName);
            Parameter param = paramlist.First();

            param.Set(value);
        }

        internal static void SetParameterValue(Element element, string paramName, double value)
        {
            IList<Parameter> paramlist = element.GetParameters(paramName);
            Parameter param = paramlist.First();

            param.Set(value);

            // this type of method is called an overload, as it matches another with different arguments
        }

        internal static void SetParameterValue(Element element, string paramName, int value)
        {
            IList<Parameter> paramlist = element.GetParameters(paramName);
            Parameter param = paramlist.First();

            param.Set(value);
        }

        internal static int GetParameterValueAsInteger(Element element, string paramName)
        {
            IList<Parameter> paramList = element.GetParameters(paramName);
            Parameter occParam = paramList.First();

            return occParam.AsInteger();
        }
        internal static double GetParameterValueAsdouble(Element element, string paramName)
        {
            IList<Parameter> paramList = element.GetParameters(paramName);
            Parameter occParam = paramList.First();

            return occParam.AsDouble();
        }

        internal static string GetParameterValueAsValueString(Element element, string paramName)
        {
            IList<Parameter> paramList = element.GetParameters(paramName);
            Parameter occParam = paramList.First();

            return occParam.AsValueString();
            //returns as string the value with a consideration of the units!
        }

    }
}
