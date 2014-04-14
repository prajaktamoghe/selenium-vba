import os, httplib2, re, urllib2, urllib, json
from StringIO import StringIO
from zipfile import ZipFile

Project_name = 'SeleniumWrapper'
Current_dir = os.getcwd() + '\\'
#Ref_dir = '.\wrapper\References\'
Ref_dir = r'.\wrapper\References\\'
Log_path = r'.\wrapper\References\log.txt'

def CheckPaths():
	for path in dir():
		if( path.endswith('_path') and not os.path.isfile(eval(path)) ) : print('Missing ' + path + '=' + eval(path));
		elif( path.endswith('_dir') and not os.path.isdir(eval(path)) ) : print('Missing ' + path + '=' + eval(path));

def GetEtag(url):
	return httplib2.Http().request(url, "HEAD")['etag']
		
if __name__ == '__main__':
	CheckPaths();

	print( "_______________________________________________________________________" )
	print( "" )
	print( "Project name     : " + Project_name )
	print( "_______________________________________________________________________\r\n" )

	#Load versions id from file or create if missing
	if not os.path.isfile(Log_path):
		logdata = {'Selenium':None, 'IE32':None, 'IE64':None, 'Chrome':None, 'PhantomJs':None, 'Safari':None}
	else:
		logstr = open(Log_path, 'r')
		logdata = json.load(logstr)
		logstr.close()
	
	print(".Net libraries...")
	rooturl = r"http://selenium-release.storage.googleapis.com/"
	#Find last version
	headers, body = httplib2.Http().request(rooturl + "?delimiter=/&prefix=")
	version = re.findall(r'<Prefix>(\d+\.\d+/)', body)[-1]
	#Get files list
	headers, body = httplib2.Http().request(rooturl + "?delimiter=/&prefix=" + version)
	node = '<Key>(' + version + '{0})</Key>'
	
	#download zip
	url = rooturl + re.findall( node.format(r'selenium-dotnet-[\d\.]+.zip'), body)[0]
	if logdata['Selenium'] == url and os.path.isfile(Ref_dir + 'WebDriver.dll'):
		print(" No update!" )
	else:
		print(" Update to version " + re.findall( r'\d\.[\d\.]+', url)[0] )
		zip = ZipFile(StringIO(  urllib2.urlopen(url).read()))
		#extract zip
		for name in zip.namelist():
			if re.match('^net35/.', name):
				file = open(Ref_dir + os.path.basename(name), 'wb')
				file.write(zip.read(name))
				file.close()
		logdata['Selenium'] = url
	
	print("IEDriverServer 32 bits...")
	url = rooturl + re.findall( node.format(r'IEDriverServer_Win32_[\d\.]+.zip') ,body)[0]
	if logdata['IE32'] == url and os.path.isfile(Ref_dir + 'IEDriverServer.exe'):
		print(" No update!" )
	else:
		print(" Update to version " + re.findall( r'\d\.[\d\.]+', url)[0] )
		zip = ZipFile(StringIO(  urllib2.urlopen(url).read()))
		open(Ref_dir + 'IEDriverServer.exe', 'wb').write(zip.read('IEDriverServer.exe'))
		logdata['IE32'] = url
	
	
	print("IEDriverServer 64 bits...")
	url = rooturl + re.findall( node.format(r'IEDriverServer_x64_[\d\.]+.zip') ,body)[0]
	if logdata['IE64'] == url and os.path.isfile(Ref_dir + 'IEDriverServer64.exe'):
		print(" No update!" )
	else:
		print(" Update to version " + re.findall( r'\d\.[\d\.]+', url)[0] )
		zip = ZipFile(StringIO(  urllib2.urlopen(url).read()))
		open(Ref_dir + 'IEDriverServer64.exe', 'wb').write(zip.read('IEDriverServer.exe'))
		logdata['IE64'] = url
		
		
	print( "Chrome driver...")	
	headers, body = httplib2.Http().request(r"http://chromedriver.storage.googleapis.com/LATEST_RELEASE")
	url = r'http://chromedriver.storage.googleapis.com/{0}/chromedriver_win32.zip'.format(body)
	if logdata['Chrome'] == url :
		print(" No update!" )
	else:
		print(" Update to version " + body )
		zip = ZipFile(StringIO(  urllib2.urlopen(url).read()))
		zip.extract('chromedriver.exe', Ref_dir)
		logdata['Chrome'] = url


	print("PhantomJS driver...")	
	headers, body = httplib2.Http(disable_ssl_certificate_validation=True).request(r"https://bitbucket.org/ariya/phantomjs/downloads")
	urls =  re.findall(r'href="(/ariya/phantomjs/downloads/phantomjs-[\d\.]+-windows.zip)"', body)
	urls.sort()
	url = r'https://bitbucket.org' + urls[-1]
	if logdata['PhantomJs'] == url and os.path.isfile(Ref_dir + 'phantomjs.exe'):
		print(" No update!" )
	else:
		print(" Update to version " + re.findall(r'\d[\d\.]+', url)[0] )
		zip = ZipFile(StringIO(  urllib2.urlopen(url).read()))
		for name in zip.namelist():
			if name.endswith('phantomjs.exe'):	
				file = open(Ref_dir + 'phantomjs.exe', 'wb')
				file.write(zip.read(name))
				file.close()
		logdata['PhantomJs'] = url
	
	print( "Safari driver...")
	etag = httplib2.Http().request(r'https://selenium.googlecode.com/git/javascript/safari-driver/prebuilt/SafariDriver.safariextz', "HEAD")[0]['etag']
	if logdata['Safari'] == etag and os.path.isfile(Ref_dir + 'SafariDriver.safariextz'):
		print(" No update!" )
	else:
		print("  New version")
		urllib.urlretrieve(r'https://selenium.googlecode.com/git/javascript/safari-driver/prebuilt/SafariDriver.safariextz', Ref_dir + 'SafariDriver.safariextz')
		logdata['Safari'] = etag
				
	#save versions
	logfile = open(Log_path, 'w')
	json.dump(logdata, logfile, sort_keys = False, indent = 4, ensure_ascii=False)
	logfile.close()

	