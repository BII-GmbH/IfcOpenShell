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
#if defined(_DEBUG) && defined(SWIG_PYTHON_INTERPRETER_NO_DEBUG)
/* https://github.com/swig/swig/issues/325 */
# include <basetsd.h>
# include <assert.h>
# include <ctype.h>
# include <errno.h>
# include <io.h>
# include <math.h>
# include <sal.h>
# include <stdarg.h>
# include <stddef.h>
# include <stdio.h>
# include <stdlib.h>
# include <string.h>
# include <sys/stat.h>
# include <time.h>
# include <wchar.h>
#endif

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
%include "utils/readonly_vector.i"
%include "std_string.i"
%include "std_pair.i"
%include "exception.i"
%include "std_shared_ptr.i"

%ignore IfcGeom::NumberNativeDouble;
%ignore ifcopenshell::geometry::Converter;

// General python-specific rename rules for comparison operators.
// Mostly to silence warnings, but might be of use some time.
%rename(Equals) operator==;
%rename(LessThan) operator<;
%rename(Compare) operator();

%rename("Attribute") IfcParse::attribute;

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

%typemap(csbase) IfcGeom::IteratorSettings::Settings "long"

// Include headers for the typemaps to function. This set of includes,
// can probably be reduced, but for now it's identical to the includes
// of the module definition below.
%{
	#include "../ifcgeom/Iterator.h"
	#include "../ifcgeom/taxonomy.h"
#ifdef IFOPSH_WITH_OPENCASCADE
	#include "../ifcgeom/Serialization/Serialization.h"
	#include "../ifcgeom/kernels/opencascade/IfcGeomTree.h"

	#include <BRepTools_ShapeSet.hxx>
#endif

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

	#include "../ifcgeom/ConversionSettings.h"
	#include "../ifcgeom/ConversionResult.h"

	#include "../svgfill/src/svgfill.h"
#ifdef IFOPSH_WITH_CGAL
	#include "../ifcgeom/kernels/cgal/CgalConversionResult.h"
#endif
%}

// Create docstrings for generated python code.
%feature("autodoc", "1");

%shared_ptr(aggregate_of_instance)
%include "utils/aggregate_of_instance.i"
%include "utils/typemaps_out.i"

%module ifcopenshell_net %{
	#include "../ifcgeom/Converter.h"
	#include "../ifcgeom/taxonomy.h"
#ifdef IFOPSH_WITH_OPENCASCADE
	#include "../ifcgeom/Serialization/Serialization.h"
	#include "../ifcgeom/kernels/opencascade/IfcGeomTree.h"

	#include <BRepTools_ShapeSet.hxx>
#endif
	#include "../ifcgeom/Iterator.h"
	#include "../ifcgeom/ConversionResult.h"

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

	#include "../ifcparse/ArgumentType.h"
	#include "../ifcparse/Argument.h"
	#include "../ifcparse/aggregate_of_instance.h"
	#include "../ifcparse/IfcBaseClass.h"
	#include "../ifcparse/IfcFile.h"
	#include "../ifcparse/IfcSchema.h"
	#include "../ifcparse/utils.h"
	
	#include "../ifcgeom/ConversionSettings.h"
	#include "../ifcgeom/ConversionResult.h"

	#include "../svgfill/src/svgfill.h"
%}

%include "IfcGeomWrapper.i"
%include "IfcParseWrapper.i"
	
%inline %{
	
	template<typename T>
	struct TypedArgument {

		virtual ~TypedArgument() = default;

		virtual bool HasValue() const = 0;
		virtual T GetValue() const = 0;
	};

	template<typename T>
	struct WithValue : public TypedArgument<T> {
		explicit WithValue(const T& arg) : value(arg) {}
		explicit WithValue(const T&& arg) : value(std::move(arg)) {}

		inline bool HasValue() const override {
			return true;
		}

		inline T GetValue() const override { 
			return value;
		}

		private:
		T value;
	};

	template <typename T>
	struct NoValue : public TypedArgument<T> {
		inline bool HasValue() const override { return false; }
		inline T GetValue() const override { 
			throw std::runtime_error("Has no value");
		}
	};

%}


