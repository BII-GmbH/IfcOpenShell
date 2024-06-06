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

FIND_PACKAGE(SWIG)
IF(NOT SWIG_FOUND)
	MESSAGE(FATAL_ERROR "BUILD_IFC_CSHARP enabled, but unable to find SWIG. Disable BUILD_IFC_CSHARP or fix SWIG paths to proceed.")
ENDIF()
include(GNUInstallDirs)
INCLUDE(${SWIG_USE_FILE})

INCLUDE_DIRECTORIES(${CMAKE_CURRENT_SOURCE_DIR})

SET(CMAKE_SWIG_FLAGS ${SWIG_DEFINES})

set(CMAKE_SWIG_OUTDIR "${CMAKE_BINARY_DIR}/IfcOpenShell.Net")
SET_SOURCE_FILES_PROPERTIES(IfcCSharp.i PROPERTIES CPLUSPLUS ON)
swig_add_library(ifcopenshell_net_wrapper LANGUAGE csharp OUTPUT_DIR "${CMAKE_BINARY_DIR}/IfcOpenShell.Net/IfcOpenShell.Net" OUTFILE_DIR "${CMAKE_BINARY_DIR}/IfcOpenShell.Net" SOURCES IfcCSharp.i)
set_property(TARGET ifcopenshell_net_wrapper PROPERTY SWIG_COMPILE_OPTIONS -namespace IfcOpenShell)

SWIG_LINK_LIBRARIES(ifcopenshell_net_wrapper ${IFCOPENSHELL_LIBRARIES} ${OPENCASCADE_LIBRARIES} ${Boost_LIBRARIES} libsvgfill)

if ((NOT WIN32) AND BUILD_SHARED_LIBS)
    SET_INSTALL_RPATHS(${SWIG_MODULE_ifcopenshell_wrapper_REAL_NAME} "${IFCDIRS};${OCC_LIBRARY_DIR}")
endif()

set_target_properties(ifcopenshell_net_wrapper
    PROPERTIES
    ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_SWIG_OUTDIR}/bin"
    LIBRARY_OUTPUT_DIRECTORY "${CMAKE_SWIG_OUTDIR}/bin"
    RUNTIME_OUTPUT_DIRECTORY "${CMAKE_SWIG_OUTDIR}/bin"
)

add_custom_target(ifcopenshell_net ALL 
    #rm "${CMAKE_SWIG_OUTDIR}/*.cxx" "${CMAKE_SWIG_OUTDIR}/*.cs"
    dotnet new sln --name "IfcOpenShell.Net" --output "${CMAKE_SWIG_OUTDIR}" --force
    COMMAND dotnet new classlib --name "IfcOpenShell.Net" -f netstandard2.0 --output "${CMAKE_SWIG_OUTDIR}/IfcOpenShell.Net" --force
    COMMAND dotnet sln "${CMAKE_SWIG_OUTDIR}/IfcOpenShell.Net.sln" add "${CMAKE_SWIG_OUTDIR}/IfcOpenShell.Net/IfcOpenShell.Net.csproj"
    COMMAND dotnet build "${CMAKE_SWIG_OUTDIR}/IfcOpenShell.Net.sln" --output "${CMAKE_SWIG_OUTDIR}/bin"
)

add_dependencies(ifcopenshell_net ifcopenshell_net_wrapper)