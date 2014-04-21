import os, types, httplib2, re, urllib2, urllib, json, sys, traceback, urllib2, hashlib, StringIO, zipfile, requests, threading, Queue, datetime

_project_name = 'SeleniumWrapper';
_current_dir = os.getcwd() + '\\'
_ref_dir = r'.\wrapper\References\\'
_versions_path = r'.\wrapper\References\versions.txt'

def main():
	
	last_update_time = get_file_datetime(_versions_path)
	
	print('' )
	print('Project : ' + _project_name )
	print('Tasks   : Create or update references' )
	print('Last update : ' + (last_update_time.strftime('%Y-%m-%d %H:%M:%S') if last_update_time else 'none'))
	print('_______________________________________________________________________\n' )

	#create the reference folder if missing
	if not os.path.isdir(_ref_dir):
		os.makedirs(_ref_dir)
		
	#execute updates in parallel
	with Tasks(_versions_path) as tasks:
		ParallelWorker(10).run_class_methods(tasks, 'update_')

	print('\n__________________________________________________________END OF SCRIPT')

def get_file_datetime(filepath):
	if not os.path.isfile(filepath):
		return None
	return datetime.datetime.fromtimestamp(os.path.getmtime(filepath))
	
def find_version_in_url(url):
	return re.search( r'\d+(\.+\d+)+', urllib2.unquote(url)).group(0)

class VersionControl(dict):
		
	def __init__(self, filepath):
		self.filepath = filepath
		if os.path.isfile(filepath):
			with open(filepath, 'r') as file:
				self.saved_versions = json.load(file)
		else:
			self.saved_versions = {}
				
	def __enter__(self):
		return self
		
	def __exit__(self, type, value, traceback):
		with open(self.filepath, 'w') as file:
			json.dump(self, file, sort_keys = False, indent = 4, ensure_ascii=False)
		
	def __getitem__(self, key):
		value = dict.get(self, key)
		if value: return value
		return self.saved_versions.get(key)

class ParallelWorker(Queue.Queue):

	def __init__(self, num_worker):
		Queue.Queue.__init__(self)
		for i in range(num_worker):
			t = threading.Thread(target=self.worker)
			#p = multiprocessing.Process(target=self.worker)
			t.daemon = True
			t.start()
			
	def worker(self):
		while True:
			self.get()()
			self.task_done()
			
	def run_class_methods(self, instance, methods_startswith = ''):
		for fn in [getattr(instance, k) for k, v in instance.__class__.__dict__.items() if isinstance(v, types.FunctionType) and k.startswith(methods_startswith)]:
			self.put(fn)
		self.join()
		
class WebZip:

	def __init__(self, url):
		self.zip = zipfile.ZipFile(StringIO.StringIO(urllib2.urlopen(url).read()))
				
	def __enter__(self):
		return self
		
	def __exit__(self, type, value, traceback):
		self.zip.close()
				
	def copy(self, pattern, target):
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
		self.req = urllib2.urlopen(url)
				
	def save(self, file):
		CHUNK = 16 * 1024
		with open(file, 'wb') as fp:
			while True:
				chunk = self.req.read(CHUNK)
				if not chunk: break
				fp.write(chunk)

class WebSource:

	def __init__(self, url):
		self.url = url
		
	def gettext(self):
		return requests.get(self.url).text
				
	def findlast(self, pattern):
		content = requests.get(self.url).content
		return sorted(re.findall( pattern, content))[-1]
		
	def find(self, pattern):
		content = requests.get(self.url).content
		res = re.search(pattern, content)
		return res.group(res.re.groups) 
		
	def etag(self):
		etag = requests.head(self.url).headers['etag']
		return re.search(r'[\w-]+', etag).group(0)

class Logger:

	def __init__(self, filename=os.path.basename(__file__) + '.log'):
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

