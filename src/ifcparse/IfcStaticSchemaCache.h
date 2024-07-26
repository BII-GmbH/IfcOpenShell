#ifndef IFCSTATICDATA_H
#define IFCSTATICDATA_H

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

#include <shared_mutex>
#include <memory>
#include <map>

#include "ifc_parse_api.h"

namespace IfcParse {

class schema_definition;

// This class is internal to the IfcParse library. It should not be exposed via IFC_PARSE_API.
// Instead of scattering thread-unsafe static data all throughout the application code, have this ONE class
// that has a static instance protected by a mutex -
class IfcStaticSchemaCache {
public:
    static std::shared_ptr<IfcParse::schema_definition> get_or_create_schema(const std::string& name);
    static void register_schema(std::shared_ptr<schema_definition> schema, bool replaceIfLoaded=false);
    ~IfcStaticSchemaCache() = default;
private:
    IfcStaticSchemaCache() = default;
    // using this we can prevent multiple multi-threaded calls to attempt to initialize
    // the static data all at once - the mutex will only be available for the first one
    // coming in & the others can read the result once the first thread is done!
    static std::shared_mutex instance_mutex;

    // DO NOT ACCESS WITHOUT AQUIRING A LOCK from instance_mutex FIRST!
    static std::unique_ptr<IfcStaticSchemaCache> s_instance;

    // returns a shared_pointer to the existing schema - or empty if this schema is not loaded yet
    static std::shared_ptr<IfcParse::schema_definition> try_get_schema(const std::string& name);

    std::map<std::string, std::shared_ptr<IfcParse::schema_definition>> schemas;
};

}

#endif
