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
%ignore boost::hash_value;

// extend the matrix4 type with getter properties for each column & useful static methods for creating & multiplying the matrices
%extend ifcopenshell::geometry::taxonomy::matrix4 {

    %newobject Col0;
    %newobject Col1;
    %newobject Col2;
    %newobject Col3;

    const std::array<double,4> Col0;
    const std::array<double,4> Col1;
    const std::array<double,4> Col2;
    const std::array<double,4> Col3;

	%newobject New;
	static ifcopenshell::geometry::taxonomy::matrix4* New(std::array<double,4> Col0, std::array<double,4> Col1, std::array<double,4> Col2,std::array<double,4> Col3)
	{
		//auto o = Eigen::Vector3d& o, const Eigen::Vector3d& z, const Eigen::Vector3d&

		auto mat = Eigen::Matrix4d();
		mat <<
			Col0[0], Col1[0], Col2[0], Col3[0],
			Col0[1], Col1[1], Col2[1], Col3[1],
			Col0[2], Col1[2], Col2[2], Col3[2],
			Col0[3], Col1[3], Col2[3], Col3[3];

		return new ifcopenshell::geometry::taxonomy::matrix4(mat);
	}

	%newobject Identity;
	static ifcopenshell::geometry::taxonomy::matrix4* Identity()
	{
		auto mat = Eigen::Matrix4d();
		mat <<
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1;

		return new ifcopenshell::geometry::taxonomy::matrix4(mat);
	}

	%newobject FromOriginAndAxes;
	static ifcopenshell::geometry::taxonomy::matrix4* FromOriginAndAxes(std::array<double,3> origin, std::array<double,3> z, std::array<double,3> x)
	{
		auto o_ = Eigen::Vector3d(origin[0], origin[1], origin[2]);
		auto z_ = Eigen::Vector3d(z[0], z[1], z[2]);
		auto x_ = Eigen::Vector3d(x[0], x[1], x[2]);
		return new ifcopenshell::geometry::taxonomy::matrix4(o_,z_,x_);
	}

	%newobject Multiply;
	static ifcopenshell::geometry::taxonomy::matrix4* Multiply(const ifcopenshell::geometry::taxonomy::matrix4*a, const ifcopenshell::geometry::taxonomy::matrix4* b)
	{
		auto mat = a->ccomponents() * b->ccomponents();
		return new ifcopenshell::geometry::taxonomy::matrix4(mat);
	}

	%ignore print;

}

// SWIG does not accept the implementations to be defined inline, so here is a code block containing them, which is included in the 
// generated cpp adapter file.
%{
    std::array<double,4>* ifcopenshell_geometry_taxonomy_matrix4_Col0_get(const ifcopenshell::geometry::taxonomy::matrix4* mat) {
        auto col = mat->ccomponents().col(0);

        return new  std::array<double,4> { col.x(), col.y(), col.z(), col.w() };
    }

    std::array<double,4>* ifcopenshell_geometry_taxonomy_matrix4_Col1_get(const ifcopenshell::geometry::taxonomy::matrix4* mat) {
        auto col = mat->ccomponents().col(1);

        return new std::array<double,4> { col.x(), col.y(), col.z(), col.w() };
    }

    std::array<double,4>* ifcopenshell_geometry_taxonomy_matrix4_Col2_get(const ifcopenshell::geometry::taxonomy::matrix4* mat) {
        auto col = mat->ccomponents().col(2);

        return new std::array<double,4> { col.x(), col.y(), col.z(), col.w() };
    }

    std::array<double,4>* ifcopenshell_geometry_taxonomy_matrix4_Col3_get(const ifcopenshell::geometry::taxonomy::matrix4* mat) {
        auto col = mat->ccomponents().col(3);

        return new std::array<double,4> { col.x(), col.y(), col.z(), col.w() };
    }
%}

// ignore this type for wrapper generation - same as in python
%ignore IfcGeom::impl::tree::selector;

