using System.Collections.Immutable;
using System.Diagnostics;
using IfcOpenShell;
using IfcOpenShell.Net;

// NOTE WS: This is a set of random code that was useful for testing while initially creating the C# wrapper.
// It is not intended to be used as an actual example application for new users or a
// functionality test application.

Console.WriteLine("Hello, World!");
// wait for user input before starting - useful for profiling
var i = Console.Read();

// random test files that i found on my PC - paths probably wont work for you
const string fileName = "sampleHouse.IFC";

var model = ifcopenshell_net.open(fileName);

var settings = new Settings();
settings.Set("generate-uvs", true);
settings.Set("weld-vertices", false);
settings.Set("no-normals", false);
var iterator = new Iterator("opencascade", settings, model, Environment.ProcessorCount);
var watch = Stopwatch.StartNew();
try
{
    // trying to read units from the file
    Console.WriteLine($"Importing with {Environment.ProcessorCount}");
    var defaultUnitsInIfc = ImmutableDictionary.CreateRange(
        model.ByType("IfcUnitAssignment")
            .SelectMany(ass => ass?.GetAttributeAsEntityList("Units") ?? Array.Empty<EntityInstance>())
            .Select(unit =>
            {
                var t = unit.GetAttributeAsString("UnitType");
                var scale = model.CalculateUnitScale(t);
                var unitSymbol = UnitUtils.GetUnitSymbol(unit);
                return (t, (scale, unitSymbol));
            })
            .Where(e => e.Item1 != null)
            .Select(e => new KeyValuePair<string, (double, string)>(e.Item1!, e.Item2))
    );

    {

        foreach (var elem in iterator.WrapAsEnumerable())
        {
            Console.WriteLine($"Progress: {iterator.progress()} after {watch.Elapsed}");
            
            var productId = elem.product().Guid() ?? "(Unknown)";

            var elem_instance = elem.product();
            var product = elem.product();
            
            var psets = elem_instance.GetPsets();

            var matrix = elem.transformation().data().Col0;
            
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




