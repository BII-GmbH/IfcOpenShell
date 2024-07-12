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

#ifndef IFCBASEENTITY_H
#define IFCBASEENTITY_H

#include "ifc_parse_api.h"

#include "IfcBaseClass.h"
#include "aggregate_of_instance.h"

#include <memory>

#include <atomic>

class Argument;

namespace IfcUtil {

	class IFC_PARSE_API IfcBaseEntity : public IfcBaseClass {
	public:
		IfcBaseEntity() : IfcBaseClass() {}
		IfcBaseEntity(IfcEntityInstanceData* d) : IfcBaseClass(d) {}

		virtual const IfcParse::entity& declaration() const = 0;

		Argument* get(const std::string& name) const;

		std::shared_ptr<aggregate_of_instance> get_inverse(const std::string& a) const;
	};
}

#endif
