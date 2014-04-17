import os, httplib2, re, urllib2, urllib, json, sys, traceback, urllib2, hashlib
from StringIO import StringIO
from zipfile import ZipFile

Project_name = 'SeleniumWrapper'
Current_dir = os.getcwd() + '\\'
Ref_dir = r'.\wrapper\References\\'
Log_path = r'.\wrapper\References\versions.txt'

def CheckPaths():
	for path in dir():
		if( path.endswith('_path') and not os.path.isfile(eval(path)) ) : print('Missing ' + path + '=' + eval(path));
		elif( path.endswith('_dir') and not os.path.isdir(eval(path)) ) : print('Missing ' + path + '=' + eval(path));

def CopyFileFromZip(zip, file_pattern, file_path):
	pattern = re.compile(file_pattern)
	for name in zip.namelist():
			if pattern.match(name):
				file = open(file_path, 'wb')
				file.write(zip.read(name))
				file.close()
				return
				
def ExtractFilesFromZip(zip, files_pattern, folder):
	p = re.compile(files_pattern)
	for name in zip.namelist():
			if p.match(name):
				file = open(folder.strip('\\') + '\\' + os.path.basename(name), 'wb')
				file.write(zip.read(name))
				file.close()		

def DownloadZip(url):
	return ZipFile(StringIO(urllib2.urlopen(url).read()))
	
def DownloadFile(url, dest_file):
	urllib.urlretrieve(url, dest_file)
		
		