class Tasks(VersionControl):

	def update_IE32(self):
		url = r"http://selenium-release.storage.googleapis.com/"
		url += WebSource(url).findlast( r'<Key>([\d\.]+/IEDriverServer_Win32_[\d\.]+.zip)')
		url +=  r'?etag=' + WebSource(url).etag()				
		dest_file = _ref_dir + 'IEDriverServer.exe'
		if self['IE32'] != url or not os.path.isfile(dest_file):
			with WebZip(url) as zip:
				zip.copy(r'IEDriverServer.exe', dest_file)
			print("Updated IE32 driver to version " + find_version_in_url(url) )
		self['IE32'] = url
	
	def update_SeleniumLibraries(self):
		url = r"http://selenium-release.storage.googleapis.com/"
		url += WebSource(url).findlast(r'<Key>([\d\.]+/selenium-dotnet-strongnamed-[\d\.]+.zip)')
		url +=  r'?etag=' + WebSource(url).etag()
		dest_file = _ref_dir + 'WebDriver.dll'
		if self['.NetLibraries'] != url or not os.path.isfile(dest_file):
			with WebZip(url) as zip:
				zip.copy(r'^net35/.', _ref_dir)
			print("Updated Selenium .Net: to version " + find_version_in_url(url) )
		self['.NetLibraries'] = url
	
	def update_IE64(self):
		url = r"http://selenium-release.storage.googleapis.com/"
		url += WebSource(url).findlast( r'<Key>([\d\.]+/IEDriverServer_x64_[\d\.]+.zip)')
		url +=  r'?etag=' + WebSource(url).etag()
		dest_file = _ref_dir + 'IEDriverServer64.exe'
		if self['IE64'] != url or not os.path.isfile(dest_file):
			with WebZip(url) as zip:
				zip.copy(r'IEDriverServer.exe', dest_file)
			print("Updated IE64 driver to version " + find_version_in_url(url) )
		self['IE64'] = url
    
	def update_PdfSharp(self):
		url = r'http://sourceforge.net/projects/pdfsharp/files/pdfsharp/'
		url += WebSource(url).findlast( r'href="/projects/pdfsharp/files/pdfsharp/(PDFsharp%20[^/]+/)')
		url = r'http://sunet.dl.sourceforge.net/project/pdfsharp/pdfsharp/' + WebSource(url).find(r'([^/]+/[^/]*Assemblies[^/]*\.zip)/download')
		url +=  r'?etag=' + WebSource(url).etag()
		dest_file = _ref_dir + 'PdfSharp.dll'
		if self['PDFsharp'] != url or not os.path.isfile(dest_file):
			with WebZip(url) as zip:
				zip.copy(r'.*/PdfSharp.dll', dest_file)
			print("Updated PDF Sharp to version " + find_version_in_url(url) )
		self['PDFsharp'] = url
	
	def update_SeleniumIDE(self):
		url = r'http://release.seleniumhq.org/selenium-ide/'
		url += WebSource(url).findlast(r'href="(\d[\d\.]+/)"')
		url += WebSource(url).find(r'href="(selenium-ide-[\d\.]+\.xpi)"')
		url +=  r'?etag=' + WebSource(url).etag()
		dest_file = _ref_dir + 'selenium-ide.xpi'
		if self['SeleniumIDE'] != url or not os.path.isfile(dest_file):
			WebFile(url).save(dest_file)
			print("Updated Selenium IDE to version " + find_version_in_url(url) )
		self['SeleniumIDE'] = url
    
	def update_ChromeDriver(self):
		url = r"http://chromedriver.storage.googleapis.com/"
		url += WebSource(url + r'LATEST_RELEASE').gettext().strip() + r'/chromedriver_win32.zip'
		url +=  r'?etag=' + WebSource(url).etag()
		dest_file = _ref_dir + 'chromedriver.exe'
		if self['Chrome'] != url or not os.path.isfile(dest_file):
			with WebZip(url) as zip:
				zip.copy(r'chromedriver.exe', dest_file)
			print("Updated Chrome driver to version " + find_version_in_url(url) )
		self['Chrome'] = url
	
	def update_PhantomJS(self):
		url = r"https://bitbucket.org/ariya/phantomjs/downloads"
		url += WebSource(url).findlast(r'href="/ariya/phantomjs/downloads(/phantomjs-[\d\.]+-windows.zip)"')
		url +=  r'?etag=' + WebSource(url).etag()
		dest_file = _ref_dir + 'phantomjs.exe'
		if self['PhantomJs'] != url or not os.path.isfile(dest_file):
			with WebZip(url) as zip:
				zip.copy(r'.*/phantomjs.exe', dest_file)
				zip.copy(r'.*/LICENSE.BSD', _ref_dir + 'phantomjs.license.txt')
			print("Updated PhantomJS to version " + find_version_in_url(url) )
		self['PhantomJs'] = url
    
	def update_Safari(self):
		url = r'https://selenium.googlecode.com/git/javascript/safari-driver/prebuilt/SafariDriver.safariextz'
		etag = WebSource(url).etag()
		url += '?etag=' + etag
		dest_file = _ref_dir + 'SafariDriver.safariextz'
		if self['Safari'] != url or not os.path.isfile(dest_file):
			WebFile(url).save(dest_file)
			print("Updated Safari driver to version " + etag )
		self['Safari'] = url

if __name__ == '__main__':
	with  Logger() as log:
		main()

	