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

%rename("buffer") stream_or_filename;

%ignore stream_or_filename::stream;

%ignore IfcGeom::impl::tree::selector;

%include "../ifcgeom_schema_agnostic/ifc_geom_api.h"
%include "../ifcgeom_schema_agnostic/IfcGeomIteratorSettings.h"
%include "../ifcgeom_schema_agnostic/IfcGeomElement.h"
%include "../ifcgeom_schema_agnostic/IfcGeomMaterial.h"
%include "../ifcgeom_schema_agnostic/IfcGeomRepresentation.h"
%include "../ifcgeom_schema_agnostic/IfcGeomIterator.h"
%include "../ifcgeom_schema_agnostic/GeometrySerializer.h"

%extend IfcGeom::Element* {
	bool IfcGeom_Element_TryGetAsSerializedElement(IfcGeom::Element* elem, IfcGeom::SerializedElement* serializedElement)
	{
		serializedElement = nullptr;
		serializedElement = dynamic_cast<IfcGeom::SerializedElement*>(elem);
		return serializedElement != nullptr;
	}

	bool IfcGeom_Element_TryGetAsTriangulationElement(IfcGeom::Element* elem, IfcGeom::TriangulationElement* serializedElement)
	{
		serializedElement = nullptr;
		serializedElement = dynamic_cast<IfcGeom::TriangulationElement*>(elem);
		return serializedElement != nullptr;
	}

	bool IfcGeom_Element_TryGetAsBRepElement(IfcGeom::Element* elem, IfcGeom::BRepElement* serializedElement)
	{
		serializedElement = nullptr;
		serializedElement = dynamic_cast<IfcGeom::BRepElement*>(elem);
		return serializedElement != nullptr;
	}

}


%include "../serializers/SvgSerializer.h"
%include "../serializers/HdfSerializer.h"
%include "../serializers/WavefrontObjSerializer.h"
%include "../serializers/XmlSerializer.h"
%include "../serializers/GltfSerializer.h"

%template(ray_intersection_results) std::vector<IfcGeom::ray_intersection_result>;

// A Template instantantation should be defined before it is used as a base class. 
// But frankly I don't care as most methods are subtlely different anyway.
%include "../ifcgeom_schema_agnostic/IfcGeomTree.h"

