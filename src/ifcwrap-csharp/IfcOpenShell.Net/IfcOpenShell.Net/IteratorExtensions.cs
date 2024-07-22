using System.Collections.Generic;

namespace IfcOpenShell.Net
{
    public static class IteratorExtensions
    {
        // TODO WS for maintainer: this could probably be moved to the SWIG generation code by making
        // Iterator actually inherit from IEnumerable, but this is easier for now
        public static IEnumerable<Element> WrapAsEnumerable(this Iterator iterator)
        {
            if (!iterator.initialize())
                yield break;
            while (true)
            {
                var current = iterator.get();
                yield return current;
                
                var next = iterator.next();
                if (next == null)
                    break;
            }
        }
    }
}