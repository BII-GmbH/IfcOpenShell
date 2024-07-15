// See https://aka.ms/new-console-template for more information

using System.Collections.Immutable;
using System.Diagnostics;
using BII.Common.Import.IfcOpenShell;
using IfcOpenShell;


Console.WriteLine("Hello, World!");
var i = Console.Read();

//const string fileName = "I:\\MPAL VA Gleise + IRA-Bahnsteig_1023-1.IFC";
const string fileName = "C:\\LocalDirs\\schnepp\\IfcOpenShell-Progress\\sampleHouse.IFC";


var model = ifcopenshell_net.open(fileName);
var settings = new Settings();
settings.Set("generate-uvs", true);
settings.Set("weld-vertices", false);
settings.Set("no-normals", false);
var iterator = new Iterator("opencascade", settings, model, Environment.ProcessorCount);
//using var task = Task.Run(() =>
//var task = async () =>
//{
    try
    {


        Stopwatch watch = Stopwatch.StartNew();
        Console.WriteLine($"Importing with {Environment.ProcessorCount}");
        var defaultUnitsInIfc = ImmutableDictionary.CreateRange(
            model.ByType("IfcUnitAssignment")
                .SelectMany(ass => ass?.GetAttributeAsEntityList("Units"))
                .Where(e => e != null)
                .Select(unit =>
                {
                    var t = unit.GetAttributeAsString("UnitType");
                    var scale = model.CalculateUnitScale(t);
                    var unitSymbol = UnitUtils.GetUnitSymbol(unit);
                    return (t, new UnitInformation(scale, unitSymbol));
                })
                .Select(e => new KeyValuePair<string, UnitInformation>(e.Item1, e.Item2))
        );

        {

            foreach (var elem in iterator.WrapAsEnumerable())
            {
                Console.WriteLine($"Progress: {iterator.progress()} after {watch.Elapsed}");
                // TODO: figure out if this can help us get accurate bounding boxes for the products
                //iterator.compute_bounds()

                var productId = elem.product().Guid() ?? "(Unknown)";

                var elem_instance = elem.product();
                var product = elem.product();

                var productPropertyInfo = ImmutableDictionary.CreateRange<string, string>(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Name",
                            product.TryGetAttributeAsString("Name", out var name) ? name : productId),
                        new KeyValuePair<string, string>("Id", product.id().ToString()),
                        new KeyValuePair<string, string>("Guid", productId),
                        new KeyValuePair<string, string>("Type", product.Is())
                    }
                );

                var psets = elem_instance.GetPsets();

                var matrix = elem.transformation().data().Col0;


                // if (matrix.Count != 12 && matrix.Count != 16) {
                //     continue;
                // }

                TriangulationElement shape = elem.TryGetAsTriangulationElement();
                if (shape == null)
                {
                    continue;
                }

                var geometry = shape.geometry();

                var shapeId = shape.id();
                // unique id for the geometry _intended_ to be used for caching/instancing of the mesh
                var geometryId = geometry.id();

                // choose our own ids, since the ones supplied by ifcopenshell do not seem to be unique
                var materials = geometry.materials().First();

                var res = psets.Select(
                        kvp =>
                        {
                            var r = kvp.Value
                                .Select(
                                    p =>
                                    {
                                        var vs = IfcOpenShellUtils.propertyValueToString(defaultUnitsInIfc, p.Value);
                                        if (vs != null) return (Name: p.Key, Value: vs);
                                        else return default;
                                    })
                                .Where(t => t != default)
                                .Distinct()
                                .ToImmutableDictionary();
                            return (kvp.Key, r);
                        }
                    )
                    .ToImmutableDictionary();

            }
        }
        Console.WriteLine($"Everything took {watch.Elapsed}");
    }
    catch (Exception e)
    {
        Console.Error.WriteLine(e);
        return -1;
    }

    return 0;
//};
//});
//var taskResult = await task();
// iterator.Dispose();
// model.Dispose();
//
// Console.WriteLine($"Task completed with result {taskResult}");





