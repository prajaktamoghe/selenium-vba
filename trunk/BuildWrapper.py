import os, string, re, time, fileinput, sys, shutil
from datetime import datetime
from subprocess import Popen, PIPE

Project_name = 'SeleniumWrapper';
Current_dir = os.getcwd() + '\\';
AssemblyInfo_path = Current_dir + r'wrapper\Properties\AssemblyInfo.cs';
iss_path = Current_dir + r'wrapper\Package.iss';

DXROOT_dir = "c:\Progra~1\Sandcastle"
SHFBROOT_dir = "c:\Progra~1\EWSoftware\Sandcastle Help File Builder"
sevenzip_path = r"C:\Program Files\7-Zip\7zFM.exe";
innosetup_path = r"C:\Program Files\Inno Setup 5\ISCC.exe";
msbuild_path = r"C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";

def CheckPaths():
	for path in dir():
		if( path.endswith('_path') and not os.path.isfile(eval(path)) ) : print('Missing ' + path + '=' + eval(path));
		elif( path.endswith('_dir') and not os.path.isdir(eval(path)) ) : print('Missing ' + path + '=' + eval(path));

def ReplaceInFile(file_path, pattern, replacement):
    text = open(file_path, 'r').read();
    open(file_path, 'w').write( re.sub(pattern, replacement, text ) );
    return;

def DeleteFolder(folder_path):
	if(os.path.isdir(folder_path)) : shutil.rmtree(folder_path);
	return;

def RunCommand(arguments):
    proc = Popen(arguments, stdout=PIPE, stderr=PIPE, shell=False)
    proc.wait()
    if(proc.returncode != 0):
        print('\r\n  ERROR while executing command:\r\n\r\n' + ' '.join(arguments) + '\r\n')
        print('\r\n  ERROR details:\r\n')
        for line in proc.stdout: print(line.decode("utf-8").replace('\r\n', ''))
        for line in proc.stderr: print(line.decode("utf-8").replace('\r\n', ''))
        print('\r\n\r\n')
        return False;
    return True;

def GetInput(message):
    try: return raw_input(message);
    except NameError: return input(message);

def GetVersion(version):
    new_version =""
    while (new_version == ""):
        res = GetInput("Digit to increment [w.x.y.z] or version [0.0.0.0] or skip [s] ? ");
        if ( re.match(r"^s|z|y|x|w$", res)) :
            version_digit = version.split('.');
            new_version = {
                "s" : version,
                "z" : version_digit[0] + "." + version_digit[1] + "." + version_digit[2] + "." + str(int(version_digit[3])+1),
                "y" : version_digit[0] + "." + version_digit[1] + "." + str(int(version_digit[2])+1) + ".0",
                "x" : version_digit[0] + "." + str(int(version_digit[1]) + 1) + ".0.0",
                "w" : str(int(version_digit[0])+1) + ".0.0.0",
            }.get(res, '');
        elif ( re.match(r"^\d+\.\d+\.\d+\.\d+$", res) ):
            new_version = res;
    return new_version

def MsBuild(csproj):
	return RunCommand([ msbuild_path,'/v:quiet', '/t:build', '/p:Configuration=Release;TargetFrameworkVersion=v3.5', csproj ]);

CheckPaths();
os.environ['DXROOT'] = DXROOT_dir;
os.environ['SHFBROOT'] = SHFBROOT_dir;
CurrentVersion = re.findall(r'AssemblyFileVersion\("([.\d]+)"\)', open(AssemblyInfo_path, 'r').read())[0];
LastModified = datetime.fromtimestamp(os.path.getmtime(AssemblyInfo_path))

print( "_______________________________________________________________________" )
print( "" )
print( "Project name     : " + Project_name )
print( "Current Version  : " + CurrentVersion )
print( "Last compilation : " + LastModified.strftime("%Y-%m-%d %H:%M:%S") )
print( "_______________________________________________________________________\r\n" )

NewVersion = GetVersion(CurrentVersion)

print( "New version : " + NewVersion + "\n")
print( "** Update version number..." )
ReplaceInFile( AssemblyInfo_path, r'AssemblyFileVersion\("[.\d]+"\)', r'AssemblyFileVersion("' + NewVersion + '")' )

print( "** Clear previous compilations ...")
DeleteFolder(Current_dir + r'wrapper\bin' );
DeleteFolder(Current_dir + r'wrapper\obj' );

print( "** Compile main library ...")
if( not MsBuild( Current_dir + r'wrapper\SeleniumWrapper.csproj' ) ): exit(1)

if(GetInput("Create the .chm help file [y/n] ? ") == 'y'):
	print( "** Api documentation creation ...");
	if( not RunCommand([ msbuild_path,'/v:quiet', '/p:Configuration=Release;CleanIntermediates=True', Current_dir + 'wrapper\SeleniumWrapper.shfbproj' ]) ): exit(1)

print( "** Build setup package ...")
if( not RunCommand([ innosetup_path, '/q', '/O'+Current_dir, iss_path ])): exit(1)

print( "\r\nEnd")