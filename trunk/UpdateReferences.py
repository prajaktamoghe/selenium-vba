#!/usr/bin/python

import sys, os, time, types, re, traceback, zipfile, threading, io, datetime, csv, requests

def main():

    set_current_folder(this_folder() + r'\wrapper\References\\')

    print '==========================================================================='
    print 'Project  : SeleniumWrapper'
    print 'Tasks    : Create or update references'
    print 'Date     : ' + time.strftime('%Y-%m-%d %H:%M:%S')
    print 'Previous : ' + file_datetime('versions.txt', format = '%Y-%m-%d %H:%M:%S')
    print '==================================START===================================='
    print ''

    #execute updates in parallel
    with DictionaryFile('versions.txt') as urls_dict:
        tasks = Tasks(urls_dict)
        exitcode = ParallelWorker(tasks).run(pattern='^update_')

    print ''
    print '==============================END OF SCRIPT================================'
    return exitcode

class Tasks():

    def __init__(self, urls_dict):
        self.urls = urls_dict

    def update_SeleniumLibraries(self):
        url = r"http://selenium-release.storage.googleapis.com/"
        value, version = WebSource(url).findlastversion(r'<Key>([\d\.]+/selenium-dotnet-([\d\.]+).zip)', group_value=1, group_version=2)
        url += value
        url +=  r'?etag=' + WebSource(url).getEtag()
        if self.urls['.NetLibraries'] != url or not file_exists('WebDriver.dll'):
            with WebZip(url) as zip:
                zip.extract(r'^net35/.')
            WebFile('http://selenium.googlecode.com/git/dotnet/CHANGELOG').save('WebDriver.changelog.txt')
            self.urls['.NetLibraries'] = url
            print "Updated Selenium .Net to version " + version

    def update_IE32(self):
        url = r"http://selenium-release.storage.googleapis.com/"
        value, version = WebSource(url).findlastversion( r'<Key>([\d\.]+/IEDriverServer_Win32_([\d\.]+).zip)', group_value=1, group_version=2)
        url += value
        url +=  r'?etag=' + WebSource(url).getEtag()
        if self.urls['IE32'] != url or not file_exists('IEDriverServer.exe'):
            with WebZip(url) as zip:
                zip.extract(r'IEDriverServer.exe')
            self.urls['IE32'] = url
            print "Updated IE32 driver to version " + version

    def update_IE64(self):
        url = r"http://selenium-release.storage.googleapis.com/"
        value, version = WebSource(url).findlastversion( r'<Key>([\d\.]+/IEDriverServer_x64_([\d\.]+).zip)', group_value=1, group_version=2)
        url += value
        url +=  r'?etag=' + WebSource(url).getEtag()
        if self.urls['IE64'] != url or not file_exists('IEDriverServer64.exe'):
            with WebZip(url) as zip:
                zip.extract(r'IEDriverServer.exe', 'IEDriverServer64.exe')
            self.urls['IE64'] = url
            print "Updated IE64 driver to version " + version

    def update_SeleniumIDE(self):
        url = r'http://release.seleniumhq.org/selenium-ide/'
        value, version = WebSource(url).findlastversion(r'href="((\d[\d\.]+)/)"', group_value=1, group_version=2)
        url += value
        url += WebSource(url).findfirst(r'href="(selenium-ide-[\d\.]+\.xpi)"')
        url +=  r'?etag=' + WebSource(url).getEtag()
        if self.urls['SeleniumIDE'] != url or not file_exists('selenium-ide.xpi'):
            WebFile(url).save('selenium-ide.xpi')
            self.urls['SeleniumIDE'] = url
            print "Updated Selenium IDE to version " + version

    def update_ChromeDriver(self):
        url = r"http://chromedriver.storage.googleapis.com/"
        version = WebSource(url + r'LATEST_RELEASE').gettext().strip()
        url += version + r'/chromedriver_win32.zip'
        url +=  r'?etag=' + WebSource(url).getEtag()
        if self.urls['Chrome'] != url or not file_exists('chromedriver.exe'):
            with WebZip(url) as zip:
                zip.extract(r'chromedriver.exe')
            self.urls['Chrome'] = url
            print "Updated Chrome driver to version " + version

    def update_PhantomJS(self):
        url = r"https://bitbucket.org/ariya/phantomjs/downloads/"
        value, version = WebSource(url).findlastversion(r'href="/ariya/phantomjs/downloads/(phantomjs-([\d\.]+)-windows.zip)"', group_value=1, group_version=2)
        url += value
        url +=  r'?etag=' + WebSource(url).getEtag()
        if self.urls['PhantomJs'] != url or not file_exists('phantomjs.exe'):
            with WebZip(url) as zip:
                zip.extract(r'.*/phantomjs.exe')
                zip.extract(r'.*/LICENSE.BSD', 'phantomjs.license.txt')
            self.urls['PhantomJs'] = url
            print "Updated PhantomJS to version " + version

    def update_Safari(self):
        url = r'https://selenium.googlecode.com/git/javascript/safari-driver/prebuilt/SafariDriver.safariextz'
        etag = WebSource(url).getEtag()
        url += '?etag=' + etag
        if self.urls['Safari'] != url or not file_exists('SafariDriver.safariextz'):
            WebFile(url).save('SafariDriver.safariextz')
            self.urls['Safari'] = url
            print "Updated Safari driver to version " + etag

    def update_PdfSharp(self):
        url = r'http://sourceforge.net/projects/pdfsharp/files/pdfsharp/'
        value, version = WebSource(url).findlastversion( r'href="/projects/pdfsharp/files/pdfsharp/(PDFsharp%20([\d\.]+)/)', group_value=1, group_version=2)
        url += value
        url = r'http://sunet.dl.sourceforge.net/project/pdfsharp/pdfsharp/' + WebSource(url).findfirst(r'([^/]+/[^/]*Assemblies[^/]*\.zip)/download')
        url +=  r'?etag=' + WebSource(url).getEtag()
        if self.urls['PDFsharp'] != url or not file_exists( 'PdfSharp.dll'):
            with WebZip(url) as zip:
                zip.extract(r'.*/PdfSharp.dll')
            self.urls['PDFsharp'] = url
            print "Updated PDF Sharp to version " + version


