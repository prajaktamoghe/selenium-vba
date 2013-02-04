
Set driver = CreateObject("SeleniumWrapper.WebDriver")
driver.setPreference "network.http.phishy-userpass-length", 255
driver.setPreference "network.automatic-ntlm-auth.trusted-uris", "domain.com"

driver.start "firefox", "https://username:password@www.domain.com"
driver.open "/"

driver.stop