%extend IfcGeom::tree {

	static aggregate_of_instance::ptr vector_to_list(const std::vector<IfcUtil::IfcBaseEntity*>& ps) {
		aggregate_of_instance::ptr r(new aggregate_of_instance);
		for (std::vector<IfcUtil::IfcBaseEntity*>::const_iterator it = ps.begin(); it != ps.end(); ++it) {
			r->push(*it);
		}
		return r;
	}

	aggregate_of_instance::ptr select_box(IfcUtil::IfcBaseClass* e, bool completely_within = false, double extend=-1.e-5) const {
		if (!e->declaration().is("IfcProduct")) {
			throw IfcParse::IfcException("Instance should be an IfcProduct");
		}
		std::vector<IfcUtil::IfcBaseEntity*> ps = $self->select_box((IfcUtil::IfcBaseEntity*)e, completely_within, extend);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select_box(const gp_Pnt& p) const {
		std::vector<IfcUtil::IfcBaseEntity*> ps = $self->select_box(p);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select_box(const Bnd_Box& b, bool completely_within = false) const {
		std::vector<IfcUtil::IfcBaseEntity*> ps = $self->select_box(b, completely_within);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select(IfcUtil::IfcBaseClass* e, bool completely_within = false, double extend = 0.0) const {
		if (!e->declaration().is("IfcProduct")) {
			throw IfcParse::IfcException("Instance should be an IfcProduct");
		}
		std::vector<IfcUtil::IfcBaseEntity*> ps = $self->select((IfcUtil::IfcBaseEntity*)e, completely_within, extend);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select(const gp_Pnt& p, double extend=0.0) const {
		std::vector<IfcUtil::IfcBaseEntity*> ps = $self->select(p, extend);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select(const std::string& shape_serialization, bool completely_within = false, double extend = -1.e-5) const {
		std::stringstream stream(shape_serialization);
		BRepTools_ShapeSet shapes;
		shapes.Read(stream);
		const TopoDS_Shape& shp = shapes.Shape(shapes.NbShapes());

		std::vector<IfcUtil::IfcBaseEntity*> ps = $self->select(shp, completely_within, extend);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select(const IfcGeom::BRepElement* elem, bool completely_within = false, double extend = -1.e-5) const {
		std::vector<IfcUtil::IfcBaseEntity*> ps = $self->select(elem, completely_within, extend);
		return IfcGeom_tree_vector_to_list(ps);
	}

}

// A visitor

%newobject construct_iterator_with_include_exclude;
%newobject construct_iterator_with_include_exclude_globalid;
%newobject construct_iterator_with_include_exclude_id;

// I couldn't get the vector<string> typemap to be applied when %extending Iterator constructor.
// anyway it does not matter as SWIG generates C code without actual constructors
%inline %{
	IfcGeom::Iterator* construct_iterator_with_include_exclude(IfcGeom::IteratorSettings settings, IfcParse::IfcFile* file, std::vector<std::string> elems, bool include, int num_threads) {
		std::set<std::string> elems_set(elems.begin(), elems.end());
		IfcGeom::entity_filter ef{ include, false, elems_set };
		return new IfcGeom::Iterator(settings, file, {ef}, num_threads);
	}

	IfcGeom::Iterator* construct_iterator_with_include_exclude_globalid(IfcGeom::IteratorSettings settings, IfcParse::IfcFile* file, std::vector<std::string> elems, bool include, int num_threads) {
		std::set<std::string> elems_set(elems.begin(), elems.end());
		IfcGeom::attribute_filter af;
		af.attribute_name = "GlobalId";
		af.populate(elems_set);
		af.include = include;
		return new IfcGeom::Iterator(settings, file, {af}, num_threads);
	}

	IfcGeom::Iterator* construct_iterator_with_include_exclude_id(IfcGeom::IteratorSettings settings, IfcParse::IfcFile* file, std::vector<int> elems, bool include, int num_threads) {
		std::set<int> elems_set(elems.begin(), elems.end());
		IfcGeom::instance_id_filter af(include, false, elems_set);
		return new IfcGeom::Iterator(settings, file, {af}, num_threads);
	}
%}

/*%extend IfcGeom::TriangulationElement {
	%pythoncode %{
        # Hide the getters with read-only property implementations
        geometry = property(geometry)
	%}
};
*/
/*
%extend IfcGeom::SerializedElement {
	%pythoncode %{
        # Hide the getters with read-only property implementations
        geometry = property(geometry)
	%}
};
*/
%extend IfcGeom::BRepElement {
    double calc_volume_() const {
        double v;
        if ($self->geometry().calculate_volume(v)) {
            return v;
        } else {
            return std::numeric_limits<double>::quiet_NaN();
        }
    }

    double calc_surface_area_() const {
        double v;
        if ($self->geometry().calculate_surface_area(v)) {
            return v;
        } else {
            return std::numeric_limits<double>::quiet_NaN();
        }
    }
/*
    %pythoncode %{
        # Hide the getters with read-only property implementations
        geometry = property(geometry)
        volume = property(calc_volume_)
        surface_area = property(calc_surface_area_)
    %}    
	*/
};
/*
%extend IfcGeom::Material {
	%pythoncode %{
        # Hide the getters with read-only property implementations
        has_diffuse = property(hasDiffuse)
        has_specular = property(hasSpecular)
        has_transparency = property(hasTransparency)
        has_specularity = property(hasSpecularity)
        diffuse = property(diffuse)
        specular = property(specular)
        transparency = property(transparency)
        specularity = property(specularity)
        name = property(name)
	%}
};

%extend IfcGeom::Transformation {
	%pythoncode %{
        # Hide the getters with read-only property implementations
        matrix = property(matrix)
	%}
};

%extend IfcGeom::Matrix {
	%pythoncode %{
        # Hide the getters with read-only property implementations
        data = property(data)
	%}
};
*/
%{
	template <typename T>
	std::string to_locale_invariant_string(const T& t) {
		std::ostringstream oss;
		oss.imbue(std::locale::classic());
		oss << t;
		return oss.str();
	}

	template <typename Schema>
	static boost::variant<IfcGeom::Element*, IfcGeom::Representation::Representation*> helper_fn_create_shape(IfcGeom::IteratorSettings& settings, IfcUtil::IfcBaseClass* instance, IfcUtil::IfcBaseClass* representation = 0) {
		IfcParse::IfcFile* file = instance->data().file;
			
		IfcGeom::Kernel kernel(file);

		// @todo unify this logic with the logic in iterator impl.

		kernel.setValue(IfcGeom::Kernel::GV_MAX_FACES_TO_ORIENT, settings.get(IfcGeom::IteratorSettings::SEW_SHELLS) ? std::numeric_limits<double>::infinity() : -1);
		kernel.setValue(IfcGeom::Kernel::GV_DIMENSIONALITY, (settings.get(IfcGeom::IteratorSettings::INCLUDE_CURVES) ? (settings.get(IfcGeom::IteratorSettings::EXCLUDE_SOLIDS_AND_SURFACES) ? -1. : 0.) : +1.));
		kernel.setValue(IfcGeom::Kernel::GV_LAYERSET_FIRST,
			settings.get(IfcGeom::IteratorSettings::LAYERSET_FIRST)
			? +1.0
			: -1.0
		);
		kernel.setValue(IfcGeom::Kernel::GV_NO_WIRE_INTERSECTION_CHECK,
			settings.get(IfcGeom::IteratorSettings::NO_WIRE_INTERSECTION_CHECK)
			? +1.0
			: -1.0
		);
		kernel.setValue(IfcGeom::Kernel::GV_NO_WIRE_INTERSECTION_TOLERANCE,
			settings.get(IfcGeom::IteratorSettings::NO_WIRE_INTERSECTION_TOLERANCE)
			? +1.0
			: -1.0
		);
		kernel.setValue(IfcGeom::Kernel::GV_PRECISION_FACTOR,
			settings.get(IfcGeom::IteratorSettings::STRICT_TOLERANCE)
			? 1.0
			: 10.0
		);

		kernel.setValue(IfcGeom::Kernel::GV_DISABLE_BOOLEAN_RESULT,
			settings.get(IfcGeom::IteratorSettings::DISABLE_BOOLEAN_RESULT)
			? +1.0
			: -1.0
		);

		kernel.setValue(IfcGeom::Kernel::GV_DEBUG_BOOLEAN,
			settings.get(IfcGeom::IteratorSettings::DEBUG_BOOLEAN)
			? +1.0
			: -1.0
		);

		kernel.setValue(IfcGeom::Kernel::GV_BOOLEAN_ATTEMPT_2D,
			settings.get(IfcGeom::IteratorSettings::BOOLEAN_ATTEMPT_2D)
			? +1.0
			: -1.0
		);
			
		if (instance->declaration().is(Schema::IfcProduct::Class())) {
			if (representation) {
				if (!representation->declaration().is(Schema::IfcRepresentation::Class())) {
					throw IfcParse::IfcException("Supplied representation not of type IfcRepresentation");
				}
			}
		
			typename Schema::IfcProduct* product = (typename Schema::IfcProduct*) instance;

			if (!representation && !product->Representation()) {
				throw IfcParse::IfcException("Representation is NULL");
			}
			
			typename Schema::IfcProductRepresentation* prodrep = product->Representation();
			typename Schema::IfcRepresentation::list::ptr reps = prodrep->Representations();
			typename Schema::IfcRepresentation* ifc_representation = (typename Schema::IfcRepresentation*) representation;
			
			if (!ifc_representation) {
				// First, try to find a representation based on the settings
				for (typename Schema::IfcRepresentation::list::it it = reps->begin(); it != reps->end(); ++it) {
					typename Schema::IfcRepresentation* rep = *it;
					if (!rep->RepresentationIdentifier()) {
						continue;
					}
					if (!settings.get(IfcGeom::IteratorSettings::EXCLUDE_SOLIDS_AND_SURFACES)) {
						if (*rep->RepresentationIdentifier() == "Body") {
							ifc_representation = rep;
							break;
						}
					}
					if (settings.get(IfcGeom::IteratorSettings::INCLUDE_CURVES)) {
						if (*rep->RepresentationIdentifier() == "Plan" || *rep->RepresentationIdentifier() == "Axis") {
							ifc_representation = rep;
							break;
						}
					}
				}
			}

			// Otherwise, find a representation within the 'Model' or 'Plan' context
			if (!ifc_representation) {
				for (typename Schema::IfcRepresentation::list::it it = reps->begin(); it != reps->end(); ++it) {
					typename Schema::IfcRepresentation* rep = *it;
					typename Schema::IfcRepresentationContext* context = rep->ContextOfItems();
					
					// TODO: Remove redundancy with IfcGeomIterator.h
					if (context->ContextType()) {
						std::set<std::string> context_types;
						if (!settings.get(IfcGeom::IteratorSettings::EXCLUDE_SOLIDS_AND_SURFACES)) {
							context_types.insert("model");
							context_types.insert("design");
							context_types.insert("model view");
							context_types.insert("detail view");
						}
						if (settings.get(IfcGeom::IteratorSettings::INCLUDE_CURVES)) {
							context_types.insert("plan");
						}			

						std::string context_type_lc = *context->ContextType();
						for (std::string::iterator c = context_type_lc.begin(); c != context_type_lc.end(); ++c) {
							*c = tolower(*c);
						}
						if (context_types.find(context_type_lc) != context_types.end()) {
							ifc_representation = rep;
						}
					}
				}
			}

			if (!ifc_representation) {
				if (reps->size()) {
					// Return a random representation
					ifc_representation = *reps->begin();
				} else {
					throw IfcParse::IfcException("No suitable IfcRepresentation found");
				}
			}

			// Read precision for found representation's context
			auto context = ifc_representation->ContextOfItems();
			if (context->template as<typename Schema::IfcGeometricRepresentationSubContext>()) {
				context = context->template as<typename Schema::IfcGeometricRepresentationSubContext>()->ParentContext();
			}
			if (context->template as<typename Schema::IfcGeometricRepresentationContext>() && context->template as<typename Schema::IfcGeometricRepresentationContext>()->Precision()) {
				double p = *context->template as<typename Schema::IfcGeometricRepresentationContext>()->Precision()
					* kernel.getValue(IfcGeom::Kernel::GV_PRECISION_FACTOR);
				p *= kernel.getValue(IfcGeom::Kernel::GV_LENGTH_UNIT);
				if (p < 1.e-7) {
					Logger::Message(Logger::LOG_WARNING, "Precision lower than 0.0000001 meter not enforced");
					p = 1.e-7;
				}
				kernel.setValue(IfcGeom::Kernel::GV_PRECISION, p);
			}

			IfcGeom::BRepElement* brep = kernel.convert(settings, ifc_representation, product);
			if (!brep) {
				throw IfcParse::IfcException("Failed to process shape");
			}
			if (settings.get(IfcGeom::IteratorSettings::USE_BREP_DATA)) {
				IfcGeom::SerializedElement* serialization = new IfcGeom::SerializedElement(*brep);
				delete brep;
				return serialization;
			} else if (!settings.get(IfcGeom::IteratorSettings::DISABLE_TRIANGULATION)) {
				IfcGeom::TriangulationElement* triangulation = new IfcGeom::TriangulationElement(*brep);
				delete brep;
				return triangulation;
			} else {
				throw IfcParse::IfcException("No element to return based on provided settings");
			}
		} else {
			if (!representation) {
				if (instance->declaration().is(Schema::IfcRepresentationItem::Class()) || 
					instance->declaration().is(Schema::IfcRepresentation::Class()) ||
					// https://github.com/IfcOpenShell/IfcOpenShell/issues/1649
					instance->declaration().is(Schema::IfcProfileDef::Class())
				) {
					IfcGeom::IfcRepresentationShapeItems shapes = kernel.convert(instance);

					IfcGeom::ElementSettings element_settings(settings, kernel.getValue(IfcGeom::Kernel::GV_LENGTH_UNIT), instance->declaration().name());
					IfcGeom::Representation::BRep brep(element_settings, to_locale_invariant_string(instance->data().id()), shapes);
					try {
						if (settings.get(IfcGeom::IteratorSettings::USE_BREP_DATA)) {
							return new IfcGeom::Representation::Serialization(brep);
						} else if (!settings.get(IfcGeom::IteratorSettings::DISABLE_TRIANGULATION)) {
							return new IfcGeom::Representation::Triangulation(brep);
						}
					} catch (...) {
						throw IfcParse::IfcException("Error during shape serialization");
					}
				}
			} else {
				throw IfcParse::IfcException("Invalid additional representation specified");
			}
		}
		return boost::variant<IfcGeom::Element*, IfcGeom::Representation::Representation*>();
	}
%}

%inline %{
	static boost::variant<IfcGeom::Element*, IfcGeom::Representation::Representation*> create_shape(IfcGeom::IteratorSettings& settings, IfcUtil::IfcBaseClass* instance, IfcUtil::IfcBaseClass* representation = 0) {
		const std::string& schema_name = instance->declaration().schema()->name();

		#ifdef HAS_SCHEMA_2x3
		if (schema_name == "IFC2X3") {
			return helper_fn_create_shape<Ifc2x3>(settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4
		if (schema_name == "IFC4") {
			return helper_fn_create_shape<Ifc4>(settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x1
		if (schema_name == "IFC4X1") {
			return helper_fn_create_shape<Ifc4x1>(settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x2
		if (schema_name == "IFC4X2") {
			return helper_fn_create_shape<Ifc4x2>(settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3_rc1
		if (schema_name == "IFC4X3_RC1") {
			return helper_fn_create_shape<Ifc4x3_rc1>(settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3_rc2
		if (schema_name == "IFC4X3_RC2") {
			return helper_fn_create_shape<Ifc4x3_rc2>(settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3_rc3
		if (schema_name == "IFC4X3_RC3") {
			return helper_fn_create_shape<Ifc4x3_rc3>(settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3_rc4
		if (schema_name == "IFC4X3_RC4") {
			return helper_fn_create_shape<Ifc4x3_rc4>(settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3
		if (schema_name == "IFC4X3") {
			return helper_fn_create_shape<Ifc4x3>(settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3_tc1
		if (schema_name == "IFC4X3_TC1") {
			return helper_fn_create_shape<Ifc4x3_tc1>(settings, instance, representation);
		}
		#endif
        #ifdef HAS_SCHEMA_4x3_add1
		if (schema_name == "IFC4X3_ADD1") {
			return helper_fn_create_shape<Ifc4x3_add1>(settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3_add2
		if (schema_name == "IFC4X3_ADD2") {
			return helper_fn_create_shape<Ifc4x3_add2>(settings, instance, representation);
		}
		#endif

		throw IfcParse::IfcException("No geometry support for " + schema_name);
	}
%}

%inline %{
	IfcUtil::IfcBaseClass* serialise(const std::string& schema_name, const std::string& shape_str, bool advanced=true) {
		std::stringstream stream(shape_str);
		BRepTools_ShapeSet shapes;
		shapes.Read(stream);
		const TopoDS_Shape& shp = shapes.Shape(shapes.NbShapes());

		return IfcGeom::serialise(schema_name, shp, advanced);
	}

	IfcUtil::IfcBaseClass* tesselate(const std::string& schema_name, const std::string& shape_str, double d) {
		std::stringstream stream(shape_str);
		BRepTools_ShapeSet shapes;
		shapes.Read(stream);
		const TopoDS_Shape& shp = shapes.Shape(shapes.NbShapes());

		return IfcGeom::tesselate(schema_name, shp, d);
	}
%}

%ignore hlr_writer;
%ignore hlr_calc;
%ignore occt_join;
%ignore prefiltered_hlr;
%ignore svgfill::svg_to_line_segments;
%ignore svgfill::line_segments_to_polygons;

%template(svg_line_segments) std::vector<std::vector<svgfill::point_2>>;
%template(svg_groups_of_line_segments) std::vector<std::vector<std::vector<svgfill::point_2>>>;
%template(svg_point) std::array<double, 2>;
%template(line_segment) std::vector<svgfill::point_2>;
%template(svg_polygons) std::vector<svgfill::polygon_2>;
%template(svg_groups_of_polygons) std::vector<std::vector<svgfill::polygon_2>>;
%template(svg_loop) std::vector<std::array<double, 2>>;
%template(svg_loops) std::vector<std::vector<std::array<double, 2>>>;

%naturalvar svgfill::polygon_2::boundary;
%naturalvar svgfill::polygon_2::inner_boundaries;
%naturalvar svgfill::polygon_2::point_inside;

%include "../svgfill/src/svgfill.h"

%inline %{
	std::vector<std::vector<svgfill::line_segment_2>> svg_to_line_segments(const std::string& data, const boost::optional<std::string>& class_name) {
		std::vector<std::vector<svgfill::line_segment_2>> r;
		if (svgfill::svg_to_line_segments(data, class_name, r)) {
			return r;
		} else {
			throw std::runtime_error("Failed to read SVG");
		}
	}

	std::vector<std::vector<svgfill::polygon_2>> line_segments_to_polygons(svgfill::solver s, double eps, const std::vector<std::vector<svgfill::line_segment_2>>& segments) {
		std::vector<std::vector<svgfill::polygon_2>> r;
		if (svgfill::line_segments_to_polygons(s, eps, segments, r)) {
			return r;
		} else {
			throw std::runtime_error("Failed to read SVG");
		}
	}
%}
