// See https://aka.ms/new-console-template for more information
using IfcOpenShell;


Console.WriteLine("Hello, World!");

const string fileName = "C:\\LocalDirs\\schnepp\\IfcOpenShell-Progress\\sampleHouse.ifc";

using var model = ifcopenshell_net.open(fileName);

var wall = model.instances_by_type("IfcWall").First();
var elem = model.instance_by_guid("3w0zWKm7n8SB1qbfwUzt0J");
var pset = elem.GetPsets();
Console.WriteLine(wall);

var settings = new IteratorSettings();
settings.set((ulong)IteratorSettings.Setting.GENERATE_UVS, true);
settings.set((ulong)IteratorSettings.Setting.WELD_VERTICES, false);
settings.set((ulong)IteratorSettings.Setting.NO_NORMALS, false);

// var wall = model.instances_by_type("IfcWall")[0];
// var name = wall.TryGetAttribute("Name").TryGetAsString().TryGetValue(out var nameAtt) ? nameAtt : "None";   
// Console.WriteLine("Name: " + name);
//
// var name2 = wall
//     .TryGetAttribute("Namelaskj", out var name2Attribute) && 
//     name2Attribute.TryGetAsString().HasValue() ? name2Attribute.TryGetAsString().getValue() : "None";
// Console.WriteLine("Namelaskj: " + name2);
Console.WriteLine($"Ifc {model.schema_name()} has map conversion implemented natively: {!model.schema_name().Equals("IFC2X3")}");

// var ifcMapConversion = model.instances_by_type("IfcMapConversion")?.FirstOrDefault();
//             
// if (ifcMapConversion != null) {
//                 
//     var mapConversion = ifcMapConversion.get_attribute_names();  
//     foreach (var attribute in mapConversion) {
//         Console.WriteLine($"Found attribute on map conversion: {attribute}");
//     }
//             
//     //var eastings = ifcMapConversion.TryGetAttribute("XAxisOrdinate").TryGetAsDouble().HasValue() ? ifcMapConversion.TryGetAttribute("XAxisOrdinate").TryGetAsDouble().GetValue() : -1.0;
//     //eastings.second
//                     
//     //Console.WriteLine($"XAxisOrdinate: {eastings}");
// }
                
// var projectedCrs = Option.SomeIfNotNull(model.instances_by_type("IfcProjectedCRS"))
//     .FlatMap(l => l.FirstOrNone());
// if (projectedCrs.TryGetValue(out var projected)) { Debug.Log($"Found projected CRS: {projected}"); }



