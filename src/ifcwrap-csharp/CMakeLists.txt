################################################################################
#                                                                              #
# This file is part of IfcOpenShell.                                           #
#                                                                              #
# IfcOpenShell is free software: you can redistribute it and/or modify         #
# it under the terms of the Lesser GNU General Public License as published by  #
# the Free Software Foundation, either version 3.0 of the License, or          #
# (at your option) any later version.                                          #
#                                                                              #
# IfcOpenShell is distributed in the hope that it will be useful,              #
# but WITHOUT ANY WARRANTY; without even the implied warranty of               #
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 #
# Lesser GNU General Public License for more details.                          #
#                                                                              #
# You should have received a copy of the Lesser GNU General Public License     #
# along with this program. If not, see <http://www.gnu.org/licenses/>.         #
#                                                                              #
################################################################################

# Check that swig is present
FIND_PACKAGE(SWIG)
IF(NOT SWIG_FOUND)
	MESSAGE(FATAL_ERROR "BUILD_IFC_CSHARP enabled, but unable to find SWIG. Disable BUILD_IFC_CSHARP or fix SWIG paths to proceed.")
ENDIF()
include(GNUInstallDirs)
INCLUDE(${SWIG_USE_FILE})

INCLUDE_DIRECTORIES(${CMAKE_CURRENT_SOURCE_DIR})

SET(CMAKE_SWIG_FLAGS ${SWIG_DEFINES})

# setup output directory
set(CMAKE_SWIG_OUTDIR "${CMAKE_BINARY_DIR}/IfcOpenShell.Net")
# tell cmake to treat this file as c++ source (it does not know the .i extension and thus does not know what to do with the file otherwise)
SET_SOURCE_FILES_PROPERTIES(IfcCSharp.i PROPERTIES CPLUSPLUS ON)
# use the cmake swig integration to setup the wrapper generation for csharp
swig_add_library(ifcopenshell_net_wrapper LANGUAGE csharp OUTPUT_DIR "${CMAKE_CURRENT_SOURCE_DIR}/IfcOpenShell.Net/IfcOpenShell.Net/generated" OUTFILE_DIR "${CMAKE_BINARY_DIR}/IfcOpenShell.Net" SOURCES IfcCSharp.i)
# we want to get a nice namespace in C#. so set it here.
set_property(TARGET ifcopenshell_net_wrapper PROPERTY SWIG_COMPILE_OPTIONS -namespace IfcOpenShell.Net)

# setup linking to the required libraries
SWIG_LINK_LIBRARIES(ifcopenshell_net_wrapper ${IFCOPENSHELL_LIBRARIES} ${OPENCASCADE_LIBRARIES} ${Boost_LIBRARIES} libsvgfill)

if ((NOT WIN32) AND BUILD_SHARED_LIBS)
    SET_INSTALL_RPATHS(${SWIG_MODULE_ifcopenshell_wrapper_REAL_NAME} "${IFCDIRS};${OCC_LIBRARY_DIR}")
endif()

# force swig output to all go in the same folder
set_target_properties(ifcopenshell_net_wrapper
    PROPERTIES
    ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_SWIG_OUTDIR}/bin"
    LIBRARY_OUTPUT_DIRECTORY "${CMAKE_SWIG_OUTDIR}/bin"
    RUNTIME_OUTPUT_DIRECTORY "${CMAKE_SWIG_OUTDIR}/bin"
)

# all of the following may be MSVC specific - needs testing on different platforms

# output files of the csharp build - define these so we can copy them on INSTALL
set(libraryfiles "${CMAKE_BINARY_DIR}/IfcOpenShell.Net/bin/IfcOpenShell.Net.deps.json" "${CMAKE_BINARY_DIR}/IfcOpenShell.Net/bin/IfcOpenShell.Net.dll" "${CMAKE_BINARY_DIR}/IfcOpenShell.Net/bin/IfcOpenShell.Net.pdb")

# custom build target for the C# build using dotnet.exe
add_custom_target(ifcopenshell_net ALL 
    BYPRODUCTS "${libraryfiles}"
    COMMAND dotnet build "${CMAKE_CURRENT_SOURCE_DIR}/IfcOpenShell.Net/IfcOpenShell.Net/IfcOpenShell.Net.csproj" --output "${CMAKE_SWIG_OUTDIR}/bin"
)

add_dependencies(ifcopenshell_net ifcopenshell_net_wrapper)

# on install, copy the generated files to the install out folder
FOREACH(file ${libraryfiles})
    FILE(RELATIVE_PATH relative "${CMAKE_BINARY_DIR}/IfcOpenShell.Net/bin/" "${file}")
    GET_FILENAME_COMPONENT(dir "${relative}" DIRECTORY)
    
    INSTALL(FILES "${file}" DESTINATION "IfcOpenShell.Net")
ENDFOREACH()
INSTALL(TARGETS ifcopenshell_net_wrapper
        DESTINATION "IfcOpenShell.Net")
# custom logic for microsoft compiler to copy the pdb
if (MSVC)
    install(FILES $<TARGET_PDB_FILE:ifcopenshell_net_wrapper> DESTINATION bin OPTIONAL)
endif()



