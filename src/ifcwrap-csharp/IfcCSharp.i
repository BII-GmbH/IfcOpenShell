/********************************************************************************
 *                                                                              *
 * This file is part of IfcOpenShell.                                           *
 *                                                                              *
 * IfcOpenShell is free software: you can redistribute it and/or modify         *
 * it under the terms of the Lesser GNU General Public License as published by  *
 * the Free Software Foundation, either version 3.0 of the License, or          *
 * (at your option) any later version.                                          *
 *                                                                              *
 * IfcOpenShell is distributed in the hope that it will be useful,              *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 *
 * Lesser GNU General Public License for more details.                          *
 *                                                                              *
 * You should have received a copy of the Lesser GNU General Public License     *
 * along with this program. If not, see <http://www.gnu.org/licenses/>.         *
 *                                                                              *
 ********************************************************************************/

%begin %{
#ifdef _MSC_VER
# pragma warning(push)
# pragma warning(disable : 4127 4244 4702 4510 4512 4610)
# if _MSC_VER > 1800
#  pragma warning(disable : 4456 4459)
# endif
#endif
// TODO add '# pragma warning(pop)' to the very end of the file
%}

%include "stdint.i"
%include "std_array.i"
%include "std_vector.i"
%include "std_string.i"
%include "exception.i"
%include <boost_shared_ptr.i>

%shared_ptr(aggregate_of_instance)
%shared_ptr(aggregate_of_instance2)

//%apply boost::shared_ptr<aggregate_of_instance> { aggregate_of_instance::ptr }
//%apply boost::shared_ptr<aggregate_of_instance>* { aggregate_of_instance::ptr* }

%shared_ptr(RandomTestSubject)

%inline %{
#include <boost/shared_ptr.hpp>

struct RandomTestSubject {
	int someContent;

	
	typedef boost::shared_ptr<RandomTestSubject> Alias;
};


void takingRandomTest(RandomTestSubject subject) {

}

void takingRandomTestR(RandomTestSubject& subject) {

}
void takingRandomTestP(RandomTestSubject* subject) {

}
void takingRandomTestS(boost::shared_ptr<RandomTestSubject> subject) {

}


void takingRandomTestA(RandomTestSubject::Alias subject) {

}

RandomTestSubject returningRandomTest() {
	return {};
}

RandomTestSubject::Alias returningRandomTestA() {
	return {};
}

class aggregate_of_instance2;

template<typename T>
inline int RandomTemplateFunctionToMakeStuffMoreComplicated(typename T::Alias subject)
{
	return 0;
}

class aggregate_of_instance2 {
public:
	typedef boost::shared_ptr<aggregate_of_instance2> ptr;
	//typedef std::vector<IfcUtil::IfcBaseClass*>::const_iterator it;
	//inline void push(IfcUtil::IfcBaseClass* l){}
	inline void push(const ptr& l) {}
};
namespace IfcParse2 {
	class IfcFile2 {
	public:

		/// Returns all entities in the file that match the positional argument.
		/// NOTE: This also returns subtypes of the requested type, for example:
		/// IfcWall will also return IfcWallStandardCase entities
		//aggregate_of_instance2::ptr instances_by_type(const IfcParse::declaration*);

		/// Returns all entities in the file that match the positional argument.
		//aggregate_of_instance2::ptr instances_by_type_excl_subtypes(const IfcParse::declaration*);

		/// Returns all entities in the file that match the positional argument.
		/// NOTE: This also returns subtypes of the requested type, for example:
		/// IfcWall will also return IfcWallStandardCase entities
		inline aggregate_of_instance2::ptr instances_by_type(const std::string& t) {
			return {};
		}

	};
}

%}

//%include <typemaps.i>

%rename(Equals) operator==;
%rename(LessThan) operator<;
%rename(Compare) operator();
// TODO: operator()

%exception {
	try {
		$action
	} catch(const IfcParse::IfcAttributeOutOfRangeException& e) {
		SWIG_exception(SWIG_IndexError, e.what());
	} catch(const IfcParse::IfcException& e) {
		SWIG_exception(SWIG_RuntimeError, e.what());
	} catch(const std::runtime_error& e) {
		SWIG_exception(SWIG_RuntimeError, e.what());
	} catch(...) {
		SWIG_exception(SWIG_RuntimeError, "An unknown error occurred");
	}
}

%include "../serializers/serializers_api.h"
//%import "../ifcgeom_schema_agnostic/Serializer.h"

%typemap(csbase) IfcGeom::IteratorSettings::Settings "long"

