# Microsoft Developer Studio Project File - Name="app2255demo" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Application" 0x0101

CFG=app2255demo - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "app-2255-demo.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "app-2255-demo.mak" CFG="app2255demo - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "app2255demo - Win32 Debug" (based on "Win32 (x86) Application")
!MESSAGE "app2255demo - Win32 Release" (based on "Win32 (x86) Application")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName "app-2255-demo"
# PROP Scc_LocalPath "."
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "app2255demo - Win32 Debug"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug"
# PROP BASE Intermediate_Dir "Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 6
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Debug"
# PROP Intermediate_Dir "Debug"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MDd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_WINDOWS" /D "_DEBUG" /D "MULTIPLE_CHANNEL" /D "_AFXDLL" /D "_MBCS" /Yu"stdafx.h" /GZ /c
# ADD CPP /nologo /MDd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_WINDOWS" /D "_DEBUG" /D "MULTIPLE_CHANNEL" /D "_AFXDLL" /D "_MBCS" /Yu"stdafx.h" /GZ /c
# ADD BASE MTL /nologo /D "_DEBUG" /win32
# ADD MTL /nologo /D "_DEBUG" /win32
# ADD BASE RSC /l 0x409 /i "$(IntDir)" /d "_DEBUG"
# ADD RSC /l 0x409 /i "$(IntDir)" /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib s2255.lib /nologo /subsystem:windows /debug /machine:IX86 /pdbtype:sept /libpath:"Debug"
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib s2255.lib /nologo /subsystem:windows /debug /machine:IX86 /pdbtype:sept /libpath:"Debug"

!ELSEIF  "$(CFG)" == "app2255demo - Win32 Release"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 5
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Release"
# PROP Intermediate_Dir "Release"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MD /W3 /GX /Zi /D "WIN32" /D "_WINDOWS" /D "NDEBUG" /D "MULTIPLE_CHANNEL" /D "STATIC_LINK" /D "_AFXDLL" /D "_MBCS" /Yu"stdafx.h" /c
# ADD CPP /nologo /MT /W3 /GX /Zi /D "WIN32" /D "_WINDOWS" /D "NDEBUG" /D "MULTIPLE_CHANNEL" /D "_MBCS" /FR /Yu"stdafx.h" /c
# ADD BASE MTL /nologo /D "NDEBUG" /win32
# ADD MTL /nologo /D "NDEBUG" /win32
# ADD BASE RSC /l 0x409 /i "$(IntDir)" /d "NDEBUG"
# ADD RSC /l 0x409 /i "$(IntDir)" /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib s2255.lib /nologo /subsystem:windows /machine:IX86 /pdbtype:sept /libpath:"Release" /opt:ref /opt:icf
# ADD LINK32 s2255.lib /nologo /subsystem:windows /machine:IX86 /pdbtype:sept /libpath:"Release" /opt:ref /opt:icf

!ENDIF 

# Begin Target

# Name "app2255demo - Win32 Debug"
# Name "app2255demo - Win32 Release"
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;def;odl;idl;hpj;bat;asm;asmx"
# Begin Source File

SOURCE=".\app-2255-demo.cpp"
DEP_CPP_APP_2=\
	".\app-2255-demo.h"\
	".\app-2255-demoDoc.h"\
	".\app-2255-demoView.h"\
	".\MainFrm.h"\
	".\s2255.h"\
	".\stdafx.h"\
	
# End Source File
# Begin Source File

SOURCE=".\app-2255-demoDoc.cpp"
DEP_CPP_APP_22=\
	".\app-2255-demoDoc.h"\
	".\image.h"\
	".\s2255.h"\
	".\s2255f.h"\
	".\stdafx.h"\
	
# End Source File
# Begin Source File

SOURCE=".\app-2255-demoView.cpp"
DEP_CPP_APP_225=\
	".\app-2255-demo.h"\
	".\app-2255-demoDoc.h"\
	".\app-2255-demoView.h"\
	".\Color.h"\
	".\Format.h"\
	".\FrameRate.h"\
	".\image.h"\
	".\s2255.h"\
	".\s2255f.h"\
	".\Scale.h"\
	".\Settings.h"\
	".\stdafx.h"\
	
