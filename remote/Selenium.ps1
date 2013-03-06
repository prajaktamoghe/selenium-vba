
# Allow script execution : Set-ExecutionPolicy RemoteSigned

#$enum = [Selenium.DefaultSelenium+ErrLevel]
#[enum]::GetValues($enum) | select @{Name='Value';e={[int]($_ -as $enum)}},@{Name='Name';e={$_ -as $enum}}
#[int][Selenium.DefaultSelenium+ErrLevel]::server
#write-error("Err level : " + [int]$s.ErrType + ", " + $s.ErrType)
#write-host "Err level : " + [int]$s.ErrType + ", " + $s.ErrType -f red -b yellow
#write-error /$($_.Exception.Message)
#if ($s.ErrType -eq [int][Selenium.DefaultSelenium+ErrLevel]::verify ) {
#($error[0].InvocationInfo.ScriptName | split-path -Leaf)
#write-host ( "ERROR L" + ($error[0].InvocationInfo.ScriptLineNumber) + " : " + ($error[0].InvocationInfo.Line) + " : " + ($s.ErrInfo) ) -f red; 

cls
trap [Exception] {
    write-host (" ERROR L" + ($error[0].InvocationInfo.ScriptLineNumber) + " : " + ($error[0].InvocationInfo.Line) + " : " + ($s.ErrInfo)) -ForegroundColor Red; 
    if ($s.ErrType -eq "verify") { continue } else{ return }
}
"Start : " + ($myInvocation.MyCommand.Definition | split-path -Leaf)
$s = New-Object -com Selenium.Driver;


#$s.Start("localhost", 4444, "*iehta", "http://www.google.com");
#$s.open("/")
$s.type("q","test");
$s.verifyValue("q","f2")
$s.assertValue("q","tes")
$s.assertValue("q","f6")


"Finished"




 