def this_folder():
    return os.path.dirname(os.path.realpath(__file__));

def set_current_folder(folder):
    if not os.path.isdir(folder):
        os.makedirs(folder)
    os.chdir(folder)

def file_exists(file_path):
    return os.path.isfile(file_path);

def file_datetime(filepath, format='%c', default='none'):
    if not os.path.isfile(filepath) :
        return default
    return datetime.datetime.fromtimestamp(os.path.getmtime(filepath)).strftime(format)

def exit(exitcode=0):
    sys.stderr = ''
    sys.exit(exitcode)

def format_trace(tb, indent='  ', tb_start=0):
    text = ''
    for filename, lineno, name, line in traceback.extract_tb(tb)[tb_start:]:
        text += indent + 'File %s line %d in %s\n' % (os.path.basename(filename), lineno, name)
        if line:
            text += (indent * 2) + '%s\n' % line.strip()
    return text


class DictionaryFile(dict):

    def __init__(self, filepath):
        self.lock = threading.Lock()
        self.filepath = filepath
        if os.path.isfile(filepath):
            with open(filepath, 'r') as file:
                #self.saved_versions = json.load(file)
                for key, val in csv.reader(file, delimiter=':', quotechar='"', lineterminator='\n'):
                    try:
                        self[key] = val
                    except: pass

    def __enter__(self):
        return self

    def __exit__(self, type, value, traceback):
        with open(self.filepath, 'w') as file:
            #json.dump(self, file, sort_keys = False, indent = 4, ensure_ascii=False)
            w = csv.writer(file, delimiter=':', quotechar='"', lineterminator='\n')
            for key, val in self.items():
                w.writerow([key, val])

    def __getitem__(self, key):
        with self.lock:
            return self.get(key)

    def __setitem__(self, key, value):
        with self.lock:
            dict.__setitem__(self, key, value)


from Queue import Queue

