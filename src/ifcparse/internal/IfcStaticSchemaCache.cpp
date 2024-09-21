#include "IfcStaticSchemaCache.h"

#include <algorithm>
#include <iostream>
#include <thread>

#include <boost/lexical_cast.hpp>
#include <boost/algorithm/string.hpp>

#include "../IfcException.h"

#ifdef HAS_SCHEMA_2x3
#include "../Ifc2x3.h"
#endif
#ifdef HAS_SCHEMA_4
#include "../Ifc4.h"
#endif
#ifdef HAS_SCHEMA_4x1
#include "../Ifc4x1.h"
#endif
#ifdef HAS_SCHEMA_4x2
#include "../Ifc4x2.h"
#endif
#ifdef HAS_SCHEMA_4x3_rc1
#include "../Ifc4x3_rc1.h"
#endif
#ifdef HAS_SCHEMA_4x3_rc2
#include "../Ifc4x3_rc2.h"
#endif
#ifdef HAS_SCHEMA_4x3_rc3
#include "../Ifc4x3_rc3.h"
#endif
#ifdef HAS_SCHEMA_4x3_rc4
#include "../Ifc4x3_rc4.h"
#endif
#ifdef HAS_SCHEMA_4x3
#include "../Ifc4x3.h"
#endif
#ifdef HAS_SCHEMA_4x3_tc1
#include "../Ifc4x3_tc1.h"
#endif
#ifdef HAS_SCHEMA_4x3_add1
#include "../Ifc4x3_add1.h"
#endif
#ifdef HAS_SCHEMA_4x3_add2
#include "../Ifc4x3_add2.h"
#endif

std::shared_mutex IfcParse::internal::IfcStaticSchemaCache::instance_mutex;
std::unique_ptr<IfcParse::internal::IfcStaticSchemaCache> IfcParse::internal::IfcStaticSchemaCache::s_instance;

std::shared_ptr<IfcParse::schema_definition> IfcParse::internal::IfcStaticSchemaCache::try_get_schema(const std::string& name) {
    std::cout << std::this_thread::get_id() << " Waiting for read lock" << std::endl;
	std::shared_lock<std::shared_mutex> lock(instance_mutex);
	std::cout << std::this_thread::get_id() << " Read Lock aquired" << std::endl;
    // do read
    if(!s_instance) return {};

    auto it = s_instance->schemas.find(name);
    if(it != s_instance->schemas.end())
    {
        return it->second;
    }
    return {};
}


std::shared_ptr<IfcParse::schema_definition> IfcParse::internal::IfcStaticSchemaCache::get_or_create_schema(const std::string& name) {
    auto upperName = boost::to_upper_copy(name);
    // try get existing cached entry
    auto result = try_get_schema(upperName);
    if(result) {
        return result;
    }
    // we could not find the entry - create it - but we need a write lock for that

    std::lock_guard<std::shared_mutex> lock(instance_mutex);
	std::cout << std::this_thread::get_id() << " Write lock aquired" << std::endl;
    // we successfully got a write lock - now check if we need to create the entire data or only the schema entry
    if(!s_instance) {
        s_instance = std::unique_ptr<IfcStaticSchemaCache>(new IfcStaticSchemaCache());
    }
    // either the instance did not exist - in which case there is no entry for this schema yet,
    // or the instance existed, but did not have an entry for this schema - so we can just
    // create the appropriate one.
    std::shared_ptr<IfcParse::schema_definition> schema;
    #ifdef HAS_SCHEMA_2x3
        if(upperName == Ifc2x3::Identifier)
             schema = Ifc2x3::create_schema();
    #endif
    #ifdef HAS_SCHEMA_4
         if(upperName == Ifc4::Identifier)
             schema = Ifc4::create_schema();
    #endif
    #ifdef HAS_SCHEMA_4x1
        if(upperName == Ifc4x1::Identifier)
             schema = Ifc4x1::create_schema();
    #endif
    #ifdef HAS_SCHEMA_4x2
        if(upperName == Ifc4x2::Identifier)
             schema = Ifc4x2::create_schema();
    #endif
    #ifdef HAS_SCHEMA_4x3_rc1
        if(upperName == Ifc4x3_rc1::Identifier)
             schema = Ifc4x3_rc1::create_schema();
    #endif
    #ifdef HAS_SCHEMA_4x3_rc2
        if(upperName == Ifc4x3_rc2::Identifier)
             schema = Ifc4x3_rc2::create_schema();
    #endif
    #ifdef HAS_SCHEMA_4x3_rc3
        if(upperName == Ifc4x3_rc3::Identifier)
             schema = Ifc4x3_rc3::create_schema();
    #endif
    #ifdef HAS_SCHEMA_4x3_rc4
        if(upperName == Ifc4x3_rc4::Identifier)
             schema = Ifc4x3_rc4::create_schema();
    #endif
    #ifdef HAS_SCHEMA_4x3
        if(upperName == Ifc4x3::Identifier)
             schema = Ifc4x3::create_schema();
    #endif
    #ifdef HAS_SCHEMA_4x3_tc1
        if(upperName == Ifc4x3_tc1::Identifier)
             schema = Ifc4x3_tc1::create_schema();
    #endif
    #ifdef HAS_SCHEMA_4x3_add1
        if(upperName == Ifc4x3_add1::Identifier)
             schema = Ifc4x3_add1::create_schema();
    #endif
    #ifdef HAS_SCHEMA_4x3_add2
        if(upperName == Ifc4x3_add2::Identifier)
             schema = Ifc4x3_add2::create_schema();
    #endif
    if(!schema)
        throw IfcParse::IfcException("No schema named " + name);
    s_instance->schemas[upperName] = schema;
    
	std::cout << std::this_thread::get_id() << " Finished writing" << std::endl;
    return schema;
}

void IfcParse::internal::IfcStaticSchemaCache::register_schema(std::shared_ptr<schema_definition> schema, bool replaceIfLoaded) {
	if(!schema) return;
	auto schemaName = schema->name();
    // try get existing cached entry
    auto result = try_get_schema(schemaName);
    if(result && result == schema) {
        return;
    }
    // we could not find the entry - insert it - but we need a write lock for that

    std::lock_guard<std::shared_mutex> lock(instance_mutex);
	std::cout << std::this_thread::get_id() << " Registering new schema" << std::endl;
    // we successfully got a write lock - now check if we need to create the entire data or only the schema entry
    if(!s_instance) {
        s_instance = std::unique_ptr<IfcStaticSchemaCache>(new IfcStaticSchemaCache());
    }
	auto it = s_instance->schemas.find(schemaName);
	if(!replaceIfLoaded && it != s_instance->schemas.end()) {
		return;
	}
	s_instance->schemas[schemaName] = schema;
	std::cout << std::this_thread::get_id() << " Finished registering" << std::endl;
}