%include <std_common.i>
%{
    #include "../ifcparse/IfcBaseClass.h"
%}

// make the wrapper class for aggregrate_of_instance inherit from additional C# interfaces - in particular IEnumerable & IReadOnlyList
%typemap(csinterfaces) aggregate_of_instance "global::System.IDisposable, global::System.Collections.IEnumerable, global::System.Collections.Generic.IEnumerable<$typemap(cstype, IfcUtil::IfcBaseClass*)>, global::System.Collections.Generic.IReadOnlyList<$typemap(cstype, IfcUtil::IfcBaseClass*)>\n"

// ignore begin & end methods - these return an iterator, which would be really awkward to wrap
%ignore aggregate_of_instance::begin;
%ignore aggregate_of_instance::end;

// rename & make the operator[] private
%rename("getitem") aggregate_of_instance::operator[];
%csmethodmodifiers aggregate_of_instance::operator[] "private"

// adding additional methods & nested classes to the generated class for aggregate_of_instance - these are the methods required for implementing the additional interfaces defined above
%typemap(cscode) aggregate_of_instance %{
    
    public bool IsEmpty => Count > 0;

    public int Count => (int)size();

    public $typemap(cstype, IfcUtil::IfcBaseClass*) this[int index]  {
        get {
            if (index>=0 && index < (int)size())
                return getitem(index);
            throw new global::System.IndexOutOfRangeException($"Index {index} outside of valid range 0-{(int)size()}");
        }
    }

    global::System.Collections.Generic.IEnumerator<$typemap(cstype, IfcUtil::IfcBaseClass*)> global::System.Collections.Generic.IEnumerable<$typemap(cstype, IfcUtil::IfcBaseClass*)>.GetEnumerator() {
        return new $csclassnameEnumerator(this);
    }

    global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() {
        return new $csclassnameEnumerator(this);
    }

    /// Type-safe enumerator
    /// Note that the IEnumerator documentation requires an InvalidOperationException to be thrown
    /// whenever the collection is modified. This has been done for changes in the size of the
    /// collection but not when one of the elements of the collection is modified as it is a bit
    /// tricky to detect unmanaged code that modifies the collection under our feet.
    public sealed class $csclassnameEnumerator : global::System.Collections.IEnumerator
        , global::System.Collections.Generic.IEnumerator<$typemap(cstype, IfcUtil::IfcBaseClass*)>
    {
        private $csclassname collectionRef;
        private int currentIndex;
        private object currentObject;
        private int currentSize;

        public $csclassnameEnumerator($csclassname collection) {
        collectionRef = collection;
        currentIndex = -1;
        currentObject = null;
        currentSize = collectionRef.Count;
        }

        // Type-safe iterator Current
        public $typemap(cstype, IfcUtil::IfcBaseClass*) Current {
            get {
                if (currentIndex == -1)
                throw new global::System.InvalidOperationException("Enumeration not started.");
                if (currentIndex > currentSize - 1)
                throw new global::System.InvalidOperationException("Enumeration finished.");
                if (currentObject == null)
                throw new global::System.InvalidOperationException("Collection modified.");
                return ($typemap(cstype, IfcUtil::IfcBaseClass*))currentObject;
            }
        }

        // Type-unsafe IEnumerator.Current
        object global::System.Collections.IEnumerator.Current {
            get {
                return Current;
            }
        }

        public bool MoveNext() {
            int size = collectionRef.Count;
            bool moveOkay = (currentIndex+1 < size) && (size == currentSize);
            if (moveOkay) {
                currentIndex++;
                currentObject = collectionRef[currentIndex];
            } else {
                currentObject = null;
            }
            return moveOkay;
        }

        public void Reset() {
            currentIndex = -1;
            currentObject = null;
            if (collectionRef.Count != currentSize) {
                throw new global::System.InvalidOperationException("Collection modified.");
            }
        }

        public void Dispose() {
            currentIndex = -1;
            currentObject = null;
        }
    }
%}