// extend the return value of the iterator->get() method with functions to get the element as a more concrete type.
%extend IfcGeom::Element {
	IfcGeom::SerializedElement* TryGetAsSerializedElement();

	// This call should not fail, unless USE_BREP_DATA & DISABLE_TRIANGULATION are specified in the iterator settings.
	IfcGeom::TriangulationElement* TryGetAsTriangulationElement();

	IfcGeom::BRepElement* TryGetAsBRepElement();
}

// SWIG does not like an inline implementation, so here is a code block containing the implementation of the above methods.
// It is included into the cpp layer generated by swig.
%inline %{
	IfcGeom::SerializedElement* IfcGeom_Element_TryGetAsSerializedElement(IfcGeom::Element* elem)
	{
		return dynamic_cast<IfcGeom::SerializedElement*>(elem);
	}

	IfcGeom::TriangulationElement* IfcGeom_Element_TryGetAsTriangulationElement(IfcGeom::Element* elem)
	{
		return dynamic_cast<IfcGeom::TriangulationElement*>(elem);
	}

	IfcGeom::BRepElement* IfcGeom_Element_TryGetAsBRepElement(IfcGeom::Element* elem)
	{
		return dynamic_cast<IfcGeom::BRepElement*>(elem);
	}

%}

// Specify that a bunch of Cpp methods have return values allocated using the new operator.
// This line transfers ownership of them to the C# wrapper object,
// allowing the native resources to be freed once the wrapper object goes out of scope & is garbage collected.
%newobject IfcGeom::Representation::BRep::item;

%newobject IfcGeom::ConversionResultShape::halfspaces;
%newobject IfcGeom::ConversionResultShape::box;
%newobject IfcGeom::ConversionResultShape::solid;
%newobject IfcGeom::ConversionResultShape::add;
%newobject IfcGeom::ConversionResultShape::subtract;
%newobject IfcGeom::ConversionResultShape::intersect;
%newobject IfcGeom::ConversionResultShape::moved;

%newobject IfcGeom::ConversionResultShape::area;
%newobject IfcGeom::ConversionResultShape::volume;
%newobject IfcGeom::ConversionResultShape::length;

%newobject nary_union;

%newobject IfcGeom::OpaqueNumber::operator+;
%newobject IfcGeom::OpaqueNumber::operator-;
%newobject IfcGeom::OpaqueNumber::operator*;
%newobject IfcGeom::OpaqueNumber::operator/;

// copied from the python implementation - not sure if we need this
%inline %{
std::string taxonomy_item_repr(ifcopenshell::geometry::taxonomy::item::ptr i) {
	std::ostringstream oss;
	i->print(oss);
	return oss.str();
}
%}

