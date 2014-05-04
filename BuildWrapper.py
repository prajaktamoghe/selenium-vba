import os, string, re, time, fileinput, sys, shutil, urllib, datetime, subprocess

_projet_name = 'SeleniumWrapper'
_current_dir = os.getcwd() + '\\'
_assembly_info_path = _current_dir + r'wrapper\Properties\AssemblyInfo.cs'
_iss_path = _current_dir + r'wrapper\Package.iss'
_seleniumide_path = _current_dir + r'wrapper\References\selenium-ide.xpi'
_formaters_path = _current_dir + r'formatters\vb-format.xpi'

_dxroot_dir = r'c:\Progra~1\Sandcastle'
_shfbroot_dir = r'c:\Progra~1\EWSoftware\Sandcastle Help File Builder'
_sevenzip_path = r'c:\Progra~1\7-Zip\7z.exe'
_innosetup_path = r'c:\Progra~1\Inno Setup 5\ISCC.exe'
_msbuild_path = r'c:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe'

def main():

	check_paths()
	
	os.environ['DXROOT'] = _dxroot_dir
	os.environ['SHFBROOT'] = _shfbroot_dir
	
	last_modified_time = get_file_datetime(_assembly_info_path)
	current_version = find_in_file(_assembly_info_path, r'AssemblyFileVersion\("([.\d]+)"\)')

	print('')
	print('Project : ' + _projet_name)
	print('Tasks   : Build and package')
	print('Current Version  : ' + current_version)
	print('Last compilation : ' + (last_modified_time.strftime('%Y-%m-%d %H:%M:%S') if last_modified_time else 'none'))
	print('_______________________________________________________________________\n')

	new_version = get_new_version(current_version)

	print('New version : ' + new_version + '\n')
	print('** Update version number...')
	replace_in_file(_assembly_info_path, r'AssemblyFileVersion\("[.\d]+"\)', r'AssemblyFileVersion("' + new_version + '")')

	print('** Clear previous compilations ...')
	delete_folder(_current_dir + r'wrapper\bin')
	delete_folder(_current_dir + r'wrapper\obj')
	delete_folder(_current_dir + r'SScript\bin')
	delete_folder(_current_dir + r'SScript\obj')

	print('** Compile main library ...')
	exec_msbuild( _current_dir + r'wrapper\SeleniumWrapper.csproj')
	
	print('** Compile console runner ...')
	exec_msbuild( _current_dir + r'\VbsConsoleRunner\VbsConsoleRunner.csproj')

	print('** Include the formatters ...')
	exec_command( _sevenzip_path, 'a', '-tzip', _seleniumide_path, _formaters_path)

	if(get_input('Create the .chm help file [y/n] ? ') == 'y'):
		print('** Api documentation creation ...')
		exec_command( _msbuild_path,'/v:quiet', '/p:Configuration=Release;CleanIntermediates=True', _current_dir + 'wrapper\SeleniumWrapper.shfbproj')
		
	print('** Build setup package ...')
	exec_command( _innosetup_path, '/q', '/O' + _current_dir, _iss_path)

	print('\n__________________________________________________________END OF SCRIPT')
	
def check_paths():
	for path in [ p for p in dir() if re.match( '.+_path$|.+_dir$ ', p) and not os.path.exists(eval(p)) ] :
		 print('Missing ' + path + '=' + eval(path))

def get_file_datetime(filepath):
	if not os.path.isfile(filepath):
		return None
	return datetime.datetime.fromtimestamp(os.path.getmtime(filepath))
		 
def find_in_file(filepath, pattern):
	with open(filepath, 'r') as f:
		result = re.search(pattern, f.read())
		return result.group(result.re.groups)
		 
def replace_in_file(filepath, pattern, replacement):
	with open(filepath, 'r') as f:
		text = f.read()
	with open(filepath, 'w') as f:
		f.write(re.sub(pattern, replacement, text))

def delete_folder(folderpath):
	if os.path.isdir(folderpath):
		shutil.rmtree(folderpath)

def exec_command(*arguments):
	p = subprocess.Popen(arguments, stdout=subprocess.PIPE, stderr=subprocess.PIPE, shell=False)
	p.wait()
	if p.returncode != 0:
		print('\n>COMMAND:\n' + ' '.join(arguments) + '\n\n>ERROR:\n')
		print((p.stdout.read().strip('\r\n') + '\n' + p.stderr.read().strip('\r\n')).decode("utf-8"))
		quit()

def exec_msbuild(csproj):
	exec_command( _msbuild_path, '/v:quiet', '/t:build', '/p:Configuration=Release;TargetFrameworkVersion=v3.5', csproj)
	
def get_input(message):
    try:
		return raw_input(message)
    except NameError:
		return input(message)

def get_new_version(version):
	new_version = ''
	matrix_add = {'s': [0,0,0,0], 'z':[0,0,0,1], 'y':[0,0,1,0], 'x':[0,1,0,0], 'w':[1,0,0,0]}
	matrix_mul = {'s': [1,1,1,1], 'z':[1,1,1,1], 'y':[1,1,1,0], 'x':[1,1,0,1], 'w':[1,0,1,1]}
	while new_version == '':
		input = get_input('Digit to increment [w.x.y.z] or version [0.0.0.0] or skip [s] ? ').strip()
		if re.match(r's|z|y|x|w', input) :
			add = matrix_add[input]
			mul = matrix_mul[input]
			values = [int(d) for d in version.split('.')]
			new_version = '.'.join( str( values[i] * mul[i] + add[i] ) for i in range(0,4))
		elif re.match(r'\d+\.\d+\.\d+\.\d+', input):
			new_version = input
	return new_version

class Logger:

	def __init__(self, filename=os.path.basename(__file__)[:-3] + '.log'):
		self.terminal = sys.stdout
		self.log = open(filename, "w")
		sys.stdout = sys.stderr = self
	
	def __enter__(self):
		return self
	
	def __exit__(self, type, value, traceback):
		self.log.close()
	
	def write(self, message):
		self.terminal.write(message)
		self.log.write(message)

if __name__ == '__main__':
	with  Logger() as log:
		main()