# End Source File
# Begin Source File

SOURCE=.\Color.cpp
DEP_CPP_COLOR=\
	".\app-2255-demo.h"\
	".\Color.h"\
	".\s2255.h"\
	".\stdafx.h"\
	
# End Source File
# Begin Source File

SOURCE=.\Format.cpp
DEP_CPP_FORMA=\
	".\app-2255-demo.h"\
	".\Format.h"\
	".\stdafx.h"\
	
# End Source File
# Begin Source File

SOURCE=.\FrameRate.cpp
DEP_CPP_FRAME=\
	".\app-2255-demo.h"\
	".\app-2255-demoDoc.h"\
	".\FrameRate.h"\
	".\s2255.h"\
	".\s2255f.h"\
	".\stdafx.h"\
	
# End Source File
# Begin Source File

SOURCE=.\image.cpp
DEP_CPP_IMAGE=\
	".\stdafx.h"\
	
# End Source File
# Begin Source File

SOURCE=.\MainFrm.cpp
DEP_CPP_MAINF=\
	".\MainFrm.h"\
	".\stdafx.h"\
	
# End Source File
# Begin Source File

SOURCE=.\Scale.cpp
DEP_CPP_SCALE=\
	".\app-2255-demo.h"\
	".\Scale.h"\
	".\stdafx.h"\
	
# End Source File
# Begin Source File

SOURCE=.\Settings.cpp
DEP_CPP_SETTI=\
	".\app-2255-demo.h"\
	".\app-2255-demoDoc.h"\
	".\s2255.h"\
	".\s2255f.h"\
	".\Settings.h"\
	".\stdafx.h"\
	
# End Source File
# Begin Source File

SOURCE=.\stdafx.cpp
DEP_CPP_STDAF=\
	".\stdafx.h"\
	

!IF  "$(CFG)" == "app2255demo - Win32 Debug"

# ADD CPP /nologo /GX /Yc"stdafx.h" /GZ

!ELSEIF  "$(CFG)" == "app2255demo - Win32 Release"

# ADD CPP /nologo /GX /Yc"stdafx.h"

!ENDIF 

# End Source File
# End Group
# Begin Group "Header Files"

# PROP Default_Filter "h;hpp;hxx;hm;inl;inc;xsd"
# Begin Source File

SOURCE=".\app-2255-demo.h"
# End Source File
# Begin Source File

SOURCE=".\app-2255-demoDoc.h"
# End Source File
# Begin Source File

SOURCE=".\app-2255-demoView.h"
# End Source File
# Begin Source File

SOURCE=.\Color.h
# End Source File
# Begin Source File

SOURCE=.\Format.h
# End Source File
# Begin Source File

SOURCE=.\FrameRate.h
# End Source File
# Begin Source File

SOURCE=.\Resource.h
# End Source File
# Begin Source File

SOURCE=.\s2255.h
# End Source File
# Begin Source File

SOURCE=.\s2255f.h
# End Source File
# Begin Source File

SOURCE=.\Scale.h
# End Source File
# Begin Source File

SOURCE=.\Settings.h
# End Source File
# Begin Source File

SOURCE=.\stdafx.h
# End Source File
# End Group
# Begin Group "Resource Files"

# PROP Default_Filter "rc;ico;cur;bmp;dlg;rc2;rct;bin;rgs;gif;jpg;jpeg;jpe;resx"
# Begin Source File

SOURCE=".\res\app-2255-demo.ico"
# End Source File
# Begin Source File

SOURCE=".\app-2255-demo.rc"
# End Source File
# Begin Source File

SOURCE=".\res\app-2255-demo.rc2"
# End Source File
# Begin Source File

SOURCE=".\res\app-2255-demoDoc.ico"
# End Source File
# Begin Source File

SOURCE=.\res\Toolbar.bmp
# End Source File
# End Group
# Begin Source File

SOURCE=".\res\app-2255-demo.manifest"
# End Source File
# Begin Source File

SOURCE=.\ReadMe.txt
# End Source File
# End Target
# End Project