if __name__ == '__main__':
	CheckPaths();

	print( "_______________________________________________________________________" )
	print( "" )
	print( "Project name     : " + Project_name )
	print( "_______________________________________________________________________\r\n" )

	#create folder if missing
	if not os.path.isdir(Ref_dir):
		os.makedirs(Ref_dir)
	
	#Load versions id from file or create if missing
	logdata = {'.NetLibraries':None, 'PDFsharp':None, 'SeleniumIDE':None, 'IE32':None, 'IE64':None, 'Chrome':None, 'PhantomJs':None, 'Safari':None}
	if os.path.isfile(Log_path):
		logfile = open(Log_path, 'r')
		for key, value in json.load(logfile).iteritems():
			if key in logdata :
				logdata[key] = value
		logfile.close()
	
	http = httplib2.Http()
	
	
	print("\nPDF Sharp...")
	try:
		appid = 'PDFsharp'
		dest_file = Ref_dir + 'PdfSharp.dll'
		url = r'http://sourceforge.net/projects/pdfsharp/files/pdfsharp/'
		headers, body = http.request(url)
		links = re.findall( r'href="/projects/pdfsharp/files/pdfsharp/(PDFsharp%20[^/]+/)', body)
		links.sort()
		url += links[-1]
		version = re.findall( r'\d[\d\.]*\d', urllib2.unquote(url) )[-1]
		headers, body = http.request( url )
		url = r'http://sunet.dl.sourceforge.net/project/pdfsharp/pdfsharp/' + re.findall( r'([^/]+/[^/]*Assemblies[^/]*\.zip)/download', body )[0]
		id = version + ' ' + url
		if logdata[appid] == id and os.path.isfile(dest_file):
			print(" No update!" )
		else:
			print(" Update to version " + version )
			zip = DownloadZip(url)
			ExtractFilesFromZip(zip, r'.*/PdfSharp.dll', Ref_dir)
			logdata[appid] = id
	except :
		traceback.print_exc()
	
	
	print("\nSelenium IDE...")
	try:
		appid = 'SeleniumIDE'
		dest_file = Ref_dir + 'selenium-ide.xpi'
		url = r'http://release.seleniumhq.org/selenium-ide/'
		#find last version
		headers, body = http.request(url)
		links = re.findall( r'href="(\d[\d\.]+/)"', body)
		links.sort()
		url += links[-1]
		headers, body = http.request(url)
		url += re.findall( r'href="(selenium-ide-[\d\.]+\.xpi)"', body)[0]
		version = re.findall( r'\d[\d\.]*\d', urllib2.unquote(url) )[-1]
		id = version + ' ' + url
		if logdata[appid] == id and os.path.isfile(dest_file):
			print(" No update!" )
		else:
			print(" Update to version " + version )
			urllib.urlretrieve( url, dest_file)
			logdata[appid] = id
	except :
		traceback.print_exc()
	
	
	print("\nSelenium libraries...")
	try:
		appid = '.NetLibraries'
		dest_file = Ref_dir + 'WebDriver.dll'
		rooturl = r"http://selenium-release.storage.googleapis.com/"
		#Find last version
		headers, body = http.request(rooturl + "?delimiter=/&prefix=")
		version = re.findall(r'<Prefix>(\d+\.\d+/)', body)[-1]
		#Get files list
		headers, body = http.request(rooturl + "?delimiter=/&prefix=" + version)
		node = '<Key>(' + version + '{0})</Key>'
		#download zip
		url = rooturl + re.findall( node.format(r'selenium-dotnet-[\d\.]+.zip'), body)[0]
		version = re.findall( r'\d[\d\.]*\d', urllib2.unquote(url) )[-1]
		id = version + ' ' + url
		if logdata[appid] == id and os.path.isfile(dest_file):
			print(" No update!" )
		else:
			print(" Update to version " + version )
			zip = DownloadZip(url)
			ExtractFilesFromZip(zip, r'^net35/.', Ref_dir)
			logdata[appid] = id
	except :
		traceback.print_exc()
	
	
	print("\nIE32 driver...")
	try:
		appid = 'IE32'
		dest_file = Ref_dir + 'IEDriverServer.exe'
		url = rooturl + re.findall( node.format(r'IEDriverServer_Win32_[\d\.]+.zip') ,body)[0]
		version = re.findall( r'\d[\d\.]*\d', urllib2.unquote(url) )[-1]
		id = version + ' ' + url
		if logdata[appid] == id and os.path.isfile(dest_file):
			print(" No update!" )
		else:
			version = re.findall( r'\d[\d\.]*\d', urllib2.unquote(url) )[-1]
			print(" Update to version " + version )
			zip = DownloadZip(url)
			CopyFileFromZip(zip, r'IEDriverServer.exe', dest_file)
			logdata[appid] = id
	except Exception, e:
		traceback.print_exc()
		
	
	print("\nIE64 driver...")
	try:
		appid = 'IE64'
		dest_file = Ref_dir + 'IEDriverServer64.exe'
		url = rooturl + re.findall( node.format(r'IEDriverServer_x64_[\d\.]+.zip') ,body)[0]
		version = re.findall( r'\d[\d\.]*\d', urllib2.unquote(url) )[-1]
		id = version + ' ' + url
		if logdata[appid] == id and os.path.isfile(dest_file):
			print(" No update!" )
		else:
			print(" Update to version " + version )
			zip = DownloadZip(url)
			CopyFileFromZip(zip, r'IEDriverServer.exe', dest_file)
			logdata[appid] = id
	except :
		traceback.print_exc()
		
		
	print("\nChrome driver...")
	try:
		appid = 'Chrome'
		dest_file = Ref_dir + 'chromedriver.exe'
		url = r"http://chromedriver.storage.googleapis.com"
		headers, body = http.request(url + r'/LATEST_RELEASE')
		version = body.strip()
		url += r'/{0}/chromedriver_win32.zip'.format(version)
		id = version + ' ' + url
		if logdata[appid] == id and os.path.isfile(dest_file):
			print(" No update!" )
		else:
			print(" Update to version " + version )
			zip = DownloadZip(url)
			CopyFileFromZip(zip, r'chromedriver.exe', dest_file)
			logdata[appid] = id
	except :
		traceback.print_exc()


	print("\nPhantomJS driver...")
	try:
		appid = 'PhantomJs'
		dest_file = Ref_dir + 'phantomjs.exe'
		url = r"https://bitbucket.org/ariya/phantomjs/downloads"
		headers, body = httplib2.Http(disable_ssl_certificate_validation=True).request(url)
		links =  re.findall(r'href="/ariya/phantomjs/downloads(/phantomjs-[\d\.]+-windows.zip)"', body)
		links.sort()
		url  += links[-1]
		version = re.findall( r'\d[\d\.]*\d', urllib2.unquote(url) )[-1]
		id = version + ' ' + url
		if logdata[appid] == id and os.path.isfile(dest_file):
			print(" No update!" )
		else:
			print(" Update to version " + version )
			zip = DownloadZip(url)
			CopyFileFromZip(zip, r'.*/phantomjs.exe', dest_file)
			CopyFileFromZip(zip, r'.*/LICENSE.BSD', Ref_dir + 'phantomjs.license.txt')
			logdata[appid] = id
	except :
		traceback.print_exc()
	
	
	print( "\nSafari driver...")
	try:
		appid = 'Safari'
		dest_file = Ref_dir + 'SafariDriver.safariextz'
		url = r'https://selenium.googlecode.com/git/javascript/safari-driver/prebuilt/SafariDriver.safariextz'
		version = hashlib.md5( http.request(url, "HEAD")[0]['etag'] ).hexdigest()
		id = version + ' ' + url
		if logdata[appid] == id and os.path.isfile(dest_file):
			print(" No update!" )
		else:
			print(" Update to version " + version )
			urllib.urlretrieve(url, dest_file)
			logdata[appid] = id
	except :
		traceback.print_exc()
				
				
	#save versions
	logfile = open(Log_path, 'w')
	json.dump(logdata, logfile, sort_keys = False, indent = 4, ensure_ascii=False)
	logfile.close()

	print('\nEND\n')
	