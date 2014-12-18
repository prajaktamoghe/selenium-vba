#!/usr/bin/python

import os, re, time, sys, traceback, shutil, datetime, subprocess

_projet_name = 'SeleniumWrapper'
_cwd = os.getcwd() + '\\'
_assembly_info_path = _cwd + r'wrapper\Properties\AssemblyInfo.cs'
_iss_path = _cwd + r'wrapper\Package.iss'
_seleniumide_path = _cwd + r'wrapper\References\selenium-ide.xpi'
_formaters_path = _cwd + r'formatters\vb-format.xpi'

_shfbroot_dir = r'c:\Progra~1\EWSoftware\Sandcastle Help File Builder'
_sevenzip_path = r'c:\Progra~1\7-Zip\7z.exe'
_innosetup_path = r'c:\Progra~1\Inno Setup 5\ISCC.exe'
_msbuild_path = r'c:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe'

def main():
    check_globals_exists('_path$|_dir$')

    os.environ['SHFBROOT'] = _shfbroot_dir

    last_modified_time = get_file_datetime(_assembly_info_path)
    current_version = find_in_file(_assembly_info_path, r'AssemblyFileVersion\("([.\d]+)"\)')

    print ''
    print 'Project : ' + _projet_name
    print 'Tasks   : Build and package'
    print 'Current Version  : ' + current_version
    print 'Last compilation : ' + (last_modified_time and '{:%Y-%m-%d %H:%M:%S}'.format(last_modified_time) or 'none')
    print '_______________________________________________________________________\n'

    new_version = get_new_version(current_version)

    print 'New version : ' + new_version + '\n'
    print '** Update version number...'
    replace_in_file(_assembly_info_path, r'AssemblyFileVersion\("[.\d]+"\)', r'AssemblyFileVersion("' + new_version + '")')

    print '** Clear previous compilations ...'
    delete_folder(_cwd + r'wrapper\bin')
    delete_folder(_cwd + r'wrapper\obj')
    delete_folder(_cwd + r'SScript\bin')
    delete_folder(_cwd + r'SScript\obj')

    print '** Compile main library ...'
    exec_cmd(_msbuild_path, '/v:quiet', '/t:build', '/p:Configuration=Release;TargetFrameworkVersion=v3.5', _cwd + r'wrapper\SeleniumWrapper.csproj')

    print '** Compile console runner ...'
    exec_cmd(_msbuild_path, '/v:quiet', '/t:build', '/p:Configuration=Release;TargetFrameworkVersion=v3.5', _cwd + r'VbsConsoleRunner\VbsConsoleRunner.csproj')

    print '** Include the formatters ...'
    exec_cmd(_sevenzip_path, 'a', '-tzip', _seleniumide_path, _formaters_path)

    if(get_input('Create the .chm help file [y/n] ? ') == 'y'):
        print '** Api documentation creation ...'
        exec_cmd(_msbuild_path, '/v:quiet', '/p:Configuration=Release;CleanIntermediates=True', _cwd + 'wrapper\SeleniumWrapper.shfbproj')

    print '** Build setup package ...'
    exec_cmd(_innosetup_path, '/q', '/O' + _cwd, _iss_path)

    print '\n__________________________________________________________END OF SCRIPT'



def check_globals_exists(pattern):
    paths = [p for p in globals() if re.search(pattern, p) and not os.path.exists(eval(p))]
    if paths:
        raise Exception('Missing path(s):\n ' + '\n '.join(['{}: {}'.format(p, eval(p)) for p in paths]))


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


class CommandException(Exception):
    pass


def exec_cmd(*arguments):
    p = subprocess.Popen(arguments, stdout=subprocess.PIPE, stderr=subprocess.PIPE, shell=False)
    p.wait()
    if p.returncode != 0:
        msg = '\n\nCommand arguments:\n("{}")\n\n{}\n{}'.format(
            '", "'.join(arguments),
            p.stdout.read().strip('\r\n').strip('\n').strip('\r').decode("utf-8"),
            p.stderr.read().strip('\r\n').strip('\n').strip('\r').decode("utf-8")
        )
        raise CommandException(msg)


def get_input(message):
    try:
        return raw_input(message)
    except NameError:
        return input(message)


def get_new_version(version):
    while True:
        input = get_input('Digit to increment [w.x.y.z] or version [0.0.0.0] or skip [s] ? ').strip()
        if re.match(r's|w|x|y|z', input) :
            idx = {'s': 99, 'w': 0, 'x': 1, 'y': 2, 'z': 3}[input]
            return '.'.join([str((int(v)+(i == idx))*(i <= idx)) for i, v in enumerate(version.split('.'))])
        elif re.match(r'\d+\.\d+\.\d+\.\d+', input):
            return input


class Logger:
    
    def __init__(self, filename=os.path.basename(__file__)[:-3] + '.log'):
        self.terminal = sys.stdout
        self.logfile = open(filename, "a")
        sys.stdout = self
    
    def __enter__(self):
        return self
    
    def __exit__(self, e_type, e_value, e_trace):
        if e_type:
            self.write_file(''.join(traceback.format_exception(e_type, e_value, e_trace)))
        self.logfile.write('\n')
        self.logfile.close()

    def write(self, message):
        self.terminal.write(message)
        self.write_file(message)

    def write_file(self, message):
        tm = '\n{} '.format(time.strftime('%Y-%m-%d %H:%M:%S'))
        self.logfile.write(message.replace('\r\n', '\n').replace('\r', '\n').replace('\n', tm))


if __name__ == '__main__':
    with Logger() as log:
        main()