%apply const std::string& { std::string* }

namespace std {
  %template(Vec3) std::array<double, 3>;
  %template(Vec4) std::array<double, 4>;
  %template(FloatVector) std::vector<float>;
  %template(IntVector) std::vector<int>;
  %template(DoubleVector) std::vector<double>;
  %template(StringVector) std::vector<std::string>;
  %template(FloatVectorVector) std::vector<std::vector<float>>;
  %template(DoubleVectorVector) std::vector<std::vector<double>>;

  %template(MaterialVector) std::vector<std::shared_ptr<ifcopenshell::geometry::taxonomy::style>>;

  %template(ArgumentByType) std::pair<IfcUtil::ArgumentType, Argument*>;

  %template(EntityPtrList) std::vector<IfcUtil::IfcBaseClass*>;
}

%extend std::array<double, 3> {
	const double X;
	const double Y;
	const double Z;

	%newobject New;
	static std::array<double,3>* New(double x, double y, double z) {
		return new std::array<double,3> {x,y,z};
	}
}

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
%}

%extend std::array<double, 4> {
	const double X;
	const double Y;
	const double Z;
	const double W;

	%newobject New;
	static std::array<double,4>* New(double x, double y, double z, double w) {
		return new std::array<double,4> {x,y,z,w};
	}
}

%{
	double std_array_Sl_double_Sc_4_Sg__X_get(const std::array<double, 4>* vec) {
		return (*vec)[0];
	}

	double std_array_Sl_double_Sc_4_Sg__Y_get(const std::array<double, 4>* vec) {
		return (*vec)[1];
	}

	double std_array_Sl_double_Sc_4_Sg__Z_get(const std::array<double, 4>* vec) {
		return (*vec)[2];
	}

	double std_array_Sl_double_Sc_4_Sg__W_get(const std::array<double, 4>* vec) {
		return (*vec)[3];
	}

	
%}




// TODO: Check if this is even still required. Maybe we can instead rename the Argument overloaded operator()s
%define TRY_GET_AS(TYPE, NAME, TYPECHECK)
	%extend std::pair<IfcUtil::ArgumentType, Argument*> {
		
		%newobject TryGetAsString;
		TypedArgument<TYPE>* TryGetAs##NAME() {
			const Argument& arg = *($self->second);
			const IfcUtil::ArgumentType type = $self->first;
			if(TYPECHECK)
			{			
				TYPE tmp = arg;
				return new WithValue<TYPE>(tmp);
			}
			return new NoValue<TYPE>();
		}
	}

	%template(NAME##Argument) TypedArgument<TYPE>;
%enddef

	// TODO: LOGICAL (whatever that is)
	// TODO: BINARY
TRY_GET_AS(std::string, String, type == IfcUtil::Argument_ENUMERATION || type == IfcUtil::Argument_STRING)
TRY_GET_AS(int, Int, type == IfcUtil::Argument_INT)
TRY_GET_AS(bool, Bool, type == IfcUtil::Argument_BOOL)
//TRY_GET_AS(std::uint8_t, Binary, type == IfcUtil::Argument_BINARY)
TRY_GET_AS(double, Double, type == IfcUtil::Argument_DOUBLE)
TRY_GET_AS(IfcUtil::IfcBaseClass*, Entity, type == IfcUtil::Argument_ENTITY_INSTANCE)
TRY_GET_AS(std::vector<int>, IntList, type == IfcUtil::Argument_AGGREGATE_OF_INT)
//TRY_GET_AS(std::vector<std::uint8_t>, BinaryList, type == IfcUtil::Argument_AGGREGATE_OF_BINARY)
TRY_GET_AS(std::vector<double>, DoubleList, type == IfcUtil::Argument_AGGREGATE_OF_DOUBLE)
TRY_GET_AS(std::vector<std::string>, StringList, type == IfcUtil::Argument_AGGREGATE_OF_STRING)
TRY_GET_AS(aggregate_of_instance::ptr, EntityList, type == IfcUtil::Argument_AGGREGATE_OF_ENTITY_INSTANCE)