// Include headers for the typemaps to function. This set of includes,
// can probably be reduced, but for now it's identical to the includes
// of the module definition below.
%{
	#include "../ifcgeom_schema_agnostic/IfcGeomIterator.h"
	#include "../ifcgeom_schema_agnostic/Serialization.h"
	#include "../ifcgeom_schema_agnostic/IfcGeomTree.h"

	#include "../serializers/SvgSerializer.h"
	#include "../serializers/WavefrontObjSerializer.h"
	#include "../serializers/HdfSerializer.h"
	
	#ifdef HAS_SCHEMA_2x3
		#include "../ifcparse/Ifc2x3.h"
	#endif
	#ifdef HAS_SCHEMA_4
		#include "../ifcparse/Ifc4.h"
	#endif
	#ifdef HAS_SCHEMA_4x1
		#include "../ifcparse/Ifc4x1.h"
	#endif
	#ifdef HAS_SCHEMA_4x2
		#include "../ifcparse/Ifc4x2.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_rc1
		#include "../ifcparse/Ifc4x3_rc1.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_rc2
		#include "../ifcparse/Ifc4x3_rc2.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_rc3
	#include "../ifcparse/Ifc4x3_rc3.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_rc4
	#include "../ifcparse/Ifc4x3_rc4.h"
	#endif
	#ifdef HAS_SCHEMA_4x3
	#include "../ifcparse/Ifc4x3.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_tc1
	#include "../ifcparse/Ifc4x3_tc1.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_add1
	#include "../ifcparse/Ifc4x3_add1.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_add2
	#include "../ifcparse/Ifc4x3_add2.h"
	#endif


	#include "../ifcparse/aggregate_of_instance.h"
	#include "../ifcparse/IfcBaseClass.h"
	#include "../ifcparse/IfcFile.h"
	#include "../ifcparse/IfcSchema.h"
	#include "../ifcparse/utils.h"

	#include "../svgfill/src/svgfill.h"

	#include <BRepTools_ShapeSet.hxx>
%}

// Create docstrings for generated python code.
%feature("autodoc", "1");

//%include "utils/type_conversion.i"
//%include "utils/typemaps_in.i"
%include "utils/typemaps_out.i"

%module ifcopenshell_net %{
	#include "../ifcgeom_schema_agnostic/IfcGeomIterator.h"
	#include "../ifcgeom_schema_agnostic/Serialization.h"
	#include "../ifcgeom_schema_agnostic/IfcGeomTree.h"

	#include "../serializers/SvgSerializer.h"
	#include "../serializers/WavefrontObjSerializer.h"
	#include "../serializers/HdfSerializer.h"
	#include "../serializers/XmlSerializer.h"
	#include "../serializers/GltfSerializer.h"
	
	#ifdef HAS_SCHEMA_2x3
		#include "../ifcparse/Ifc2x3.h"
	#endif
	#ifdef HAS_SCHEMA_4
		#include "../ifcparse/Ifc4.h"
	#endif
	#ifdef HAS_SCHEMA_4x1
		#include "../ifcparse/Ifc4x1.h"
	#endif
	#ifdef HAS_SCHEMA_4x2
		#include "../ifcparse/Ifc4x2.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_rc1
		#include "../ifcparse/Ifc4x3_rc1.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_rc2
		#include "../ifcparse/Ifc4x3_rc2.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_rc3
		#include "../ifcparse/Ifc4x3_rc3.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_rc4
		#include "../ifcparse/Ifc4x3_rc4.h"
	#endif
	#ifdef HAS_SCHEMA_4x3
		#include "../ifcparse/Ifc4x3.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_tc1
		#include "../ifcparse/Ifc4x3_tc1.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_add1
		#include "../ifcparse/Ifc4x3_add1.h"
	#endif
	#ifdef HAS_SCHEMA_4x3_add2
		#include "../ifcparse/Ifc4x3_add2.h"
	#endif

	#include "../ifcparse/aggregate_of_instance.h"
	#include "../ifcparse/IfcBaseClass.h"
	#include "../ifcparse/IfcFile.h"
	#include "../ifcparse/aggregate_of_instance.h"
	#include "../ifcparse/IfcSchema.h"
	#include "../ifcparse/utils.h"

	#include "../svgfill/src/svgfill.h"

	#include <BRepTools_ShapeSet.hxx>
%}

%include "IfcGeomWrapper.i"
%include "IfcParseWrapper.i"
	
namespace std {
  %template(Vec3) std::array<double, 3>;
  %template(Vec4) std::array<double, 4>;
  %template(FloatVector) std::vector<float>;
  %template(IntVector) std::vector<int>;
  %template(DoubleVector) std::vector<double>;
  %template(StringVector) std::vector<std::string>;
  %template(FloatVectorVector) std::vector<std::vector<float>>;
  %template(DoubleVectorVector) std::vector<std::vector<double>>;

  %template(MaterialVector) std::vector<IfcGeom::Material>;


  %template(EntityPtrList) std::vector<IfcUtil::IfcBaseClass*>;
}
// TODO: Decide if these should stay

%extend std::array<double, 3> {
	const double X;
	const double Y;
	const double Z;
}

// %extend Vec4 {
// 	const double X;
// 	const double Y;
// 	const double Z;
// 	const double W;
// }

%{

	double std_array_Sl_double_Sc_3_Sg__X_get(const std::array<double, 3>* vec) {
		return (*vec)[0];
	}

	double std_array_Sl_double_Sc_3_Sg__Y_get(const std::array<double, 3>* vec) {
		return (*vec)[1];
	}

	double std_array_Sl_double_Sc_3_Sg__Z_get(const std::array<double, 3>* vec) {
		return (*vec)[2];
	}

	// double Vec4_X_get(const Vec4* vec) {
	// 	return (*vec)[0];
	// }

	// double Vec4_Y_get(const Vec4* vec) {
	// 	return (*vec)[1];
	// }

	// double Vec4_Z_get(const Vec4* vec) {
	// 	return (*vec)[2];
	// }
	// double Vec4_W_get(const Vec4* vec) {
	// 	return (*vec)[3];
	// }
%}
