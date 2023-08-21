#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Autodesk.Revit.DB.Structure;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using static ArchSmarter_Addin_Bootcamp_v2.Module03Challenge;

#endregion

namespace ArchSmarter_Addin_Bootcamp_v2
{
    [Transaction(TransactionMode.Manual)]
    public class Module03Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)


        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            
            //create a list of furniture types + sets
            List<FurnitureType> fTypeList = new List<FurnitureType>();
            List<FurnitureSet> fSetList = new List<FurnitureSet>();
                                                                                
            //!!!!!GET ALL FURNITURE TYPES
            //Get furniture types as Stringarray 
            IList<string[]> typeArray = GetFurnitureTypes();
            //getting the furniture from the furniture types list. 
            typeArray.RemoveAt(0);

            foreach (string[] fs in typeArray) 
            {
                // get each string list in the tuple
                //[{"side chair", "Chair-Breuer", "Chair-Breuer"},{"desk", "Desk", "60in x 30in"}]
                foreach (string f in fs)
                {            
                    f.Trim();                                  
                }
                // grab the furniure name, family name, and list of pieces
                string FuName = fs[0];
                string FaName = fs[1];
                string FType = fs[2];

                FurnitureType ZZ = new FurnitureType(FuName, FaName, FType);
                // M - FurnitureType ZZ = new FurnitureType(fs[0], fs[1], fs[2]);
                fTypeList.Add(ZZ);
            }
            //remove first line as that is the titles

            
            //!!!! GET ALL FURNITURE SETS
            //get furniture sets as array
            IList<string[]> setArray = GetFurnitureSets();
            //getting the furniture from the furniture types list. 
            setArray.RemoveAt(0);
            foreach (string[] fs in setArray)
            {
                // "Furniture Set", "Room Type", "Included Furniture" 
                // "A", "Office", ["desk, task chair, side chair, bookcase"]
                string FuSet = fs[0];
                string roomType = fs[1];
                string sList = fs[2];

                //make a list to store the list of furniture as a string
                //List<string> STlist = new List<string>();
                
                string[] SArray = sList.Split(',');
                
                
                List<FurnitureType> ZZlist = new List<FurnitureType>();
                // build list of Furniture in string

                foreach (string s in SArray)
                {
                    foreach (FurnitureType XX in fTypeList) 
                    {
                        if (s.Trim() == XX.Name)
                        {
                            ZZlist.Add(XX);
                        } 
                    }
                }
                
                //array of strings, convert strings into FurnitureTypes by name
                //name of set, name of room, list of Furniture Names
                FurnitureSet ZZ = new FurnitureSet(FuSet, roomType, ZZlist);
                fSetList.Add(ZZ);

            }

            //!!! GET ALL ROOMS AND PLACE FURNITURE SET IN THE ROOM 
            //note that rooms is not a class is it a part of the SpatialElement class
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Rooms);

            FilteredElementCollector Famcollector = new FilteredElementCollector(doc);
            Famcollector.OfCategory(BuiltInCategory.OST_Furniture).OfClass(typeof(FamilySymbol));

            using (Transaction t = new Transaction(doc))
            {
                t.Start("create revit elements by room");
                {
                    // active family symbol, sometimes the family is not active in the memory
                    //familysymbol.Activate();
                    
                    foreach (SpatialElement room in collector)
                    {
                        List<FamilySymbol> families = new List<FamilySymbol>();
                        List<FurnitureType> FTList = new List<FurnitureType>();
                        
                        string CurRoomSet = Utils.GetParameterValueAsString(room, "Furniture Set");

                        //find the matching furniture set
                        foreach (FurnitureSet FS in fSetList)
                        {//if the furnitureset name and roomtype is equal to the current room
                            if (CurRoomSet == FS.Name)
                            {   // get the furniture types from the includedfurniturelist
                                FTList = FS.IncludedFurnitureList;

                                //now we have a list of furnituretypes, we need to get the familysymbols that match
                                foreach (FurnitureType FT in FTList)
                                {
                                    string famName = FT.Family;//family name as string
                                    string fsName = FT.Type;//FamilySymbol Name as string

                                    foreach (FamilySymbol FamSym in Famcollector)
                                    {                                        
                                        string FamSymType = FamSym.Name;

                                        if (famName == FamSym.FamilyName && fsName == FamSym.Name)
                                        {
                                            FamilySymbol ss = Utils.GetFamilySymbolByName(doc, famName, fsName);

                                            if (ss != null)
                                            {
                                                if(ss.IsActive == false)
                                                {
                                                    ss.Activate();
                                                }
                                            }

                                            families.Add(ss);

                                        }
                                    }
                                }
                                //locations can be a location point or curve, it depends what your using
                                //use lookup to check which type of location the element has
                                LocationPoint loc = room.Location as LocationPoint;
                                XYZ roomPoint = loc.Point as XYZ;

                                foreach (FamilySymbol Sym in families)
                                {
                                    FamilyInstance curFI = doc.Create.NewFamilyInstance(roomPoint, Sym, StructuralType.NonStructural);
                                }
                                
                            }
                        }
                    }
                }
                t.Commit();
            }

            return Result.Succeeded;
        }


        public class FurnitureType
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Family { get; set; }

            // add constructor to class
            public FurnitureType(string _name,  string _family, string _type)
            {             
                Name = _name;
                Family = _family;
                Type = _type;
            }
            
        }


        public class FurnitureSet
        {
            public string Name { get; set; }
            public string RoomType { get; set; }
            public List<FurnitureType> IncludedFurnitureList { get; set; }
            public FurnitureSet(string _name, string _roomType, List<FurnitureType> _includedFurnitureList)
            {
                Name = _name;
                RoomType = _roomType;
                IncludedFurnitureList = _includedFurnitureList;
            }
            //michael set this not as a list but as a string and then pulled out the elements within the constructor of the class
            public int getIncFurnitureCount()
            {
                return IncludedFurnitureList.Count;
            }            

        }   

        private List<string[]> GetFurnitureTypes()
        {
            List<string[]> returnList = new List<string[]>();
            returnList.Add(new string[] { "Furniture Name", "Revit Family Name", "Revit Family Type" });
            returnList.Add(new string[] { "desk", "Desk", "60in x 30in" });
            returnList.Add(new string[] { "task chair", "Chair-Task", "Chair-Task" });
            returnList.Add(new string[] { "side chair", "Chair-Breuer", "Chair-Breuer" });
            returnList.Add(new string[] { "bookcase", "Shelving", "96in x 12in x 84in" });
            returnList.Add(new string[] { "loveseat", "Sofa", "54in" });
            returnList.Add(new string[] { "teacher desk", "Table-Rectangular", "48in x 30in" });
            returnList.Add(new string[] { "student desk", "Desk", "60in x 30in Student" });
            returnList.Add(new string[] { "computer desk", "Table-Rectangular", "48in x 30in" });
            returnList.Add(new string[] { "lab desk", "Table-Rectangular", "72in x 30in" });
            returnList.Add(new string[] { "lounge chair", "Chair-Corbu", "Chair-Corbu" });
            returnList.Add(new string[] { "coffee table", "Table-Coffee", "30in x 60in x 18in" });
            returnList.Add(new string[] { "sofa", "Sofa-Corbu", "Sofa-Corbu" });
            returnList.Add(new string[] { "dining table", "Table-Dining", "30in x 84in x 22in" });
            returnList.Add(new string[] { "dining chair", "Chair-Breuer", "Chair-Breuer" });
            returnList.Add(new string[] { "stool", "Chair-Task", "Chair-Task" });

            return returnList;
        }

        private List<string[]> GetFurnitureSets()
        {
            List<string[]> returnList = new List<string[]>();
            returnList.Add(new string[] { "Furniture Set", "Room Type", "Included Furniture" });
            returnList.Add(new string[] { "A", "Office", "desk, task chair, side chair, bookcase" });
            returnList.Add(new string[] { "A2", "Office", "desk, task chair, side chair, bookcase, loveseat" });
            returnList.Add(new string[] { "B", "Classroom - Large", "teacher desk, task chair, student desk, student desk, student desk, student desk, student desk, student desk, student desk, student desk, student desk, student desk, student desk, student desk" });
            returnList.Add(new string[] { "B2", "Classroom - Medium", "teacher desk, task chair, student desk, student desk, student desk, student desk, student desk, student desk, student desk, student desk" });
            returnList.Add(new string[] { "C", "Computer Lab", "computer desk, computer desk, computer desk, computer desk, computer desk, computer desk, task chair, task chair, task chair, task chair, task chair, task chair" });
            returnList.Add(new string[] { "D", "Lab", "teacher desk, task chair, lab desk, lab desk, lab desk, lab desk, lab desk, lab desk, lab desk, stool, stool, stool, stool, stool, stool, stool" });
            returnList.Add(new string[] { "E", "Student Lounge", "lounge chair, lounge chair, lounge chair, sofa, coffee table, bookcase" });
            returnList.Add(new string[] { "F", "Teacher's Lounge", "lounge chair, lounge chair, sofa, coffee table, dining table, dining chair, dining chair, dining chair, dining chair, bookcase" });
            returnList.Add(new string[] { "G", "Waiting Room", "lounge chair, lounge chair, sofa, coffee table" });

            return returnList;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnModule03Challenge";
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
