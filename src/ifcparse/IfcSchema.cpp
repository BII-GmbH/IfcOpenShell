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

#include "IfcSchema.h"
#include "IfcStaticSchemaCache.h"
#include "IfcBaseClass.h"

#include <map>

bool IfcParse::declaration::is(const std::string& name) const {
    const std::string* name_ptr = &name;
    if (std::any_of(name.begin(), name.end(), [](char character) { return std::islower(character); })) {
        temp_string_() = name;
        boost::to_upper(temp_string_());
        name_ptr = &temp_string_();
    }

    if (name_upper_ == *name_ptr) {
        return true;
    }

    if ((this->as_entity() != nullptr) && (this->as_entity()->supertype() != nullptr)) {
        return this->as_entity()->supertype()->is(name);
    }
    if (this->as_type_declaration() != nullptr) {
        const IfcParse::named_type* named_type = this->as_type_declaration()->declared_type()->as_named_type();
        if (named_type != nullptr) {
            return named_type->is(name);
        }
    }

    return false;
}

bool IfcParse::declaration::is(const IfcParse::declaration& decl) const {
    if (this == &decl) {
        return true;
    }

    if ((this->as_entity() != nullptr) && (this->as_entity()->supertype() != nullptr)) {
        return this->as_entity()->supertype()->is(decl);
    }
    if (this->as_type_declaration() != nullptr) {
        const IfcParse::named_type* named_type = this->as_type_declaration()->declared_type()->as_named_type();
        if (named_type != nullptr) {
            return named_type->is(decl);
        }
    }

    return false;
}

bool IfcParse::named_type::is(const std::string& name) const {
    return declared_type()->is(name);
}

bool IfcParse::named_type::is(const IfcParse::declaration& decl) const {
    return declared_type()->is(decl);
}

IfcParse::entity::~entity() {
    for (const auto* attribute : attributes_) {
        delete attribute;
    }
    for (const auto* inverse_attribute : inverse_attributes_) {
        delete inverse_attribute;
    }
}

IfcParse::schema_definition::schema_definition(const std::string& name, const std::vector<const declaration*>& declarations, instance_factory* factory)
    : name_(name),
      declarations_(declarations),
      factory_(factory) {
    std::sort(declarations_.begin(), declarations_.end(), declaration_by_index_sort());
    for (std::vector<const declaration*>::iterator it = declarations_.begin(); it != declarations_.end(); ++it) {
        (**it).schema_ = this;

        if ((**it).as_type_declaration() != nullptr) {
            type_declarations_.push_back((**it).as_type_declaration());
        }
        if ((**it).as_select_type() != nullptr) {
            select_types_.push_back((**it).as_select_type());
        }
        if ((**it).as_enumeration_type() != nullptr) {
            enumeration_types_.push_back((**it).as_enumeration_type());
        }
        if ((**it).as_entity() != nullptr) {
            entities_.push_back((**it).as_entity());
        }
    }
}

IfcParse::schema_definition::~schema_definition() {
    for (std::vector<const declaration*>::const_iterator it = declarations_.begin(); it != declarations_.end(); ++it) {
        delete *it;
    }
    delete factory_;
}

IfcUtil::IfcBaseClass* IfcParse::schema_definition::instantiate(IfcEntityInstanceData* data) const {
    if (factory_ != nullptr) {
        return (*factory_)(data);
    }
    return new IfcUtil::IfcLateBoundEntity(data->type(), data);
}