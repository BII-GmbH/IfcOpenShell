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
%include "std_pair.i"
%include "exception.i"
%include <boost_shared_ptr.i>

%shared_ptr(aggregate_of_instance)

//%include <typemaps.i>

%rename(Equals) operator==;
%rename(LessThan) operator<;
%rename(Compare) operator();

%rename("Attribute") IfcParse::attribute;
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

// TODO: not sure if i want/need this to be partial, for now an extension method is enough
//%typemap(csclassmodifiers) IfcUtil::IfcBaseClass "public partial class"



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
	
	#include "../ifcparse/Argument.h"
	#include "../ifcparse/IfcBaseClass.h"
	#include "../ifcparse/IfcFile.h"
	#include "../ifcparse/IfcSchema.h"
	#include "../ifcparse/utils.h"

	#include "../svgfill/src/svgfill.h"

	#include <BRepTools_ShapeSet.hxx>
%}

// Create docstrings for generated python code.
%feature("autodoc", "1");

%include "impl/aggregate_of_instance.i"


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

	#include "../ifcparse/Argument.h"
	#include "../ifcparse/aggregate_of_instance.h"
	#include "../ifcparse/IfcBaseClass.h"
	#include "../ifcparse/IfcFile.h"
	#include "../ifcparse/IfcSchema.h"
	#include "../ifcparse/utils.h"

	#include "../svgfill/src/svgfill.h"

	#include <BRepTools_ShapeSet.hxx>
%}

%include "IfcGeomWrapper.i"
%include "IfcParseWrapper.i"

%include "typemaps.i"
// // TODO :sob:
// %define CUSTOM_OUTPUT_TYPEMAP(TYPE, CSTYPE)
// %typemap(cstype, out="CSTYPE") TYPE* "out CSTYPE"
// %typemap(csin,
// 	pre="    CSTYPE temp$csinput = new CSTYPE();",
//   	post="    $csinput = temp$csinput;"
// ) TYPE* "$csclassname.getCPtr(temp$csinput)"
// %enddef

%apply std::string { std::string* }

//CUSTOM_OUTPUT_TYPEMAP(std::string, string)

// %typemap(cstype) IfcParse::simple_type::data_type { string }

//%newobject std::string*
// %typemap(ctype, out="void *") std::string* OUTPUT "std::string *"
// %typemap(imtype, out="IntPtr") std::string* OUTPUT "out string"
// %typemap(cstype, out="string") std::string* OUTPUT "out string"
// %typemap(csin) std::string* OUTPUT "out $csinput"

//%apply double* OUTPUT { std::string* OUTPUT }

//%apply char* { std::string* OUTPUT };

%extend std::pair<IfcUtil::ArgumentType, Argument*> {

	bool try_get_as_string(std::string* OUTPUT) {
		const Argument& arg = *($self->second);
		const IfcUtil::ArgumentType type = $self->first;
		if(type == IfcUtil::Argument_ENUMERATION || type == IfcUtil::Argument_STRING)
		{			
			std::string* t = new std::string(arg);
			OUTPUT = t;
			return true;
		}
		return false;
	}
}


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

  %template(ArgumentByType) std::pair<IfcUtil::ArgumentType, Argument*>;

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