// apply shared_ptr wrapping to a bunch of types - same as in the python version
%shared_ptr(ifcopenshell::geometry::taxonomy::item);
%shared_ptr(ifcopenshell::geometry::taxonomy::implicit_item);
%shared_ptr(ifcopenshell::geometry::taxonomy::piecewise_function);
%shared_ptr(ifcopenshell::geometry::taxonomy::less_functor);
%shared_ptr(ifcopenshell::geometry::taxonomy::eigen_base);
%shared_ptr(ifcopenshell::geometry::taxonomy::matrix4);
%shared_ptr(ifcopenshell::geometry::taxonomy::colour);
%shared_ptr(ifcopenshell::geometry::taxonomy::style);
%shared_ptr(ifcopenshell::geometry::taxonomy::geom_item);
%shared_ptr(ifcopenshell::geometry::taxonomy::cartesian_base);
%shared_ptr(ifcopenshell::geometry::taxonomy::point3);
%shared_ptr(ifcopenshell::geometry::taxonomy::direction3);
%shared_ptr(ifcopenshell::geometry::taxonomy::curve);
%shared_ptr(ifcopenshell::geometry::taxonomy::line);
%shared_ptr(ifcopenshell::geometry::taxonomy::circle);
%shared_ptr(ifcopenshell::geometry::taxonomy::ellipse);
%shared_ptr(ifcopenshell::geometry::taxonomy::bspline_curve);
%shared_ptr(ifcopenshell::geometry::taxonomy::offset_curve);
%shared_ptr(ifcopenshell::geometry::taxonomy::trimmed_curve);
%shared_ptr(ifcopenshell::geometry::taxonomy::edge);
%shared_ptr(ifcopenshell::geometry::taxonomy::collection_base);
%shared_ptr(ifcopenshell::geometry::taxonomy::collection);
%shared_ptr(ifcopenshell::geometry::taxonomy::loop);
%shared_ptr(ifcopenshell::geometry::taxonomy::face);
%shared_ptr(ifcopenshell::geometry::taxonomy::shell);
%shared_ptr(ifcopenshell::geometry::taxonomy::solid);
%shared_ptr(ifcopenshell::geometry::taxonomy::loft);
%shared_ptr(ifcopenshell::geometry::taxonomy::surface);
%shared_ptr(ifcopenshell::geometry::taxonomy::plane);
%shared_ptr(ifcopenshell::geometry::taxonomy::cylinder);
%shared_ptr(ifcopenshell::geometry::taxonomy::sphere);
%shared_ptr(ifcopenshell::geometry::taxonomy::torus);
%shared_ptr(ifcopenshell::geometry::taxonomy::bspline_surface);
%shared_ptr(ifcopenshell::geometry::taxonomy::sweep);
%shared_ptr(ifcopenshell::geometry::taxonomy::extrusion);
%shared_ptr(ifcopenshell::geometry::taxonomy::revolve);
%shared_ptr(ifcopenshell::geometry::taxonomy::sweep_along_curve);
%shared_ptr(ifcopenshell::geometry::taxonomy::node);

// include a bunch of relevant files for generating wrappers for.
%include "../ifcgeom/ifc_geom_api.h"
%include "../ifcgeom/Converter.h"
%include "../ifcgeom/ConversionResult.h"
%include "../ifcgeom/IteratorSettings.h"
%include "../ifcgeom/ConversionSettings.h"
%include "../ifcgeom/IfcGeomElement.h"
%include "../ifcgeom/IfcGeomRepresentation.h"
%include "../ifcgeom/Iterator.h"
%include "../ifcgeom/GeometrySerializer.h"

%include "../ifcgeom/taxonomy.h"

%include "../serializers/SvgSerializer.h"
%include "../serializers/HdfSerializer.h"
%include "../serializers/WavefrontObjSerializer.h"
%include "../serializers/XmlSerializer.h"
%include "../serializers/GltfSerializer.h"

%extend ifcopenshell::geometry::taxonomy::style {
	size_t instance_id() const {
		if (self->instance == nullptr) {
			return 0;
		}
		const IfcUtil::IfcBaseEntity* ent;
		if ((ent = self->instance->as<IfcUtil::IfcBaseEntity>()) == nullptr) {
			return 0;
		}
		return ent->data().id();
	}
}

%extend ifcopenshell::geometry::taxonomy::loop {
	const std::vector<ifcopenshell::geometry::taxonomy::edge::ptr>& Children() const {
		return $self->children;
	}
}

// Extend the settings object with methods for setting&getting values.
// Needs explicit implementations because the internal one is templated.
%extend ifcopenshell::geometry::Settings {
	void Set(const std::string& name, bool val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, int val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, ifcopenshell::geometry::settings::IteratorOutputOptions val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, ifcopenshell::geometry::settings::PiecewiseStepMethod val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, ifcopenshell::geometry::settings::OutputDimensionalityTypes val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, double val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, const std::string& val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, const std::set<int>& val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, const std::set<std::string>& val) {
		return $self->set(name, val);
	}
	ifcopenshell::geometry::Settings::value_variant_t Get(const std::string& name) {
		return $self->get(name);
	}
	std::vector<std::string> SettingNames() {
		return $self->setting_names();
	}
}

