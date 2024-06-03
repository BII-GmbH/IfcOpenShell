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
	MESSAGE(FATAL_ERROR "BUILD_IFCPYTHON enabled, but unable to find SWIG. Disable BUILD_IFCPYTHON or fix SWIG paths to proceed.")
ENDIF()
include(GNUInstallDirs)
INCLUDE(${SWIG_USE_FILE})

#IF(NOT "$ENV{PYTHON_INCLUDE_DIR}" STREQUAL "")
#	SET(PYTHON_INCLUDE_DIR $ENV{PYTHON_INCLUDE_DIR} CACHE FILEPATH "Python header files")
#	MESSAGE(STATUS "Looking for Python header files in: ${PYTHON_INCLUDE_DIR}")
#ENDIF()
#IF(NOT "$ENV{PYTHON_LIBRARY}" STREQUAL "")#
#	SET(PYTHON_LIBRARY $ENV{PYTHON_LIBRARY} CACHE FILEPATH "Python library file")
#	MESSAGE(STATUS "Looking for Python library file in: ${PYTHON_LIBRARY}")
#ENDIF()

# NOTE PYTHONLIBS_FOUND and PYTHONINTERP_FOUND cannot seem to be trusted so
# we need further checks to see whether the packages were actually found or not.
#FIND_PACKAGE(PythonLibs)
#IF(NOT PYTHONLIBS_FOUND OR "$PYTHON_INCLUDE_DIRS}" STREQUAL "")
#	MESSAGE(FATAL_ERROR "BUILD_IFCPYTHON enabled, but unable to find Python lib or header. Disable BUILD_IFCPYTHON or fix Python paths to proceed.")
#ENDIF()

INCLUDE_DIRECTORIES(${PYTHON_INCLUDE_DIRS})
INCLUDE_DIRECTORIES(${CMAKE_CURRENT_SOURCE_DIR})

SET(CMAKE_SWIG_FLAGS ${SWIG_DEFINES})
# NOTE Workaround for most likely missing debug Python libraries on Windows (requires Python built from the source).
# Python 3.5 intaller and onwards will have an option to install the debug libraries too.
# NOTE PYTHON_DEBUG_LIBRARIES appears to be a deprecated variable
#IF (WIN32 AND NOT PYTHON_DEBUG_LIBRARIES)
#	MESSAGE(STATUS "PYTHON_DEBUG_LIBRARIES not found, defining SWIG_PYTHON_INTERPRETER_NO_DEBUG workaround for IfcWrap.")
#	ADD_DEFINITIONS(-DSWIG_PYTHON_INTERPRETER_NO_DEBUG)
#ENDIF()


set(CMAKE_SWIG_OUTDIR "${CMAKE_BINARY_DIR}/IfcOpenShell.Net")
SET_SOURCE_FILES_PROPERTIES(IfcPython.i PROPERTIES CPLUSPLUS ON)
swig_add_library(ifcopenshell_wrapper_csharp LANGUAGE csharp SOURCES IfcPython.i)
set_property(TARGET ifcopenshell_wrapper_csharp PROPERTY SWIG_COMPILE_OPTIONS -namespace IfcOpenShell)
if("$ENV{LDFLAGS}" MATCHES ".undefined.suppress")
# On osx there is some state in the python dylib. With `-Wl,undefined,suppress` we can ignore the missing symbols at compile time.
SWIG_LINK_LIBRARIES(ifcopenshell_wrapper_csharp ${IFCOPENSHELL_LIBRARIES} ${OPENCASCADE_LIBRARIES} ${Boost_LIBRARIES} libsvgfill)
else()
SWIG_LINK_LIBRARIES(ifcopenshell_wrapper_csharp ${IFCOPENSHELL_LIBRARIES} ${PYTHON_LIBRARIES} ${OPENCASCADE_LIBRARIES} ${Boost_LIBRARIES} libsvgfill)
endif()
if ((NOT WIN32) AND BUILD_SHARED_LIBS)
    SET_INSTALL_RPATHS(${SWIG_MODULE_ifcopenshell_wrapper_REAL_NAME} "${IFCDIRS};${OCC_LIBRARY_DIR}")
endif()

message("OutDir:  ${CMAKE_SWIG_OUTDIR}")
add_custom_target(ifcopenshell_net ALL 
    #rm "${CMAKE_SWIG_OUTDIR}/*.cxx" "${CMAKE_SWIG_OUTDIR}/*.cs"
    # COMMAND 
    dotnet new sln --name "IfcOpenShell.Net" --output "${CMAKE_SWIG_OUTDIR}" --force
    COMMAND dotnet new classlib --name "IfcOpenShell.Net" -f netstandard2.0 --output "${CMAKE_SWIG_OUTDIR}" --force
    COMMAND dotnet sln "${CMAKE_SWIG_OUTDIR}/IfcOpenShell.Net.sln" add "${CMAKE_SWIG_OUTDIR}/IfcOpenShell.Net.csproj"
    COMMAND dotnet build "${CMAKE_SWIG_OUTDIR}/IfcOpenShell.Net.sln"
)

add_dependencies(ifcopenshell_net ifcopenshell_wrapper_csharp)