class ParallelWorker(Queue):

    def __init__(self, instance, max_workers=10):
        Queue.__init__(self)
        self.instance = instance
        self.exitcode = 0
        self.max_workers = max_workers
        self._console_lock = threading.Lock()

    def __run__(self):
        while not self.empty():
            method = self.get()
            try:
                method()
            except Exception as ex:
                self.exitcode = 1
                e_type, e_value, e_trace = sys.exc_info()
                sys.stderr.write('ERROR in %s line %s\n %s: %s\n%s\n' % (method.__name__, e_trace.tb_next.tb_lineno, e_type.__name__, str(e_value), format_trace(e_trace, tb_start=1)) )
            finally:
                self.task_done()
                sys.exc_clear()

    def run(self, pattern=''):
        methods = [getattr(self.instance, k) for k, v in self.instance.__class__.__dict__.items() if isinstance(v, types.FunctionType) and re.search(pattern, k)]
        nb_workers = min(self.max_workers, len(methods))
        for method in methods:
            self.put(method)
        for i in range(nb_workers):
            t = threading.Thread(target=self.__run__)
            t.daemon = True
            t.start()
        self.join()
        return self.exitcode

class WebZip:

    def __init__(self, url):
        response = requests.get(url)
        response.raise_for_status()
        buffer = io.BytesIO(response.content)
        self.zip = zipfile.ZipFile(buffer)

    def __enter__(self):
        return self

    def __exit__(self, type, value, traceback):
        self.zip.close()

    def extract(self, pattern, target = '.'):
        p = re.compile(pattern)
        isdir = os.path.isdir(target)
        for name in self.zip.namelist():
            if p.match(name):
                if isdir:
                    with open(target.strip('\\') + '\\' + os.path.basename(name), 'wb') as file:
                        file.write(self.zip.read(name))
                else:
                    with open(target, 'wb') as file:
                        file.write(self.zip.read(name))
                        return

class WebFile:

    def __init__(self, url):
        self.url = url

    def save(self, file):
        response = requests.get(self.url, stream=True)
        response.raise_for_status()
        with open(file, 'wb') as handle:
            for block in response.iter_content(1024):
                if not block:
                    break
                handle.write(block)


class WebSource:

    def __init__(self, url):
        self.url = url
        self.text = None

    def gettext(self):
        if self.text is None:
            self.text = requests.get(self.url).text
        return self.text

    def findfirst(self, pattern, group=-1):
        res = re.search(pattern, self.gettext())
        if not res:
            raise PatternNotFound(pattern, self.url)
        return res.group(res.re.groups if group == -1 else group)

    def getEtag(self, default='none'):
        try:
            etag = requests.head(self.url).headers['etag']
            return re.search(r'[\w-]+', etag).group(0)
        except:
            return default

    def findlastversion(self, pattern, group_value=1, group_version=2):
        match = re.finditer(pattern, self.gettext())
        lst_versions = [(m.group(group_value), m.group(group_version), map(int, m.group(group_version).split('.'))) for m in match]
        if not lst_versions:
            raise PatternNotFound(pattern, self.url)
        lst_versions.sort(key=lambda m: m[2], reverse=True)
        return (lst_versions[0][0], lst_versions[0][1])

    def findlastdate(self, pattern, group_value=1, group_datetime=2, datetime_format='%Y-%m-%dT%H:%M:%S'):
        match = re.finditer(pattern, self.gettext())
        lst_dates = [(m.group(group_value), time.strptime(m.group(group_datetime), datetime_format)) for m in match]
        if not lst_dates:
            raise PatternNotFound(pattern, self.url)
        lst_dates.sort(key=lambda m: m[1], reverse=True)
        return lst_dates[0]

class PatternNotFound(Exception):

    def __init__(self, pattern, source):
        self.__data__ = (pattern, source)

    def __str__(self):
        return 'Pattern "%s" not found in %s' % self.__data__

class Logger:

    def __init__(self, filename=os.path.basename(__file__)[:-3] + '.log'):
        self.lock = threading.Lock()
        self.log = open(filename, "w")
        sys.stdout = sys.stderr = self

    def __enter__(self):
        return self

    def __exit__(self, type, value, traceback):
        sys.stdout = sys.__stdout__
        sys.stderr = sys.__stderr__
        self.log.close()

    def write(self, message):
        with self.lock:
            sys.__stdout__.write(message)
            self.log.write(message)

    def write_exception(self):
        e_type, e_value, e_trace = sys.exc_info()
        self.write('%s: %s\n%s\n' % (e_type.__name__, e_value, format_trace(e_trace)) )

if __name__ == '__main__':
    with Logger() as log:
        try:
            exitcode = main()
        except:
            exitcode = 1
            log.write_exception()
        log.write('SystemExit: %s (%s)\n' % (exitcode, 'SUCCEED' if exitcode == 0 else 'FAILED'))
    exit(exitcode)