// Extend the settings object with methods for setting&getting values.
// Needs explicit implementations because the internal one is templated.
%extend ifcopenshell::geometry::SerializerSettings {
	void Set(const std::string& name, bool val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, int val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, double val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, const std::string& val) {
		return $self->set(name, val);
	}
	void Set(const std::string& name, const std::set<int>& val) {
		return $self->set(name, val);
	}
	ifcopenshell::geometry::SerializerSettings::value_variant_t Get(const std::string& name) {
		return $self->get(name);
	}
	std::vector<std::string> SettingNames() {
		return $self->setting_names();
	}
}

#ifdef IFOPSH_WITH_OPENCASCADE

%template(ray_intersection_results) std::vector<IfcGeom::ray_intersection_result>;

%template(clashes) std::vector<IfcGeom::clash>;

// A Template instantantation should be defined before it is used as a base class. 
// But frankly I don't care as most methods are subtlely different anyway.
%include "../ifcgeom/kernels/opencascade/IfcGeomTree.h"

%extend IfcGeom::tree {

	static aggregate_of_instance::ptr vector_to_list(const std::vector<const IfcUtil::IfcBaseEntity*>& ps) {
		aggregate_of_instance::ptr r(new aggregate_of_instance);
		for (auto it = ps.begin(); it != ps.end(); ++it) {
			// @todo
			r->push(const_cast<IfcUtil::IfcBaseEntity*>(*it));
		}
		return r;
	}

	aggregate_of_instance::ptr select_box(IfcUtil::IfcBaseClass* e, bool completely_within = false, double extend=-1.e-5) const {
		if (!e->declaration().is("IfcProduct")) {
			throw IfcParse::IfcException("Instance should be an IfcProduct");
		}
		std::vector<const IfcUtil::IfcBaseEntity*> ps = $self->select_box((IfcUtil::IfcBaseEntity*)e, completely_within, extend);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select_box(const gp_Pnt& p) const {
		std::vector<const IfcUtil::IfcBaseEntity*> ps = $self->select_box(p);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select_box(const Bnd_Box& b, bool completely_within = false) const {
		std::vector<const IfcUtil::IfcBaseEntity*> ps = $self->select_box(b, completely_within);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select(IfcUtil::IfcBaseClass* e, bool completely_within = false, double extend = 0.0) const {
		if (!e->declaration().is("IfcProduct")) {
			throw IfcParse::IfcException("Instance should be an IfcProduct");
		}
		std::vector<const IfcUtil::IfcBaseEntity*> ps = $self->select((IfcUtil::IfcBaseEntity*)e, completely_within, extend);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select(const gp_Pnt& p, double extend=0.0) const {
		std::vector<const IfcUtil::IfcBaseEntity*> ps = $self->select(p, extend);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select(const std::string& shape_serialization, bool completely_within = false, double extend = -1.e-5) const {
		std::stringstream stream(shape_serialization);
		BRepTools_ShapeSet shapes;
		shapes.Read(stream);
		const TopoDS_Shape& shp = shapes.Shape(shapes.NbShapes());

		std::vector<const IfcUtil::IfcBaseEntity*> ps = $self->select(shp, completely_within, extend);
		return IfcGeom_tree_vector_to_list(ps);
	}

	aggregate_of_instance::ptr select(const IfcGeom::BRepElement* elem, bool completely_within = false, double extend = -1.e-5) const {
		std::vector<const IfcUtil::IfcBaseEntity*> ps = $self->select(elem, completely_within, extend);
		return IfcGeom_tree_vector_to_list(ps);
	}




	std::vector<clash> clash_intersection_many(const std::vector<IfcUtil::IfcBaseClass*>& set_a, const std::vector<IfcUtil::IfcBaseClass*>& set_b, double tolerance, bool check_all) const {
        std::vector<const IfcUtil::IfcBaseEntity*> set_a_entities;
        std::vector<const IfcUtil::IfcBaseEntity*> set_b_entities;
        for (auto* e : set_a) {
            if (!e->declaration().is("IfcProduct")) {
                throw IfcParse::IfcException("All instances should be of type IfcProduct");
            }
            set_a_entities.push_back(static_cast<IfcUtil::IfcBaseEntity*>(e));
        }
        for (auto* e : set_b) {
            if (!e->declaration().is("IfcProduct")) {
                throw IfcParse::IfcException("All instances should be of type IfcProduct");
            }
            set_b_entities.push_back(static_cast<IfcUtil::IfcBaseEntity*>(e));
        }
               return $self->clash_intersection_many(set_a_entities, set_b_entities, tolerance, check_all);
       }

       std::vector<clash> clash_collision_many(const std::vector<IfcUtil::IfcBaseClass*>& set_a, const std::vector<IfcUtil::IfcBaseClass*>& set_b, bool allow_touching) const {
        std::vector<const IfcUtil::IfcBaseEntity*> set_a_entities;
        std::vector<const IfcUtil::IfcBaseEntity*> set_b_entities;
        for (auto* e : set_a) {
            if (!e->declaration().is("IfcProduct")) {
                throw IfcParse::IfcException("All instances should be of type IfcProduct");
            }
            set_a_entities.push_back(static_cast<IfcUtil::IfcBaseEntity*>(e));
        }
        for (auto* e : set_b) {
            if (!e->declaration().is("IfcProduct")) {
                throw IfcParse::IfcException("All instances should be of type IfcProduct");
            }
            set_b_entities.push_back(static_cast<IfcUtil::IfcBaseEntity*>(e));
        }
               return $self->clash_collision_many(set_a_entities, set_b_entities, allow_touching);
       }

       std::vector<clash> clash_clearance_many(const std::vector<IfcUtil::IfcBaseClass*>& set_a, const std::vector<IfcUtil::IfcBaseClass*>& set_b, double clearance, bool check_all) const {
        std::vector<const IfcUtil::IfcBaseEntity*> set_a_entities;
        std::vector<const IfcUtil::IfcBaseEntity*> set_b_entities;
        for (auto* e : set_a) {
            if (!e->declaration().is("IfcProduct")) {
                throw IfcParse::IfcException("All instances should be of type IfcProduct");
            }
            set_a_entities.push_back(static_cast<IfcUtil::IfcBaseEntity*>(e));
        }
        for (auto* e : set_b) {
            if (!e->declaration().is("IfcProduct")) {
                throw IfcParse::IfcException("All instances should be of type IfcProduct");
            }
            set_b_entities.push_back(static_cast<IfcUtil::IfcBaseEntity*>(e));
        }
               return $self->clash_clearance_many(set_a_entities, set_b_entities, clearance, check_all);
	}
}

#endif

%newobject construct_iterator_with_include_exclude;
%newobject construct_iterator_with_include_exclude_globalid;
%newobject construct_iterator_with_include_exclude_id;

// I couldn't get the vector<string> typemap to be applied when %extending Iterator constructor.
// anyway it does not matter as SWIG generates C code without actual constructors
%inline %{
	IfcGeom::Iterator* construct_iterator_with_include_exclude(const std::string& geometry_library, ifcopenshell::geometry::Settings settings, IfcParse::IfcFile* file, std::vector<std::string> elems, bool include, int num_threads) {
		std::set<std::string> elems_set(elems.begin(), elems.end());
		IfcGeom::entity_filter ef{ include, false, elems_set };
		return new IfcGeom::Iterator(geometry_library, settings, file, {ef}, num_threads);
	}

	IfcGeom::Iterator* construct_iterator_with_include_exclude_globalid(const std::string& geometry_library, ifcopenshell::geometry::Settings settings, IfcParse::IfcFile* file, std::vector<std::string> elems, bool include, int num_threads) {
		std::set<std::string> elems_set(elems.begin(), elems.end());
		IfcGeom::attribute_filter af;
		af.attribute_name = "GlobalId";
		af.populate(elems_set);
		af.include = include;
		return new IfcGeom::Iterator(geometry_library, settings, file, {af}, num_threads);
	}

	IfcGeom::Iterator* construct_iterator_with_include_exclude_id(const std::string& geometry_library, ifcopenshell::geometry::Settings settings, IfcParse::IfcFile* file, std::vector<int> elems, bool include, int num_threads) {
		std::set<int> elems_set(elems.begin(), elems.end());
		IfcGeom::instance_id_filter af(include, false, elems_set);
		return new IfcGeom::Iterator(geometry_library, settings, file, {af}, num_threads);
	}
%}

%extend IfcGeom::Element {
    std::pair<const double*, size_t> transformation_buffer() const {
        // @todo check whether needs to be transposed
        const double* data = self->transformation().data()->ccomponents().data();
        return { data, 16 * sizeof(double) };
    }
};

// extension methods for BRepElement copied from the python versions - not sure what they are used for
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

};

// extension methods copied from the python versions - not sure what they are used for
%{
	template <typename T>
	std::string to_locale_invariant_string(const T& t) {
		std::ostringstream oss;
		oss.imbue(std::locale::classic());
		oss << t;
		return oss.str();
	}

	template <typename Schema>
	static boost::variant<IfcGeom::Element*, IfcGeom::Representation::Representation*, IfcGeom::Transformation*> helper_fn_create_shape(const std::string& geometry_library, ifcopenshell::geometry::Settings& settings, IfcUtil::IfcBaseClass* instance, IfcUtil::IfcBaseClass* representation = 0) {
		IfcParse::IfcFile* file = instance->data().file;
			
		ifcopenshell::geometry::Converter kernel(geometry_library, file, settings);
			
		if (typename Schema::IfcProduct* product = instance->as<typename Schema::IfcProduct>()) {
			if (representation) {
				if (!representation->declaration().is(Schema::IfcRepresentation::Class())) {
					throw IfcParse::IfcException("Supplied representation not of type IfcRepresentation");
				}
			}
		
			if (!representation && !product->Representation()) {
				throw IfcParse::IfcException("Representation is NULL");
			}
			
			typename Schema::IfcProductRepresentation* prodrep = product->Representation();
			typename Schema::IfcRepresentation::list::ptr reps = prodrep->Representations();
			typename Schema::IfcRepresentation* ifc_representation = representation ? representation->as<typename Schema::IfcRepresentation>() : nullptr;
			
			if (!ifc_representation) {
				// First, try to find a representation based on the settings
				for (typename Schema::IfcRepresentation::list::it it = reps->begin(); it != reps->end(); ++it) {
					typename Schema::IfcRepresentation* rep = *it;
					if (!rep->RepresentationIdentifier()) {
						continue;
					}
					if (settings.get<ifcopenshell::geometry::settings::OutputDimensionality>().get() != ifcopenshell::geometry::settings::CURVES) {
						if (*rep->RepresentationIdentifier() == "Body" || *rep->RepresentationIdentifier() == "Facetation") {
							ifc_representation = rep;
							break;
						}
					} else {
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
						if (settings.get<ifcopenshell::geometry::settings::OutputDimensionality>().get() != ifcopenshell::geometry::settings::CURVES) {
							context_types.insert("model");
							context_types.insert("design");
							context_types.insert("model view");
							context_types.insert("detail view");
						} else {
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

			IfcGeom::BRepElement* brep = kernel.create_brep_for_representation_and_product(ifc_representation, product);
			if (!brep) {
				throw IfcParse::IfcException("Failed to process shape");
			}
			if (settings.get<ifcopenshell::geometry::settings::IteratorOutput>().get() == ifcopenshell::geometry::settings::SERIALIZED) {
				IfcGeom::SerializedElement* serialization = new IfcGeom::SerializedElement(*brep);
				delete brep;
				return serialization;
			} else if (settings.get<ifcopenshell::geometry::settings::IteratorOutput>().get() == ifcopenshell::geometry::settings::TRIANGULATED) {
				IfcGeom::TriangulationElement* triangulation = new IfcGeom::TriangulationElement(*brep);
				delete brep;
				return triangulation;
			} else {
				throw IfcParse::IfcException("No element to return based on provided settings");
			}
		} else if (instance->as<typename Schema::IfcPlacement>() != nullptr || instance->as<typename Schema::IfcObjectPlacement>()) {
			auto item = ifcopenshell::geometry::taxonomy::cast<ifcopenshell::geometry::taxonomy::matrix4>(kernel.mapping()->map(instance));
			if (item == nullptr) {
				throw IfcParse::IfcException("Failed to convert placement");
			}
			return new IfcGeom::Transformation(settings, item);
		} else {
			if (!representation) {
				if (instance->declaration().is(Schema::IfcRepresentationItem::Class()) || 
					instance->declaration().is(Schema::IfcRepresentation::Class()) ||
					// https://github.com/IfcOpenShell/IfcOpenShell/issues/1649
					instance->declaration().is(Schema::IfcProfileDef::Class())
				) {
					IfcGeom::ConversionResults shapes;
					try {
						shapes = kernel.convert(instance);
					} catch (...) {
						throw IfcParse::IfcException("Failed to process shape");
					}

					IfcGeom::Representation::BRep brep(settings, instance->declaration().name(), to_locale_invariant_string(instance->data().id()), shapes);
					try {
						if (settings.get<ifcopenshell::geometry::settings::IteratorOutput>().get() == ifcopenshell::geometry::settings::SERIALIZED) {
							return new IfcGeom::Representation::Serialization(brep);
						} else if (settings.get<ifcopenshell::geometry::settings::IteratorOutput>().get() == ifcopenshell::geometry::settings::TRIANGULATED) {
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

// extension methods copied from the python versions - not sure what they are used for
%inline %{
	ifcopenshell::geometry::taxonomy::item::ptr map_shape(ifcopenshell::geometry::Settings& settings, IfcUtil::IfcBaseClass* instance) {
        std::unique_ptr<ifcopenshell::geometry::abstract_mapping> mapping(ifcopenshell::geometry::impl::mapping_implementations().construct(instance->data().file, settings));
		return mapping->map(instance);
	}
%}

// extension methods copied from the python versions - not sure what they are used for
%inline %{
	static boost::variant<IfcGeom::Element*, IfcGeom::Representation::Representation*, IfcGeom::Transformation*> create_shape(ifcopenshell::geometry::Settings& settings, IfcUtil::IfcBaseClass* instance, IfcUtil::IfcBaseClass* representation = 0, const char* const geometry_library="opencascade") {
		const std::string& schema_name = instance->declaration().schema()->name();

		#ifdef HAS_SCHEMA_2x3
		if (schema_name == "IFC2X3") {
			return helper_fn_create_shape<Ifc2x3>(geometry_library, settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4
		if (schema_name == "IFC4") {
			return helper_fn_create_shape<Ifc4>(geometry_library, settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x1
		if (schema_name == "IFC4X1") {
			return helper_fn_create_shape<Ifc4x1>(geometry_library, settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x2
		if (schema_name == "IFC4X2") {
			return helper_fn_create_shape<Ifc4x2>(geometry_library, settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3_rc1
		if (schema_name == "IFC4X3_RC1") {
			return helper_fn_create_shape<Ifc4x3_rc1>(geometry_library, settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3_rc2
		if (schema_name == "IFC4X3_RC2") {
			return helper_fn_create_shape<Ifc4x3_rc2>(geometry_library, settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3_rc3
		if (schema_name == "IFC4X3_RC3") {
			return helper_fn_create_shape<Ifc4x3_rc3>(geometry_library, settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3_rc4
		if (schema_name == "IFC4X3_RC4") {
			return helper_fn_create_shape<Ifc4x3_rc4>(geometry_library, settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3
		if (schema_name == "IFC4X3") {
			return helper_fn_create_shape<Ifc4x3>(geometry_library, settings, instance, representation);
		}
		#endif
		#ifdef HAS_SCHEMA_4x3_tc1
		if (schema_name == "IFC4X3_TC1") {
			return helper_fn_create_shape<Ifc4x3_tc1>(geometry_library, settings, instance, representation);
		}
		#endif
        #ifdef HAS_SCHEMA_4x3_add1
		if (schema_name == "IFC4X3_ADD1") {
			return helper_fn_create_shape<Ifc4x3_add1>(geometry_library, settings, instance, representation);
		}
		#endif
        #ifdef HAS_SCHEMA_4x3_add2
		if (schema_name == "IFC4X3_ADD2") {
			return helper_fn_create_shape<Ifc4x3_add2>(geometry_library, settings, instance, representation);
		}
		#endif

		throw IfcParse::IfcException("No geometry support for " + schema_name);
	}
%}

#ifdef IFOPSH_WITH_OPENCASCADE

// extension methods copied from the python versions - not sure what they are used for
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

#endif

#ifdef IFOPSH_WITH_CGAL

%ignore hlr_writer;
%ignore hlr_calc;
%ignore occt_join;
%ignore prefiltered_hlr;
%ignore svgfill::svg_to_line_segments;
%ignore svgfill::line_segments_to_polygons;

// NOTE WS: Commented out-code that was copied from the python implementation. For some reason these types are not successfully
// wrapped in C#. As we dont need the svg serializer i just commented this out, but if someone needs it, fix these lines!

// %template(svg_line_segments) std::vector<std::array<svgfill::point_2, 2>>;
// %template(svg_groups_of_line_segments) std::vector<std::vector<std::array<svgfill::point_2, 2>>>;
// %template(svg_point) std::array<double, 2>;
// %template(line_segment) std::array<svgfill::point_2, 2>;
// %template(svg_polygons) std::vector<svgfill::polygon_2>;
// %template(svg_groups_of_polygons) std::vector<std::vector<svgfill::polygon_2>>;
// %template(svg_loop) std::vector<std::array<double, 2>>;
// %template(svg_loops) std::vector<std::vector<std::array<double, 2>>>;

%template(OpaqueCoordinate_3) IfcGeom::OpaqueCoordinate<3>;
%template(OpaqueCoordinate_4) IfcGeom::OpaqueCoordinate<4>;

%newobject create_epeck;

%inline %{
	IfcGeom::OpaqueNumber* create_epeck(int i) {
		return new ifcopenshell::geometry::NumberEpeck(i);
	}
	IfcGeom::OpaqueNumber* create_epeck(double d) {
		return new ifcopenshell::geometry::NumberEpeck(d);
	}
	IfcGeom::OpaqueNumber* create_epeck(const std::string& s) {
		return new ifcopenshell::geometry::NumberEpeck(typename CGAL::Epeck::FT::ET(s));
	}
%}

%extend IfcGeom::ConversionResultShape {
	std::string serialize_obj() {
		std::ostringstream result;
		auto cgs = dynamic_cast<ifcopenshell::geometry::CgalShape*>($self);
		if (cgs) {
			write_to_obj(cgs->nef(), result, std::numeric_limits<size_t>::max());
		}		
		return result.str();
	}

	void convex_tag(bool b) {
		auto cgs = dynamic_cast<ifcopenshell::geometry::CgalShape*>($self);
		if (cgs) {
			cgs->convex_tag() = b;
		}		
	}

	std::string serialize() {
		std::string result;
		ifcopenshell::geometry::taxonomy::matrix4 iden;
		$self->Serialize(iden, result);
		return result;
	}
}

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

#endif
