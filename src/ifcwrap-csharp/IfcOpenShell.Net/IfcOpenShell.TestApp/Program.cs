// See https://aka.ms/new-console-template for more information

using System.Collections.Immutable;
using System.Diagnostics;
using IfcOpenShell;


Console.WriteLine("Hello, World!");
var i = Console.Read();

const string fileName = "I:\\MPAL VA Gleise + IRA-Bahnsteig_1023-1.IFC";

using var model = ifcopenshell_net.open(fileName);

var settings = new IteratorSettings();
settings.set((ulong)IteratorSettings.Setting.GENERATE_UVS, true);
settings.set((ulong)IteratorSettings.Setting.WELD_VERTICES, false);
settings.set((ulong)IteratorSettings.Setting.NO_NORMALS, false);

Stopwatch watch = Stopwatch.StartNew();
Console.WriteLine($"Importing with {Environment.ProcessorCount}");
var iterator = new Iterator(settings, model, Environment.ProcessorCount);

if (iterator.initialize()) {
    while (true) {
        Console.WriteLine($"Progress: {iterator.progress()} after {watch.Elapsed}");
        // TODO: figure out if this can help us get accurate bounding boxes for the products
        //iterator.compute_bounds()
        var elem = iterator.get();

        var productId = elem.product().Guid() ?? "(Unknown)";

        var elem_instance = elem.product();
        var product = elem.product();
        
        var productPropertyInfo = ImmutableDictionary.CreateRange<string, string>(
            new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("Name", product.TryGetAttributeAsString("Name", out var name) ? name : productId),
                new KeyValuePair<string, string>("Id", product.id().ToString()),
                new KeyValuePair<string, string>("Guid", productId),
                new KeyValuePair<string, string>("Type", product.is_a())
                }
            );
        
        var psets = elem_instance.GetPsets();
        
        var matrix = elem.transformation().matrix().data();
        
        
        if (matrix.Count != 12 && matrix.Count != 16) {
            continue;
        }

        TriangulationElement shape = elem.TryGetAsTriangulationElement();
        if (shape == null) {
            continue;
        }
        var geometry = shape.geometry();
        var shapeId = shape.id();
        // unique id for the geometry _intended_ to be used for caching/instancing of the mesh
        var geometryId = geometry.id();
        
        // choose our own ids, since the ones supplied by ifcopenshell do not seem to be unique
        var materials = geometry.materials();
        
        
        if (iterator.next() == null) 
            break;
    }
    
    
    //Debug.Log($"Processed {vertices.Count} vertices in the model");
}
Console.WriteLine($"Everything took {watch.Elapsed}");





